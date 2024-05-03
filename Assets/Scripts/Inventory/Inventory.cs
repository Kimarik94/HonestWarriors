using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System;

public class Inventory : MonoBehaviour
{
    [SerializeReference] public List<ItemSlotInfo> _items = new List<ItemSlotInfo>();

    [Space]
    [Header("Inventory Menu Components")]
    public GameObject _inventoryMenu;
    public GameObject _itemPanel;
    public GameObject _itemPanelGrid;

    public MouseInventory _mouse;

    private List<ItemPanel> _existingPanels = new List<ItemPanel>();

    Dictionary<string, Item> allItemsDictionary = new Dictionary<string, Item>();

    [Space]
    public int _inventorySize = 30;

    public bool _isInventoryOpen = false;

    private void Start()
    {
        for (int i = 0; i < _inventorySize; i++)
        {
            _items.Add(new ItemSlotInfo(null, 0));
        }

        List<Item> allItems = GetAllItems().ToList();
        string itemsInDictionary = "Items in Dictionary: ";

        foreach(Item item in allItems)
        {
            if (!allItemsDictionary.ContainsKey(item.GiveName()))
            {
                allItemsDictionary.Add(item.GiveName(), item);
                itemsInDictionary += ", " + item.GiveName();
            }
            else
            {
                Debug.Log("" + item + " already exists in Dictionary - shares name with " + allItemsDictionary[item.GiveName()]);
            }
        }

        _inventoryMenu.SetActive(false);

        itemsInDictionary += ".";
        Debug.Log(itemsInDictionary);       
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !Gamebahaivour._isPaused)    
        {
            if (_inventoryMenu.activeSelf)
            {
                _isInventoryOpen = false;
                _inventoryMenu.SetActive(false);
                _mouse.EmptySlot();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                _isInventoryOpen = true;
                _inventoryMenu.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                RefreshInventory();
            }
        }
        if(Input.GetKeyDown(KeyCode.Mouse1) && _mouse._itemSlot._item != null)
        {
            RefreshInventory();
        }
    }

    public void RefreshInventory()
    {
        _existingPanels = _itemPanelGrid.GetComponentsInChildren<ItemPanel>().ToList();
        //Create Panels if needed;

        if(_existingPanels.Count < _inventorySize)
        {
            int amountToCreate = _inventorySize - _existingPanels.Count;
            for(int i = 0; i < amountToCreate; i++)
            {
                GameObject newPanel = Instantiate(_itemPanel, _itemPanelGrid.transform);
                _existingPanels.Add(newPanel.GetComponent<ItemPanel>());
            }
        }

        int index = 0;
        foreach(ItemSlotInfo i in _items)
        {
            //Name our List Elements
            i._name = "" + (index + 1);
            if (i._item != null) i._name += ": " + i._item.GiveName();
            else i._name += ": - ";

            //Update our Panels
            ItemPanel panel = _existingPanels[index];
            panel.name = i._name + "Panel";
            if(panel != null)
            {
                panel._inventory = this;
                panel._itemSlot = i;
                if(i._item != null)
                {
                    panel._itemImage.gameObject.SetActive(true);
                    panel._itemImage.sprite = i._item.GiveItemImage();
                    panel._itemImage.CrossFadeAlpha(1f, 0.05f, true);
                    panel._stacksText.gameObject.SetActive(true);
                    panel._stacksText.text = "" + i._stacks;
                }
                else
                {
                    panel._itemImage.gameObject.SetActive(false);
                    panel._stacksText.gameObject.SetActive(false);
                }
            }
            index++;
            _mouse.EmptySlot();
        }
    }

    public int AddItem(string itemName, int amount)
    {
        //Find Item to add
        Item item = null;
        allItemsDictionary.TryGetValue(itemName, out item);

        //Exit method if no Item was found
        if(item == null)
        {
            Debug.Log("Could not find Item in Dictionary to add to Inventory");
            return amount;
        }

        foreach(ItemSlotInfo i in _items)
        {
            if(i._item != null)
            {
                if(i._item.GiveName() == item.GiveName())
                {
                    if(amount > i._item.MaxStacks() - i._stacks)
                    {
                        amount -= i._item.MaxStacks() - i._stacks;
                        i._stacks = i._item.MaxStacks();
                    }
                    else
                    {
                        i._stacks += amount;
                        if (_inventoryMenu.activeSelf) RefreshInventory();
                        return 0;
                    }
                }
            }
        }

        foreach(ItemSlotInfo i in _items)
        {
            if(i._item == null)
            {
                if(amount > item.MaxStacks())
                {
                    i._item = item;
                    i._stacks = item.MaxStacks();
                    amount -= item.MaxStacks();
                }
                else
                {
                    i._item = item;
                    i._stacks = amount;
                    if (_inventoryMenu.activeSelf) RefreshInventory();
                    return 0;
                }
            }
        }

        Debug.Log("No space in Inventory for: " + item.GiveName());
        if (_inventoryMenu.activeSelf) RefreshInventory();
        return amount;
    }

    public void ClearSlot(ItemSlotInfo slot)
    {
        slot._item = null;
        slot._stacks = 0;
    }

    IEnumerable<Item> GetAllItems()
    {
        return System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsSubclassOf(typeof(Item)))
            .Select(type => System.Activator.CreateInstance(type) as Item);
    }
}
