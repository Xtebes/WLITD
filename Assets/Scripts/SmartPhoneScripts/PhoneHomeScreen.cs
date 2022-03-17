using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PhoneHomeScreen : MonoBehaviour
{
    [SerializeField]
    private Transform itemListParent;
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private TextMeshProUGUI itemName, itemDescription;
    [SerializeField]
    private GameObject popUp, recipeList;
    [SerializeField]
    private SmartPhoneController smartPhone;
    [SerializeField]
    private Button[] popUpButtons;
    void CheckItem(Item item)
    {
        smartPhone.SetHorientation(SmartPhoneController.Orientation.horizontal);
        itemName.text = item.name;
        itemImage.sprite = Singleton<References>.Instance.itemImages[(int)Inventory.itemTypeToIndex[item.GetType()]];
        itemDescription.text = item.description;
        smartPhone.SetActiveScreen(SmartPhoneController.SmartPhoneScreen.inspector);
    }
    void UpdateCraftingList(GameObject pressedIcon, Inventory inventory, Item item)
    {

    }
    private void OnEnable()
    {
        popUp.SetActive(false);
    }
    void ChangePopUp(GameObject pressedIcon, Inventory inventory, Item item)
    {
        popUp.SetActive(true);
        popUp.transform.position = new Vector2(pressedIcon.transform.position.x, pressedIcon.transform.position.y + 20);
        foreach (Button button in popUpButtons)
        {
            button.onClick.RemoveAllListeners();
        }
        if (item.useAction != null)
        {
            popUpButtons[0].gameObject.SetActive(true);
            popUpButtons[0].onClick.AddListener(delegate { item.Action(); EnableInventory(inventory); });
        }
        else
        {
            popUpButtons[0].gameObject.SetActive(false);
        }
        popUpButtons[1].onClick.AddListener(delegate { CheckItem(item); });
        if (inventory.CanCraft(item))
        {
            popUpButtons[2].gameObject.SetActive(true);
            popUpButtons[2].onClick.AddListener(delegate { inventory.Craft(item); EnableInventory(inventory); });
        }
        else
            popUpButtons[2].gameObject.SetActive(false);
    }
    public void EnableInventory(Inventory inventory)
    {
        popUp.SetActive(false);
        recipeList.SetActive(false);
        gameObject.SetActive(true);
        itemListParent.DestroyAllFirstLevelChildren();
        foreach (KeyValuePair<Type, int> pair in inventory.items)
        {
            var item = (Item)Activator.CreateInstance(pair.Key);
            if (pair.Value > 0 || inventory.CanCraft(item))
            {
                var itemUI = Instantiate(Singleton<References>.Instance.uiPrefabs[0], itemListParent);
                var itemUIBuilder = itemUI.GetComponent<ItemBuilder>();
                itemUIBuilder.iconButton.onClick.AddListener(delegate
                {
                    ChangePopUp(itemUI, inventory, item);
                    UpdateCraftingList(itemUI, inventory, item);
                });
                itemUIBuilder.BuildItemUI(inventory, item, pair.Value); 
            }
        }
    }
}