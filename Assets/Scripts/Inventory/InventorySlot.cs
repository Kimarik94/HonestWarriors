using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image _icon;
    public Button _removeButton; 

    Item _item;

    private void Start()
    {
        _removeButton.image.color = new Color(0f, 0f , 0f, 0f);
    }

    public void AddItem(Item newItem)
    {
        _item = newItem;

        _icon.sprite = _item._icon;
        _icon.enabled = true;
        _removeButton.interactable = true;
        _removeButton.image.color = new Color(255f, 255f, 255f, 255f);

    }

    public void ClearSlot()
    {
        _item = null;

        _icon.sprite = null;
        _icon.enabled = false;
        _removeButton.interactable = false;
        _removeButton.image.color = new Color(0f, 0f, 0f, 0f);
    }

    public void OnRemoveButton()
    {
        Inventory.instance.Remove(_item);
    }

    public void UseItem()
    {
        if (_item != null)
        {
            _item.Use();
        }
    }
}
