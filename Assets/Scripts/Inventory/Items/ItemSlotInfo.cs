[System.Serializable]
public class ItemSlotInfo
{
    public Item _item;
    public string _name;
    public int _stacks;

    public ItemSlotInfo(Item newItem, int newStacks)
    {
        _item = newItem;
        _stacks = newStacks;
    }
}