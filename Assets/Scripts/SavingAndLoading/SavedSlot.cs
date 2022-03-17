using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SavedSlot : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI currentRoomText, timePlayedText; public TextMeshProUGUI slotNumber;
    public Button saveButton, loadButton, deleteButton;
    [SerializeField]
    Image banner;
    public void LoadSlot(GameDetails details)
    {
        currentRoomText.text = details.currentRoomName;
    }
}
