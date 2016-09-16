using UnityEngine;
using System.Collections;

public class ItemTypesData {

    public int id;
    public ItemType itemType = ItemType.none;

    public ItemTypesData() { }
    public ItemTypesData(int id, ItemType itemType)
    {
        this.id = id;
        this.itemType = itemType;
    }

}
