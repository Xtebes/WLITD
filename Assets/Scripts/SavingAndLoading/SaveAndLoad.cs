using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
public class SaveAndLoad : MonoBehaviour
{
    [SerializeField]
    Transform saveSlotsParent;
    [SerializeField]
    GameObject emptySlot, saveSlot, popUp;
    TextMeshProUGUI popUpText, popUpTitle;
    Button confirmButton, cancelButton;
    public static readonly string
    baseSavedGameFolder = "SavedGame",
    saveFileName = "Save.save",
    detailsSaveFileName = "details.save",
    itemsCollectedFileName = "items.save",
    doorsUnlockedFileName = "doors.save";
    void PopUp(string title, string text)
    {
        var canvasGroup = popUp.GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        popUpTitle.text = title;
        popUpText.text = text;
        popUp.SetActive(true);
        Tween tween = popUp.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce);
        tween.onComplete += delegate { canvasGroup.interactable = true; };
    }
    void PopDown()
    {
        confirmButton.onClick.RemoveAllListeners();
        var canvasGroup = popUp.GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        Tween tween = popUp.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBounce);
        tween.onComplete += delegate { popUp.SetActive(false); };
    }
    void PopDown(out Tween tween)
    {
        confirmButton.onClick.RemoveAllListeners();
        var canvasGroup = popUp.GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        tween = popUp.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBounce);
        tween.onComplete += delegate { popUp.SetActive(false); };
    }
    private void Start()
    {
        TextMeshProUGUI[] texts = popUp.GetComponentsInChildren<TextMeshProUGUI>();
        Button[] buttons = popUp.GetComponentsInChildren<Button>();
        popUpText = texts[1];
        popUpTitle = texts[0];
        confirmButton = buttons[0];
        cancelButton = buttons[1];
        confirmButton.AddScalingToButton();
        cancelButton.AddScalingToButton();
        cancelButton.onClick.AddListener(PopDown);
    }
    private void OnEnable()
    {
        LoadSaveSlots();
        popUp.SetActive(false);
        if (confirmButton != null)
            confirmButton.onClick.RemoveAllListeners();
        popUp.transform.localScale = Vector3.zero;
    }
    private static void DeleteGame(string folderPath)
    {
        foreach (string file in Directory.GetFiles(folderPath))
        {
            File.Delete(file);
        }
        Directory.Delete(folderPath);
    }
    private static void SaveGame(string folderPath)
    {
        string savePath = folderPath + Path.DirectorySeparatorChar + saveFileName;
        string detailsPath = folderPath + Path.DirectorySeparatorChar + detailsSaveFileName;
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
        using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, SavedInfo.save);
        }
        using (FileStream fs = new FileStream(detailsPath, FileMode.OpenOrCreate))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, SavedInfo.gameDetails);
        }
    }
    private static void LoadGame(string folderPath)
    {
        string savePath = folderPath + Path.DirectorySeparatorChar + saveFileName;
        string detailsPath = folderPath + Path.DirectorySeparatorChar + detailsSaveFileName;
        using (FileStream fs = new FileStream(savePath, FileMode.Open))
        {
            BinaryFormatter bf = new BinaryFormatter();
            SavedInfo.save = (Save)bf.Deserialize(fs);
        }
        using (FileStream fs = new FileStream(detailsPath, FileMode.Open))
        {
            BinaryFormatter bf = new BinaryFormatter();
            SavedInfo.gameDetails = (GameDetails)bf.Deserialize(fs);
        }
        Singleton<GameManager>.Instance.StartCoroutine(Singleton<GameManager>.Instance.LoadGame());
    }
    private static void NewGame(string folder)
    {
        Singleton<GameManager>.Instance.newGame = true;
        SavedInfo.gameDetails = new GameDetails();
        SavedInfo.gameDetails.currentRoomName = "Entrance";
        SavedInfo.save = new Save();
        Directory.CreateDirectory(folder);
        SaveGame(folder);
        LoadGame(folder);
    }
    private void TrySaveGame(string folder, Tween tween)
    {
        if (Singleton<GameManager>.Instance.currentRoom.safeZone)
        {
            SaveGame(folder);
            LoadSaveSlots();
        }
        else
            tween.onComplete += delegate { PopUp("Save Failed", "Please find an area with clear signal."); };
    }
    public void LoadSaveSlots()
    {
        saveSlotsParent.DestroyAllFirstLevelChildren();
        for (int i = 1; i < 6; i++)
        {
            int index = i;
            string saveFolderPath = Application.persistentDataPath + Path.DirectorySeparatorChar + baseSavedGameFolder + i;
            if (Directory.Exists(saveFolderPath))
            {
                GameDetails details;
                using (FileStream fl = new FileStream(saveFolderPath + Path.DirectorySeparatorChar + detailsSaveFileName, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    details = (GameDetails)bf.Deserialize(fl);
                }
                GameObject saveSlotInstance = Instantiate(saveSlot, saveSlotsParent);
                SavedSlot slot = saveSlotInstance.GetComponent<SavedSlot>();
                slot.slotNumber.text = index.ToString();
                slot.LoadSlot(details);
                if (slot.saveButton != null)
                {
                    slot.saveButton.AddScalingToButton();
                    slot.saveButton.onClick.AddListener(delegate
                    {
                        PopUp("Save", $"Are you sure you want to overwrite slot {index}?");
                        confirmButton.onClick.AddListener(delegate { Tween tween; PopDown(out tween); TrySaveGame(saveFolderPath, tween); });
                    });
                }
                slot.loadButton.AddScalingToButton();
                slot.loadButton.onClick.AddListener(delegate
                {
                    PopUp("Load", $"Are you sure you want to load slot {index}?");
                    confirmButton.onClick.AddListener(delegate { LoadGame(saveFolderPath); PopDown(); });
                });
                slot.deleteButton.AddScalingToButton();
                slot.deleteButton.onClick.AddListener(delegate
                {
                    PopUp("Delete", $"Are you sure you want to delete slot {index}?");
                    confirmButton.onClick.AddListener(delegate { DeleteGame(saveFolderPath); PopDown(); LoadSaveSlots(); });
                });
            }
            else
            {
                GameObject emptySlotInstance = Instantiate(emptySlot, saveSlotsParent);
                EmptySlot slot = emptySlotInstance.GetComponent<EmptySlot>();
                slot.slotNumber.text = index.ToString();
                slot.newGameButton.AddScalingToButton();
                slot.newGameButton.onClick.AddListener(delegate
                {
                    PopUp("New Game", $"Are you sure you want to start a new game on slot {index}?");
                    confirmButton.onClick.AddListener(delegate { NewGame(saveFolderPath); PopDown(); });
                });
                if (slot.saveButton != null)
                {
                    slot.saveButton.AddScalingToButton();
                    slot.saveButton.onClick.AddListener(delegate
                    {
                        PopUp("Save", $"Are you sure you want to save on slot {index}?");
                        confirmButton.onClick.AddListener(delegate { Tween tween; PopDown(out tween); TrySaveGame(saveFolderPath, tween); });
                    });
                }
            }
        }
    }
}

public static class SavedInfo
{
    public static Save save;
    public static GameDetails gameDetails;
}
[Serializable]
public class Save
{
    public PlayerInfo playerInfo;
    public bool[] isItemCollected;
    public bool[] isDoorLocked;
    public bool[] isMessageTriggered;
    public List<MessageLog> logs;
    public Save()
    {
        playerInfo = new PlayerInfo();
        logs = new List<MessageLog>();
        foreach (MessageScreen.contacts contact in Enum.GetValues(typeof(MessageScreen.contacts)))
        {
            MessageLog log = new MessageLog();
            log.contact = contact;
            logs.Add(log);
        }
    }
}
[Serializable]
public class PlayerInfo
{
    public Lantern equippedLantern;
    public Inventory inventory;
    public PlayerInfo()
    {
        equippedLantern = Activator.CreateInstance<PhoneLantern>();
        inventory = new Inventory();
    }
}
[Serializable]
public class GameDetails
{
    public string currentRoomName;
    public int timePlayed;
}
[Serializable]
public class Inventory
{
    public static void LoadEntries()
    {
        itemIndexToType = new Dictionary<ItemType, Type>()
        {
            {ItemType.phoneLantern, typeof(PhoneLantern)},
            {ItemType.standaloneLantern, typeof(StandaloneLantern)},
            {ItemType.livingRoomKey, typeof(SilverKey) },
            {ItemType.hallKey, typeof(BronzeKey) },
            {ItemType.libraryKey, typeof(GoldKey) },
            {ItemType.shinjiNote, typeof(ShinjiNote) },
            {ItemType.libraryNote, typeof(LibraryNote) },
            {ItemType.libraryBook, typeof(LibraryBook) },
            {ItemType.camera, typeof(CameraItem) },
            {ItemType.film, typeof(CameraFilm) },
        };
        itemTypeToIndex = itemIndexToType.ToDictionary(i => i.Value, i => i.Key);
    }
    public static Dictionary<ItemType, Type> itemIndexToType;
    public Dictionary<Type, int> items;
    public static Dictionary<Type, ItemType> itemTypeToIndex;
    public Inventory()
    {
        items = new Dictionary<Type, int>();
        itemIndexToType = new Dictionary<ItemType, Type>();
        LoadEntries();
        foreach (Type type in itemIndexToType.Values)
        {
            items.Add(type, 0);
        }
    }
    public enum ItemType
    {
        phoneLantern,
        standaloneLantern,
        livingRoomKey,
        hallKey,
        libraryKey,
        shinjiNote,
        libraryNote,
        libraryBook,
        camera,
        film,
    }
    public bool CanCraft(Item item)
    {
        if (item.recipe == null)
            return false;
        else
        {
            foreach (Type type in item.recipe)
            {
                if (items[type] == 0)
                    return false;
            }
        }
        return true;
    }
    public void Craft(Item item)
    {
        foreach (Type itemType in item.recipe)
        {
            items[itemType] -= 1;
        }
        items[item.GetType()] += 1;
    }
}
#region itemTypes
[Serializable]
public abstract class Item
{
    public Action useAction;
    public List<Type> recipe;
    public string name;
    public string description;
    public virtual void Action() { }
}
[Serializable]
public abstract class Lantern : Item
{
    public Lantern()
    {
        useAction = Action;
    }
    public static Action<Lantern> onEquip;
    public float runningLengthMultiplier, walkingLengthMultiplier;
    public override void Action()
    {
        SavedInfo.save.playerInfo.inventory.items[SavedInfo.save.playerInfo.equippedLantern.GetType()] = 1;
        SavedInfo.save.playerInfo.inventory.items[GetType()] = 0;
        SavedInfo.save.playerInfo.equippedLantern = (Lantern)Activator.CreateInstance(GetType());
        onEquip.Invoke(SavedInfo.save.playerInfo.equippedLantern);
    }
}
[Serializable]
public abstract class Key : Item
{
    public static Action doorUnlocked;
    public int doorId;
    public Key()
    {
        useAction = Action;
    }
    public override void Action()
    {
        Door[] doors = Extensions.GetObjectsInAreaWithComponent<Door>(Singleton<Player>.Instance.transform.position, 1);
        if (doors.Length == 0)
        {
            Singleton<GameManager>.Instance.interactionPopUp.PopUp("There isn't any lock nearby");
            return;
        }
        foreach (Door door in doors)
        {
            if (Inventory.itemIndexToType[door.key] == GetType())
            {
                if (door.isLocked)
                {
                    Singleton<GameManager>.Instance.interactionPopUp.PopUp("Door has been unlocked");
                    SavedInfo.save.playerInfo.inventory.items[GetType()] -= 1;
                    door.Unlock();
                }
                else
                    Singleton<GameManager>.Instance.interactionPopUp.PopUp("Door has already been unlocked");
                return;
            }
        }
        Singleton<GameManager>.Instance.interactionPopUp.PopUp("The key doesn't fit");
    }
}
#endregion
#region Items
[Serializable]
public class GoldKey : Key
{
    public GoldKey()
    {
        name = "Key";
        description = "An heavy golden key";
    }
}
[Serializable]
public class BronzeKey : Key
{
    public BronzeKey()
    {
        name = "Key";
        description = "An old looking bronze key";
    }
}
[Serializable]
public class SilverKey : Key
{
    public SilverKey()
    {
        name = "Key";
        description = "These keys seem to come in pairs. As such, it looks like they belong to a room with multiple doors.";
    }
}
[Serializable]
public class PhoneLantern : Lantern
{
    public PhoneLantern()
    {
        name = "Lantern";
        description = "Your phones' lantern.";
        walkingLengthMultiplier = 1;
        runningLengthMultiplier = 0.4f;
    }
}
[Serializable]
public class StandaloneLantern : Lantern
{
    public StandaloneLantern()
    {
        name = "Lantern";
        description = "A LED Flashlight with some pretty durable batteries.";
        walkingLengthMultiplier = 2.2f;
        runningLengthMultiplier = 1;
    }
}
[Serializable]
public class LibraryNote : Item
{
    public LibraryNote()
    {
        name = "Note";
        description = "'Until further notice, the library access will be closed to the house residents, " +
                      "spare keys will be given to be used in case of emergency'";
    }
}
public class ShinjiNote : Item
{
    public ShinjiNote()
    {
        name = "Note";
        description = "'About Shinji: I have decided to install a security door to the entrance of my grandsons' bedroom hallway. " +
                      "He's been acting strange lately and his night time escapades are becoming more frequent. " +
                      "The Keys Will be in my office if the staff needs it.'";
    }
}
[Serializable]
public class LibraryBook : Item
{
    public LibraryBook()
    {
        name = "Book";
        description = "'HP Lovecraft's Call Of Cthullu. Inside it theres a special note inside: 'To my biggest fan Mr.Kobayashi, " +
                      "keep searching for the unknown.'";
    }
}
[Serializable]

public class CameraItem : Item
{
    public CameraItem()
    {
        name = "Camera";
        description = "A late nineties camera model, the lens is completely shattered";
    }
}
public class CameraFilm : Item
{
    public CameraFilm()
    {
        name = "Film";
        description = "In the reactive there's a picture of a wall with a password painted in blood on it.";
    }
}
#endregion
