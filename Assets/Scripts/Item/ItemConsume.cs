using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemConsume : ItemCustom
{

    public int numberTicks;
    public float delayTick;
    public int maxInStack;

    public List<ItemAttribute> itemAttributes = new List<ItemAttribute>();

    public ItemConsume() { }
    public ItemConsume(int id, string name)
    {
        this.id = id;
        this.name = name;
        this.itemType = ItemTypeCustom.Consume;
        this.quantity = 0;
    }
    public ItemConsume getCopy()
    {
        return (ItemConsume)this.MemberwiseClone();
    }
}

