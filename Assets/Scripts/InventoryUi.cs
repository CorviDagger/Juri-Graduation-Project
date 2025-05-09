using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUi : MonoBehaviour
{

    public GameObject itemTemplate;
    public Transform gridParent;
    // Start is called before the first frame update
    private void OnEnable()
    {
        RefreshInventory();
    }

    public void RefreshInventory()
    {
        foreach (Transform child in gridParent)
        {
            if (child != itemTemplate.transform)
                Destroy(child.gameObject);
        }

        foreach (var item in InventoryManager.Instance.inventoryItems)
        {
            GameObject newItem = Instantiate(itemTemplate, gridParent);
            newItem.SetActive(true);

            Image iconImage = newItem.GetComponent<Image>();
            if (item.icon != null) iconImage.sprite = item.icon;
        }
    }
}
