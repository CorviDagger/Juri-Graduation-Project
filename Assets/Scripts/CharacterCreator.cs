using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreator : MonoBehaviour
{
    //Character Body parts and Sprites can be added in the Editor
    [Header("Character Customization")]
    public SpriteRenderer bodyPart;
    public List<Sprite> options = new List<Sprite>();
    private int currentOption = 0;

    [Header("Item Selection")]
    public SpriteRenderer displayItemSlot;
    public List<InventoryManager.InventoryItem> ownedItems = new List<InventoryManager.InventoryItem>();
    private int currentItem = 0;

    //Function to put on button, selects next Sprite for the corresponding bodypart
    public void NextOption()
    {
        currentOption++;
        if(currentOption >= options.Count)
        {
            currentOption = 0;
        }
        bodyPart.sprite = options[currentOption];
    }

    //Function to put on button, selects next item to be displayed next to users Avatar
    public void NextDisplayItem()
    {
        if(ownedItems.Count == 0)
        {
            displayItemSlot.sprite = null;
            return;
        }
        currentItem = (currentItem + 1) % ownedItems.Count;
        UpdateDisplayItem();
    }

    //Sets up the list of items used for the item selection logic using the inventory
    //Also sets up option for no item to be selected
    public void SetInventoryItem(List<InventoryManager.InventoryItem> itemSprites)
    {
        ownedItems = new List<InventoryManager.InventoryItem>();
        ownedItems.Add(new InventoryManager.InventoryItem("None", null));
        ownedItems.AddRange(itemSprites);
        currentItem = 0;
        UpdateDisplayItem();
    }

    private void Start()
    {
        SetInventoryItem(InventoryManager.Instance.inventoryItems);
    }

    //Updates the Sprite of the item to the item selected by the button
    private void UpdateDisplayItem()
    {
        if (ownedItems.Count == 0)
        {
            displayItemSlot.sprite = null;
            return;
        }
            displayItemSlot.sprite = ownedItems[currentItem].icon;
    }

}
