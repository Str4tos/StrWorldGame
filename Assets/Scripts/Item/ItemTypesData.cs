using UnityEngine;
using System.Collections;

public class ItemTypesData {

    public int id;
    public ItemTypeCustom itemType = ItemTypeCustom.none;

    public ItemTypesData() { }
    public ItemTypesData(int id, ItemTypeCustom itemType)
    {
        this.id = id;
        this.itemType = itemType;
    }

}
