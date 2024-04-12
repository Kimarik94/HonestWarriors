using UnityEngine;

public class ItemPickup : Interactable
{
    public Item _item;

    public override void Interact()
    {
        base.Interact();

        if(base._isFocus) PickUp();
    }

    void PickUp()
    {
        Debug.Log("Picking up " + _item.name);
        bool wasPickedUp = Inventory.instance.Add(_item);

        if (wasPickedUp)
            Destroy(gameObject);
    }
}
