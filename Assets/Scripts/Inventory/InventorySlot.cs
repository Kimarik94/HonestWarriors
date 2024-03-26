﻿using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{

    public Image icon;
    public Button removeButton; 

    Item item;

    private void Start()
    {
        removeButton.image.color = new Color(0f, 0f , 0f, 0f);
    }

    public void AddItem(Item newItem)
    {
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
        removeButton.interactable = true;
        removeButton.image.color = new Color(255f, 255f, 255f, 255f);

    }

    public void ClearSlot()
    {
        item = null;

        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
        removeButton.image.color = new Color(0f, 0f, 0f, 0f);
    }

    public void OnRemoveButton()
    {
        Inventory.instance.Remove(item);
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
        }
    }
}