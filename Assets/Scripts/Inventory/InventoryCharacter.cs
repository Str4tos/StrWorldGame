using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryCharacter : InventoryCustom
{

    private List<ItemEquipedData> itemsEquipedData;
    private ItemEquipType[] itemEquipTypes;
    private StatusPlayer _StatusPlayer;

    protected override void Start()
    {
        typeParentInv = TypeParentInv.equip;
        _StatusPlayer = GameObject.FindWithTag("Player").GetComponent<StatusPlayer>();

        itemEquipTypes = new ItemEquipType[]{
            ItemEquipType.Agation, ItemEquipType.Head, ItemEquipType.Wings, 
            ItemEquipType.WeaponRightHend, ItemEquipType.Chest, ItemEquipType.WeaponLeftHend,
            ItemEquipType.Hands, ItemEquipType.Pants, ItemEquipType.Shoes,
            ItemEquipType.RingRight, ItemEquipType.Necklace, ItemEquipType.RingLeft
        };
        itemsEquipedData = new List<ItemEquipedData>(12);
        for (int i = 0; i < itemEquipTypes.Length; i++)
        {
            itemsEquipedData.Add(new ItemEquipedData(itemEquipTypes[i], new ItemEquip()));
        }

        base.Start();
    }

    public override void OpenInventory()
    {
        base.OpenInventory();
        _StatusPlayer.RefreshStatsInInventory(this.gameObject.transform.Find("Stats"));
    }

    public ItemEquip Equip(ItemEquip equipItem)
    {
        ItemEquipType tempType = equipItem.itemEquipType;
        ItemEquip itemForReturn = null;
        if (tempType == ItemEquipType.Ring)
        {
            if (!GetItemEquipData(ItemEquipType.RingLeft).IsEquiped)
                tempType = ItemEquipType.RingLeft;
            else tempType = ItemEquipType.RingRight;
        }

        ItemEquipedData _ItemEquipedData = GetItemEquipData(tempType);
        if (_ItemEquipedData.IsEquiped)
        {
            Destroy(slotContainer.transform.GetChild(_ItemEquipedData.Item.indexItemInList).GetChild(0).gameObject);
            itemForReturn = _ItemEquipedData.Item;
            itemForReturn.indexItemInList = equipItem.indexItemInList;
        }
        ItemEquip newItem = equipItem.getCopy();
        newItem.indexItemInList = GetIndexItemInList(tempType);
        _ItemEquipedData.IsEquiped = true;
        _ItemEquipedData.Item = newItem;

		AddItemInObj(newItem, newItem.indexItemInList);
        return itemForReturn;
    }

    public void UnEquip(ItemCustom item)
    {
        ItemEquipedData _ItemEquipedData = itemsEquipedData[item.indexItemInList];
        Destroy(slotContainer.transform.GetChild(item.indexItemInList).GetChild(0).gameObject);
        _ItemEquipedData.UnEquiped();
    }

    public override ItemEquip GetEquipItem(ItemCustom item)
    {
        return itemsEquipedData[item.indexItemInList].Item;
    }

    public ItemEquip GetEquipItem(ItemEquipType equipType)
    {
        if (equipType == ItemEquipType.Ring)
        {
            if (GetItemEquipData(ItemEquipType.RingLeft).IsEquiped)
                equipType = ItemEquipType.RingLeft;
            else equipType = ItemEquipType.RingRight;
        }
        return GetItemEquipData(equipType).Item;
    }

    public InventoryCharacterData GetInventoryData()
    {
        InventoryCharacterData result = new InventoryCharacterData();
        result.itemsEquipedData = itemsEquipedData;
        return result;
    }

    public void LoadSerialization(InventoryCharacterData loadedInventory)
    {
        itemsEquipedData = loadedInventory.itemsEquipedData;
        foreach (ItemEquipedData itemEquipedData in itemsEquipedData)
        {
			if(!itemEquipedData.Item.IsEmpty())
            	AddItemInObj(itemEquipedData.Item, itemEquipedData.Item.indexItemInList);
        }
    }

    private int GetIndexItemInList(ItemEquipType tempType)
    {
        for (int i = 0; i < itemEquipTypes.Length; i++)
        {
            if (itemEquipTypes[i] == tempType)
                return i;
        }
        return -1;
    }

    private ItemEquipedData GetItemEquipData(ItemEquipType equipType)
    {
        for (int i = 0; i < itemsEquipedData.Count; i++)
            if (itemsEquipedData[i].Type == equipType)
                return itemsEquipedData[i];
        return null;
    }


}
