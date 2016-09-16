using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemConsume : Item
{

    public int numberTicks;
    public float delayTick;
    public int maxInStack;

    public List<AttributeItem> itemAttributes = new List<AttributeItem>();

    public ItemConsume() { }
    public ItemConsume(int id, string name)
    {
        this.id = id;
        this.name = name;
        this.itemType = ItemType.Consume;
        this.quantity = 0;
    }
    public ItemConsume getCopy()
    {
        return (ItemConsume)this.MemberwiseClone();
    }
}

