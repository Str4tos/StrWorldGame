using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ItemEquip : ItemCustom
{
    public ItemEquipType itemEquipType;
    public int requestLevel;
    public int requestStrenght;
    public int requestAgility;
    public int requestVitality;
    public int requestEnergy;
    public int enchantLevel;

    public List<ItemAttribute> itemAttributes = new List<ItemAttribute>();

    public ItemEquip(){}

    public ItemEquip(int id, string name)
    {
        this.id = id;
        this.name = name;
        this.itemType = ItemTypeCustom.Equip;
		this.enchantLevel = 0;
        this.quantity = 1;
    }

    public ItemEquip getCopy()
    {
        return (ItemEquip)this.MemberwiseClone();
    }
}
