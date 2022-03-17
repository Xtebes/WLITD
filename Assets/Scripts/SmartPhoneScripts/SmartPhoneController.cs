using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SmartPhoneController : MonoBehaviour, ImLoadedByPlayer
{  
    public CanvasGroup phoneCanvasGroup;
    [SerializeField]
    private Transform phonePivot;
    [SerializeField]
    private GameObject[] tabs;
    PhoneHomeScreen phoneHomeScreen;
    [SerializeField]
    Button mainButton, saveAppButton, messageAppButton, exitButton;
    public GameObject smartPhoneBody, exitMenu;
    float duration = 0.3f;
    int activeScreenIndex = 0;
    Vector3 horizontalPosition;
    Vector3 verticalPosition;
    private Orientation orientation;
    private SmartPhoneScreen smartPhoneScreen;
    public enum Orientation
    {
        horizontal,
        vertical,
    }
    void ImLoadedByPlayer.Load(Player player)
    {
        player.input.togglePhone.performed += delegate
        {
            Tween Animation;
            player.input.inputActionAsset.actionMaps[0].Disable();
            bool isPhoneOn = ToggleSmartPhone(out Animation);
            Animation.onComplete += delegate
            {
                player.input.togglePhone.Enable();
                if (!isPhoneOn)
                    player.input.inputActionAsset.actionMaps[0].Enable();
            };
        };
        orientation = Orientation.vertical;
        phoneHomeScreen = GetComponentInChildren<PhoneHomeScreen>(true);
        verticalPosition = phonePivot.transform.position;
        horizontalPosition = new Vector2(Screen.width - 8, 0);
        for (int i = 1; i < tabs.Length; i++)
        {
            Transform screen;
            screen = tabs[i].transform;
            screen.localScale = Vector3.zero;
            screen.gameObject.SetActive(false);
        }
        saveAppButton.onClick.AddListener(delegate { SetActiveScreen(SmartPhoneScreen.save); });
        messageAppButton.onClick.AddListener(delegate { SetActiveScreen(SmartPhoneScreen.message); SetHorientation(Orientation.horizontal); });
        mainButton.onClick.AddListener(delegate { SetActiveScreen(SmartPhoneScreen.main); SetHorientation(Orientation.vertical); });
        exitButton.onClick.AddListener(delegate { exitMenu.SetActive(true);});
        Button[] exitMenuButtons = exitMenu.GetComponentsInChildren<Button>();
        exitMenuButtons[1].onClick.AddListener(delegate { SceneManager.LoadScene("MainMenu"); });
        exitMenuButtons[0].onClick.AddListener(delegate { Application.Quit(); });
        exitMenuButtons[2].onClick.AddListener(delegate { exitMenu.SetActive(false); });
        ToggleSmartPhone(true);
    }

    public enum SmartPhoneScreen
    {
        main = 0,
        inspector = 1,
        save = 2,
        message = 3,
    }
    public void SetActiveScreen(SmartPhoneScreen screen)
    {
        exitMenu.SetActive(false);
        if (screen == smartPhoneScreen)
            return;
        smartPhoneScreen = screen;
        Tween shrink = tabs[activeScreenIndex].transform.DOScale(Vector3.zero, duration / 2);
        shrink.onComplete += delegate 
        { 
            tabs[(int)screen].transform.DOScale(Vector3.one, duration / 2);
            tabs[activeScreenIndex].SetActive(false);
            activeScreenIndex = (int)screen;
            tabs[activeScreenIndex].gameObject.SetActive(true);
        };
    }
    public bool ToggleSmartPhone(bool immediate = false)
    {
        orientation = Orientation.vertical;
        if (smartPhoneBody.activeSelf)
        {
            if (immediate)
            {
                phonePivot.transform.position = new Vector3(phonePivot.position.x, phonePivot.position.y - Screen.height - 8, 0);
                smartPhoneBody.SetActive(false);
            }
            else
            {
                Tween tween = phonePivot.DOMove(new Vector3(phonePivot.position.x, phonePivot.position.y - Screen.height - 8, 0), duration);
                tween.onComplete += delegate { smartPhoneBody.SetActive(false); };
            }
            return false;
        }
        else
        {
            smartPhoneBody.SetActive(true);
            phonePivot.eulerAngles = Vector3.zero;
            phonePivot.position = new Vector3(verticalPosition.x, verticalPosition.y - Screen.height - 8, 0);
            if (immediate)
            {
                phonePivot.position = verticalPosition;
            }
            else
            {
                phonePivot.DOMove(verticalPosition, duration);
            }
            SetActiveScreen(SmartPhoneScreen.main);
            phoneHomeScreen.EnableInventory(SavedInfo.save.playerInfo.inventory);
            return true;
        }
    }
    public bool ToggleSmartPhone(out Tween tween)
    {
        if (smartPhoneBody.activeSelf)
        {
            tween = phonePivot.DOMove(new Vector3(phonePivot.position.x, phonePivot.position.y - Screen.height - 8, 0), duration);
            tween.onComplete += delegate { smartPhoneBody.SetActive(false); };
            return false;
        }
        else
        {
            smartPhoneBody.SetActive(true);
            phonePivot.eulerAngles = Vector3.zero;
            phonePivot.position = new Vector3(verticalPosition.x, verticalPosition.y - Screen.height - 8, 0);
            tween = phonePivot.DOMove(verticalPosition, duration);
            SetActiveScreen(SmartPhoneScreen.main);
            phoneHomeScreen.EnableInventory(SavedInfo.save.playerInfo.inventory);
            return true;
        }
    }
    public void SetHorientation(Orientation orientation)
    {
       
        this.orientation = orientation;
        Tween tweenRotation;
        phoneCanvasGroup.interactable = false;
        if (orientation == Orientation.horizontal)
        {
            tweenRotation = phonePivot.DOMove(horizontalPosition, duration);
            phonePivot.DORotate(new Vector3(0, 0, 90), duration);
        }
        else
        {
            tweenRotation = phonePivot.DOMove(verticalPosition, duration);
            phonePivot.DORotate(new Vector3(0, 0, 0), duration);
        }
        tweenRotation.onComplete += delegate { phoneCanvasGroup.interactable = true; };
    }
}
