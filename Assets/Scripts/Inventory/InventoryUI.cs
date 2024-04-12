using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform _itemsParent;
    public GameObject _inventoryUI;

    Inventory _inventory;

    InventorySlot[] _slots;

    public bool _isInventoryOpen = false;

    void Start()
    {
        _inventory = Inventory.instance;
        _inventory.onItemChangedCallback += UpdateUI;
        _slots = _itemsParent.GetComponentsInChildren<InventorySlot>();

        _inventoryUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
            if (_isInventoryOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (!_isInventoryOpen)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
            }
        }
    }

    void ToggleInventory()
    {
        _isInventoryOpen = !_isInventoryOpen;
        _inventoryUI.SetActive(_isInventoryOpen);
    }

    void UpdateUI()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (i < _inventory._items.Count)
            {
                _slots[i].AddItem(_inventory._items[i]);
            }
            else
            {
                _slots[i].ClearSlot();
            }
        }
    }
}
