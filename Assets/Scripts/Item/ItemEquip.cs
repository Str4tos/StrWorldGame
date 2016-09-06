using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemEquip : Item {
    public ItemEquipType itemEquipType;
    public int requestLevel;
    public int requestStrenght;
    public int requestAgility;
    public int requestVitality;
    public int requestEnergy;

    public List<ItemAttribute> itemAttributes = new List<ItemAttribute>();
	
}
