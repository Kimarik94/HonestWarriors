using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public string _itemToDrop;
    public int _amount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Inventory playerInventory = GameObject.Find("GUI").GetComponent<Inventory>();

            if(playerInventory != null) PickUpItem(playerInventory);
        }
    }

    public void PickUpItem(Inventory inventory)
    {
        _amount = inventory.AddItem(_itemToDrop, _amount);

        if(_amount < 1) Destroy(this.gameObject);
    }
}
