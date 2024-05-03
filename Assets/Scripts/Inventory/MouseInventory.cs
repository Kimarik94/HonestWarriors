using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseInventory : MonoBehaviour
{
    public GameObject _mouseItemUI;
    public Image _mouseCursor;
    public ItemSlotInfo _itemSlot;
    public Image _itemImage;
    public TextMeshProUGUI _stacksText;
    public Vector3 _mouseCursorOffset = new Vector3(20f, -5f, 0f);

    public ItemPanel _sourceItemPanel;
    public int _splitSize;
    
    void Update()
    {
        transform.position = Input.mousePosition + _mouseCursorOffset;
        if(Cursor.lockState == CursorLockMode.Locked)
        {
            _mouseCursor.enabled = false;
            _mouseItemUI.SetActive(false);
        }
        else
        {
            _mouseCursor.enabled = true;

            if(_itemSlot._item != null)
            {
                _mouseItemUI.SetActive(true);
            }
            else
            {
                _mouseItemUI.SetActive(false);
            }
        }
        if(_itemSlot._item != null)
        {
            if(Input.GetAxis("Mouse ScrollWheel") > 0 && _splitSize < _itemSlot._stacks)
            {
                _splitSize++;
            }
            if(Input.GetAxis("Mouse ScrollWheel") < 0 && _splitSize > 1)
            {
                _splitSize--;
            }
            _stacksText.text = "" + _splitSize;
            if (_splitSize == _itemSlot._stacks) _sourceItemPanel._stacksText.gameObject.SetActive(false);
            else
            {
                _sourceItemPanel._stacksText.gameObject.SetActive(true);
                _sourceItemPanel._stacksText.text = "" + (_itemSlot._stacks - _splitSize);
            }
        }
    }

    public void SetUI()
    {
        _stacksText.text = "" + _splitSize;
        _itemImage.sprite = _itemSlot._item.GiveItemImage();
    }

    public void EmptySlot()
    {
        _itemSlot = new ItemSlotInfo(null, 0);
    }
}
