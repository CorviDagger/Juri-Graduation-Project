using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    public InventoryManager.InventoryItem item;
    public Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    public void SetItem(InventoryManager.InventoryItem newItem)
    {
        item = newItem;
        Image iconImage = GetComponent<Image>();
        if (item.icon != null)
        {
            iconImage.sprite = item.icon;
        }
        else
        {
            iconImage.sprite = null;
        }
    }

    private void OnClick()
    {
        CharacterCreator characterCreator = FindFirstObjectByType<CharacterCreator>();
        if (characterCreator != null)
        {
            characterCreator.SelectItem(item);
        }
    }

}
