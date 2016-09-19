using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BagInventory : Inventory
{


    private int CashValue = 0;
    private Text LabelCash;

    protected override void Start()
    {
        Initiate();
        base.Start();
        typeParentInv = TypeParentInv.bag;
        LabelCash = transform.FindChild("CashCanvas/LabelCash").GetComponent<Text>();

    }

    void Update()
    {

    }


    public void UpdateCash(int Cash)
    {
        CashValue = Cash;
        UpdateCash();
    }
    public void UpdateCash()
    {
        LabelCash.text = CashValue.ToString();
    }
    public int GetCashValue()
    {
        return CashValue;
    }

    public void LoadSerialization(InventoryBagData loadedInventory)
    {
        LoadSerialization(loadedInventory as InventoryData);
        UpdateCash(loadedInventory.CashValue);
    }

    public InventoryBagData GetInventoryData()
    {
        InventoryBagData result = new InventoryBagData();
        foreach (Item itemInInv in itemsInInv)
            switch (itemInInv.itemType)
            {
                case ItemType.Equip:
                    result.itemsEquipInInv.Add(itemInInv as ItemEquip);
                    break;
                case ItemType.Consume:
                    result.itemsConsumeInInv.Add(itemInInv as ItemConsume);
                    break;
                case ItemType.Other:
                    result.itemsOtherInInv.Add(itemInInv as ItemOther);
                    break;
            }
        result.CashValue = CashValue;
        return result;
    }



    // Only for tests. Delete soon
    public void AddTestItem(int id)
    {
        AddItemToInventory(1, 5);
        AddItemToInventory(4, 1);
    }
}
