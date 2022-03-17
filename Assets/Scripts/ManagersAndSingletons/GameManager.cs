using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Video;

public class GameManager : Singleton<GameManager>
{
    public bool newGame = false;
    public GameObject loadingScreen;
    [HideInInspector]
    public InteractionPopUp interactionPopUp;
    public Room[] rooms;
    private Room _currentRoom;
    public VideoClip[] cutscenes;
    public bool isOnCutscene;
    private Player player;
    public Room currentRoom
    {
        set
        {
            _currentRoom = value;
            SavedInfo.gameDetails.currentRoomName = value.name;
        }
        get
        {
            return _currentRoom;
        }
    }
    private void SkipCutscene(Coroutine cutscene)
    {
        StopCoroutine(cutscene);
        cutscene = null;
        FadeInOut.player.Stop();
        FadeInOut.canvasGroup.DOFade(0, 0.5f).onComplete += delegate { isOnCutscene = false; };
    }
    public IEnumerator LoadGame()
    {
        //Transition To Loading Screen
        loadingScreen.SetActive(true);
        Slider loadingBar = loadingScreen.GetComponentInChildren<Slider>();
        CanvasGroup canvasGroup = loadingScreen.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        //Start loading
        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync("Game");
        while (!sceneLoading.isDone)
        {
            loadingBar.value = Mathf.Clamp01(sceneLoading.progress / 0.9f);
            yield return null;
        }
        player = FindObjectOfType<Player>();
        interactionPopUp = FindObjectOfType<InteractionPopUp>(true);
        rooms = FindObjectsOfType<Room>();
        Debug.Log($"Found {rooms.Length} rooms");
        foreach (Room room in rooms)
        {
            if (room.name == SavedInfo.gameDetails.currentRoomName)
            {
                currentRoom = room;
                Debug.Log($"Spawning in room {room.name} on {room.zone}");
                player.transform.position = currentRoom.roomSpawnPoint.position;
                break;
            }
        }
        MessageSender[] messages = FindObjectsOfType<MessageSender>();
        if (SavedInfo.save.isMessageTriggered == null)
        {
            SavedInfo.save.isMessageTriggered = new bool[messages.Length];
            for (int i = 0; i < SavedInfo.save.isMessageTriggered.Length; i++)
            {
                SavedInfo.save.isMessageTriggered[i] = false;
            }
        }
        for (int i = 0; i < SavedInfo.save.isMessageTriggered.Length; i++)
        {
            if (SavedInfo.save.isMessageTriggered[i])
            {
                Destroy(messages[i].gameObject);
            }
            else
            {
                messages[i].index = i;
            }
        }
        Inventory.LoadEntries();
        PlaceableItem[] items = FindObjectsOfType<PlaceableItem>();
        if (SavedInfo.save.isItemCollected == null)
        {
            SavedInfo.save.isItemCollected = new bool[items.Length];
            for (int i = 0; i < SavedInfo.save.isItemCollected.Length; i++)
            {
                SavedInfo.save.isItemCollected[i] = false;
            }
        }
        for (int i = 0; i < SavedInfo.save.isItemCollected.Length; i++)
        {
            if (SavedInfo.save.isItemCollected[i])
            {
                Destroy(items[i].gameObject);
            }
            else
            {
                items[i].Load(player, i);
            }
        }
        Door[] doors = FindObjectsOfType<Door>();
        if (SavedInfo.save.isDoorLocked == null)
        {
            SavedInfo.save.isDoorLocked = new bool[doors.Length];
            for (int i = 0 ; i < SavedInfo.save.isDoorLocked.Length; i++)
            {
                SavedInfo.save.isDoorLocked[i] = doors[i].isLocked;
            }
        }
        for (int index = 0; index < doors.Length; index++)
        {
            doors[index].Load(index, SavedInfo.save.isDoorLocked[index]);
        }
        if (newGame)
        {
            newGame = false;
            loadingScreen.SetActive(false);
            Coroutine cutscene = StartCoroutine(FadeInOut.FadeInAndOutCutscene(0, cutscenes[0], 0.5f));
            player.input.worldInteraction.Enable();
            isOnCutscene = true;
            player.input.worldInteractionAction += ()=> SkipCutscene(cutscene);
            while(isOnCutscene == true)
            {
                yield return null;
            }
            player.input.worldInteractionAction -= ()=> SkipCutscene(cutscene);
        }
        else
        {
            canvasGroup.DOFade(0, 0.5f);
            yield return new WaitForSeconds(0.5f);
            loadingScreen.SetActive(false);
        }
        FindObjectOfType<MonsterAI>().StartMonster();
        player.input.inputActionAsset.Enable();
        player.input.worldInteraction.Disable();
    }
}
