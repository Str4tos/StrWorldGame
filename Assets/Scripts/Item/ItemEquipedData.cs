using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemEquipedData
{
    public ItemEquipType Type;
    public ItemEquip Item;
    public bool IsEquiped = false;

    public ItemEquipedData() { }
    public ItemEquipedData(ItemEquipType type, ItemEquip item)
    {
        this.Type = type;
        this.Item = item;
    }

    public void UnEquiped()
    {
        IsEquiped = false;
        Item = new ItemEquip();
    }
}

