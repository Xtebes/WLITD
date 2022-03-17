using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ItemBuilder : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI itemName, itemAmount;
    [SerializeField]
    private Image itemIcon, itemNote;
    public Button iconButton;
    public void BuildItemUI(Inventory inventory, Item item, int amount)
    {
        itemName.text = item.name;
        itemAmount.text = amount.ToString();
        itemIcon.sprite = Singleton<References>.Instance.itemIcons[(int)Inventory.itemTypeToIndex[item.GetType()]];
        if (!inventory.CanCraft(item))
        {
            itemNote.gameObject.SetActive(false);
        }
    }
}