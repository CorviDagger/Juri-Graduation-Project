using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private HashSet<string> unlockedItems = new HashSet<string>();
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();

    public CharacterCreator characterCreator;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            unlockedItems.Clear();
            inventoryItems.Clear();
        }
        else Destroy(gameObject);
    }

    //Adds an item to the inventory
    public void UnlockItem(string itemName)
    {
        if (!unlockedItems.Contains(itemName))
        {
            unlockedItems.Add(itemName);
            Sprite itemIcon = GetIconForItem(itemName);
            inventoryItems.Add(new InventoryItem(itemName, itemIcon));
            Debug.Log($"Unlocked item: {itemName}");
        }
        if(characterCreator != null)
        {
            characterCreator.SetInventoryItem(inventoryItems);
        }
    }

    public bool HasItem(string itemName)
    {
        return unlockedItems.Contains(itemName);
    }

    //Finds correct sprite for the item
    private Sprite GetIconForItem(string itemName)
    {
        foreach (var entry in itemSprites)
        {
            if (entry.itemName == itemName)
                return entry.icon;
        }
        return null;
    }

    //This is used by the Inventory UI
    [System.Serializable]
    public class InventoryItem
    {
        public string itemName;
        public Sprite icon;

        public InventoryItem(string name, Sprite icon = null)
        {
            itemName = name;
            this.icon = icon;
        }
    }

    //This is where you make the items in the editor
    [System.Serializable]
    public class ItemSpriteEntry
    {
        public string itemName;
        public Sprite icon;
    }
    public List<ItemSpriteEntry> itemSprites = new List<ItemSpriteEntry>();

}
