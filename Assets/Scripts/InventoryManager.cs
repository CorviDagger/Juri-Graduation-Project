using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private HashSet<string> unlockedItems = new HashSet<string>();
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UnlockItem(string itemName)
    {
        if (!unlockedItems.Contains(itemName)) ;
        {
            unlockedItems.Add(itemName);
            inventoryItems.Add(new InventoryItem(itemName));
            Debug.Log($"Unlocked new item: {itemName}");
            //TODO UI stuff
        }
    }

    public bool HasItem(string itemName)
    {
        return unlockedItems.Contains(itemName);
    }



    [System.Serializable]
    public class InventoryItem
    {
        public string itemName;
        public Sprite icon;

        public InventoryItem(string name)
        {
            itemName = name;
        }

    }
}
