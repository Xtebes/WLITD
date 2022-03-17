using UnityEngine;
public class MessageSender : Interactable
{
    [HideInInspector]
    public int index;
    [SerializeField]
    bool senderIsMe;
    [SerializeField]
    private MessageScreen.contacts contact;
    [SerializeField]
    private string messageText;
    public override void OnPlayerEnter(Player player)
    {
        MessageScreen.SendMessage(contact, senderIsMe, messageText);
        Destroy(gameObject);
        SavedInfo.save.isMessageTriggered[index] = true;
    }
}
