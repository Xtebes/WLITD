using System.Collections;
using UnityEngine;
public class Door : Interactable
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip openDoorAudio, closeDoorAudio, lockedDoorAudio, unlockedDoorAudio;
    private Coroutine coroutine;
    private bool isOpened = false;
    private float passedTime;
    private int index;
    public bool isLocked;
    public Inventory.ItemType key;
    [SerializeField]
    private GameObject doorClosed, doorOpened;
    public void Unlock()
    {
        audioSource.clip = unlockedDoorAudio;
        audioSource.Play();
        isLocked = false;
        SavedInfo.save.isDoorLocked[index] = false;
    }
    void OpenDoor()
    {
        audioSource.clip = openDoorAudio;
        audioSource.Play();
        doorClosed.SetActive(false);
        doorOpened.SetActive(true);
        isOpened = true;
    }
    void CloseDoor()
    {
        audioSource.clip = closeDoorAudio;
        audioSource.Play();
        doorClosed.SetActive(true);
        doorOpened.SetActive(false);
        isOpened = false;
    }
    public IEnumerator OpenDoor(float seconds)
    {
        OpenDoor();
        passedTime = 0;
        while (passedTime < seconds)
        {
            passedTime += Time.deltaTime;
            yield return null;
        }
        passedTime = 0;
        CloseDoor();
    }
    public override void OnMonsterEnter(MonsterAI monster)
    {
        if (!isLocked)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            else
            {
                OpenDoor();
            }
        }
    }
    public override void OnMonsterExit(MonsterAI monster)
    {
        if (!isLocked)
            CloseDoor();
    }
    public void Load(int i, bool locked)
    {
        isLocked = locked;
        index = i;
        onInteract += delegate
        {
            if (isLocked)
            {
                audioSource.clip = lockedDoorAudio;
                audioSource.Play();
                Singleton<GameManager>.Instance.interactionPopUp.PopUp("The door is locked.");
            }
            else
            {
                if (!isOpened)
                    coroutine = StartCoroutine(OpenDoor(3));
            }
        };
    }
}