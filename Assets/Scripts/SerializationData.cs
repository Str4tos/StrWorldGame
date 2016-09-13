using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryData {
    public List<ItemEquip> itemsEquipInInv;
    public List<ItemConsume> itemsConsumeInInv;
    public List<ItemOther> itemsOtherInInv;
}

public class InventoryBagData : InventoryData
{
    public int CashValue;
}

public class InventoryCharacterData
{
    public List<ItemEquipedData> itemsEquipedData;
}

public class StatusPlayerData
{
    public string personalName;
    public string locationName;

    public int level;
    public int strenght;
    public int agility;
    public int vitality;
    public int energy;

    public float rangeAttack;

    public int statPoints;
    public float currExp;
    public float currExpToLevelUp = 37;

}