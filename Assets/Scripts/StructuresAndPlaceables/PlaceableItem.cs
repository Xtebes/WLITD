using System;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CircleCollider2D))]
public class PlaceableItem : Interactable
{
    [SerializeField]
    private AudioSource audioSource;
    public int amount;
    public Inventory.ItemType item;
    public void Load(Player player, int index)
    {
        audioSource = GetComponent<AudioSource>();
        Item itemInstance = (Item)Activator.CreateInstance(Inventory.itemIndexToType[item]);
        title.text = $"{itemInstance.name} [{amount}]";
        GetComponent<SpriteRenderer>().sprite = Singleton<References>.Instance.itemDropped[(int)item];
        onInteract += delegate 
        {
            audioSource.Play();
            player.input.worldInteractionAction = null; 
            player.input.worldInteraction.Disable();
            SavedInfo.save.playerInfo.inventory.items[Inventory.itemIndexToType[item]] += amount;
            SavedInfo.save.isItemCollected[index] = true;
            Destroy(gameObject);
        };
        info.text = "Press " + player.input.worldInteraction.GetBindingDisplayString() + " to pick up";
    }
}