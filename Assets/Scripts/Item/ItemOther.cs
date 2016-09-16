using UnityEngine;
using System.Collections;

public class ItemOther : Item
{
    public ItemOtherType itemOtherType;

    public ItemOther() { }
    public ItemOther(int id, string name)
    {
        this.id = id;
        this.name = name;
        this.itemType = ItemType.Other;
        this.quantity = 0;
    }

    public ItemOther getCopy()
    {
        return (ItemOther)this.MemberwiseClone();
    }
}
