using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MessageLog
{
    public MessageScreen.contacts contact;
    public List<bool> senderIsMe = new List<bool>();
    public List<string> texts = new List<string>();
}
public class MessageScreen : MonoBehaviour
{
    private void Start()
    {
        Button[] iconButtons = iconHolder.GetComponentsInChildren<Button>();
        for (int i = 0; i < iconButtons.Length; i++)
        {
            int index = i;
            iconButtons[index].onClick.AddListener(delegate 
            {
                LoadContactsChat((contacts)index);
            });
        }
    }
    public enum contacts
    {
        Shinji = 0,
        Asuka = 1,
        Hirano = 2,
    }
    public static void SendMessage(contacts contact, bool senderIsMe, string message)
    {
        MessageLog log = SavedInfo.save.logs.First(i => i.contact == contact);
        if (log == null)
        {
            log = new MessageLog();
            log.contact = contact;
            SavedInfo.save.logs.Add(log);
        }
        log.senderIsMe.Add(senderIsMe);
        log.texts.Add(message);
    }
    public void LoadContactsChat(contacts contact)
    {
        MessageLog log = null;
        try
        {
            log = SavedInfo.save.logs.First(i => i.contact == contact);
        }
        catch
        {

        }
        chat.SetActive(true);
        messageHolder.transform.DestroyAllFirstLevelChildren();
        contactNameHolder.text = Enum.GetName(typeof(contacts),contact);
        if (log != null)
        {
            for (int i = 0; i < log.texts.Count; i++)
            {
                TextMeshProUGUI message = Instantiate(messagePrefab, messageHolder.transform).GetComponentInChildren<TextMeshProUGUI>();
                Image bubble = message.transform.parent.GetComponent<Image>();
                if (log.senderIsMe[i])
                {
                    bubble.transform.localEulerAngles = new Vector3(0,180);
                    message.transform.localEulerAngles = new Vector3(0, 180);
                    bubble.color = Color.white / 2;
                }
                message.text = log.texts[i];
            }
        }
    }
    private void OnEnable()
    {
        contactNameHolder.text = "";
        chat.SetActive(false);
    }
    [SerializeField]
    private TextMeshProUGUI contactNameHolder;
    [SerializeField]
    private GameObject chat, messageHolder, messagePrefab;
    [SerializeField]
    private GameObject iconHolder;
}