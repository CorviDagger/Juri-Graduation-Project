using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUi : MonoBehaviour
{

    public GameObject itemTemplate;
    public Transform gridParent;

    //Updates the content of inventory when the panel is bein activated
    private void OnEnable()
    {
        RefreshInventory();
    }

    //Creates another Gamobject with the correct item 
    public void RefreshInventory()
    {
        foreach (Transform child in gridParent)
        {
            if (child != itemTemplate.transform)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (var item in InventoryManager.Instance.inventoryItems)
        {
            GameObject newItem = Instantiate(itemTemplate, gridParent);
            newItem.SetActive(true);
            Image iconImage = newItem.GetComponent<Image>();
            if (item.icon != null)
            {
                iconImage.sprite = item.icon;
            }
        }
    }
}
