using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemPanel : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IDropHandler
{
    public Inventory _inventory;
    private MouseInventory _mouse;
    public ItemSlotInfo _itemSlot;
    public Image _itemImage;
    public TextMeshProUGUI _stacksText;

    private bool click;

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventData.pointerPress = this.gameObject;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        click = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (click)
        {
            OnClick();
            click = false;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        OnClick();
        click = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (click)
        {
            OnClick();
            click = false;
        }
    }

    public void PickUpItem()
    {
        _mouse._itemSlot = _itemSlot;
        _mouse._sourceItemPanel = this;
        if (Input.GetKey(KeyCode.LeftShift) && _itemSlot._stacks > 1) _mouse._splitSize = _itemSlot._stacks / 2;
        else _mouse._splitSize = _itemSlot._stacks;
        _mouse.SetUI();
    }

    public void FadeOut()
    {
        _itemImage.CrossFadeAlpha(0.3f, 0.05f, true);
    }

    public void DropItem()
    {
        _itemSlot._item = _mouse._itemSlot._item;
        if(_mouse._splitSize < _mouse._itemSlot._stacks)
        {
            _itemSlot._stacks = _mouse._splitSize;
            _mouse._itemSlot._stacks -= _mouse._splitSize;
            _mouse.EmptySlot();
        }
        else
        {
            _itemSlot._stacks = _mouse._itemSlot._stacks;
            _inventory.ClearSlot(_mouse._itemSlot);
        }
    }

    public void SwapItem(ItemSlotInfo itemSlotA, ItemSlotInfo itemSlotB)
    {
        ItemSlotInfo tempItem = new ItemSlotInfo(itemSlotA._item, itemSlotA._stacks);

        itemSlotA._item = itemSlotB._item;
        itemSlotA._stacks = itemSlotB._stacks;

        itemSlotB._item = tempItem._item;
        itemSlotB._stacks = tempItem._stacks;
    }

    public void StackItem(ItemSlotInfo source, ItemSlotInfo destination, int amount)
    {
        int slotsAvaliable = destination._item.MaxStacks() - destination._stacks;
        if (slotsAvaliable == 0) return;
        if(amount > slotsAvaliable)
        {
            source._stacks -= slotsAvaliable;
            destination._stacks = destination._item.MaxStacks();
        }
        if(amount <= slotsAvaliable)
        {
            destination._stacks += amount;
            if (source._stacks == amount) _inventory.ClearSlot(source);
            else source._stacks -= amount;
        }
    }

    public void OnClick()
    {
        if(_inventory != null)
        {
            _mouse = _inventory._mouse;

            //Grab item if mouse slot is Empty
            if (_mouse._itemSlot._item == null)
            {
                if(_itemSlot._item != null)
                {
                    PickUpItem();
                    FadeOut();
                }
            }
            else
            {
                //Clicked on original slot
                if(_itemSlot == _mouse._itemSlot)
                {
                    _inventory.RefreshInventory();
                }

                //Clicked on an Empty Slot
                else if(_itemSlot._item == null)
                {
                    DropItem();
                    _inventory.RefreshInventory();
                }

                //Clicked on occupied slot of different item type

                else if(_itemSlot._item.GiveName() != _mouse._itemSlot._item.GiveName())
                {
                    SwapItem(_itemSlot, _mouse._itemSlot);
                    _inventory.RefreshInventory();
                }

                //Clicked on occupided slot of a same type
                else if (_itemSlot._stacks < _itemSlot._item.MaxStacks())
                {
                    StackItem(_mouse._itemSlot, _itemSlot, _mouse._splitSize);
                    _inventory.RefreshInventory();
                }
            }
        }
    }
}
