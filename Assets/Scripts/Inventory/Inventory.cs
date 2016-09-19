using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

public class Inventory : MonoBehaviour
{

    protected GameObject prefabItem;

    //Items
    protected List<Item> itemsInInv;

    protected Transform slotContainer;
    protected bool isActive;

    public delegate void InventoryEvent();
    public event InventoryEvent InventoryInit;
    public event InventoryEvent InventoryOpen;

    public enum TypeParentInv { bag, equip, hotbar, storage, shop, craft };
    [HideInInspector]
    public TypeParentInv typeParentInv;

    public bool IsActive()
    {
        return isActive;
    }

    protected virtual void Start()
    {
        slotContainer = transform.FindChild("Slots");
        prefabItem = Resources.Load("Prefabs/Inventory/Item") as GameObject;
        isActive = false;
        gameObject.SetActive(false);

        if (InventoryInit != null)
            InventoryInit();
    }
    protected void Initiate()
    {
        itemsInInv = new List<Item>();
    }

    public virtual void OpenInventory()
    {
        gameObject.SetActive(true);
        isActive = true;
        //updateCashValue();
        //if (this.tag == "EquipmentSystem")
        //{
        //    _StatusPlayer.RefreshStatsInInventory(this.gameObject.transform.Find("Stats"));
        //}
        if (InventoryOpen != null)
        {

            InventoryOpen();
        }
    }

    public void CloseInventory()
    {
        gameObject.SetActive(false);
        isActive = false;
    }

    public void LoadSerialization(InventoryData loadedInventory)
    {
        foreach (ItemEquip itemEquip in loadedInventory.itemsEquipInInv)
        {
            itemsInInv.Add(itemEquip);
            AddItemInObj(itemEquip, itemEquip.indexItemInList);
        }
        foreach (ItemConsume itemConsume in loadedInventory.itemsConsumeInInv)
        {
            itemsInInv.Add(itemConsume);
            AddItemInObj(itemConsume, itemConsume.indexItemInList);
        }
        foreach (ItemOther itemOther in loadedInventory.itemsOtherInInv)
        {
            itemsInInv.Add(itemOther);
            AddItemInObj(itemOther, itemOther.indexItemInList);
        }
    }

    #region add/delete items

    /// <summary>
    /// Add item in inventory by id. Recomendet method AddItemToInventory(Item item)<br/>
    /// Returns: True - successfully | False - fail
    /// </summary>
    /// <param name="id">Item id</param>
    /// <param name="quantity">Quantity items</param>
    /// <returns></returns>
    public bool AddItemToInventory(int id, int quantity)
    {
        Item currItem = null;
        //Get copy from inventory
        foreach (Item itemInInv in itemsInInv)
            if (itemInInv.id == id)
            {
                currItem = itemInInv.GetCopy();
                break;
            }

        //else get item from xml storage
        if (currItem == null)
        {
            XmlStorageItem storageItem = new XmlStorageItem();
            currItem = storageItem.GetItemById(id);
        }
        currItem.quantity = quantity;
        return AddItemToInventory(currItem);
    }
    /// <summary>
    /// Add item to inventory. This recomendet method.<br/>
    /// Returns: True - successfully | False - fail
    /// </summary>
    /// <param name="item">ItemEquip/ItemConsume/ItemOther</param>
    /// <returns>True - successfully | False - fail</returns>
    public bool AddItemToInventory(Item item)
    {
        List<int> freeSlots;
        switch (item.itemType)
        {
            case ItemType.Equip:
                return AddEquipInInvExtension(item);

            case ItemType.Consume:
                return AddConsumInInvExtension(item);

            case ItemType.Other:
                return AddOtherInInvExtension(item);
        }
        return false;
    }
    /// <summary>
    /// Add equip item to inventory in indexSlot.<br/>
    /// Returns: True - successfully | False - slot not free
    /// </summary>
    /// <param name="item">ItemEquip/ItemConsume/ItemOther</param>
    /// <param name="indexSlot">Index slot in inventory</param>
    /// <returns></returns>
    public bool AddItemToInventory(Item item, int indexSlot)
    {
        return AddItemToInventory(item, indexSlot, false, true);
    }
    /// <summary>
    /// Add equip item to inventory in indexSlot./
    /// Returns: True - successfully | False - slot not free
    /// </summary>
    /// <param name="item">ItemEquip/ItemConsume/ItemOther</param>
    /// <param name="indexSlot">Index slot in inventory</param>
    /// <param name="createCopy">True - for create copy data item</param>
    /// <param name="checkIsVoidSlot">True - for check index slot on void</param>
    /// <returns></returns>
    public bool AddItemToInventory(Item item, int indexSlot, bool createCopy, bool checkIsVoidSlot)
    {
        if (checkIsVoidSlot && !IsFreeIndexInInventory(indexSlot))
            return false;
        Item result;
        if (createCopy)
            result = item.GetCopy();
        else
            result = item;
        result.indexItemInList = indexSlot;
        itemsInInv.Add(result);
        AddItemInObj(result, indexSlot);
        return true;
    }

    /// <summary>
    /// Return Item from inventory by id. Or load item from xml storage.
    /// </summary>
    /// <param name="id">Item id</param>
    /// <param name="elseLoadFromXml">True for try load item from xml storage</param>
    public Item GetItemById(int id, bool elseLoadFromXml)
    {
        foreach (Item iteminInv in itemsInInv)
            if (iteminInv.id == id)
                return iteminInv;

        if (elseLoadFromXml)
        {
            XmlStorageItem storageItem = new XmlStorageItem();
            return storageItem.GetItemById(id);
        }
        return null;
    }


    /// <summary>
    /// Delete item by id from inventory./
    /// Returns: True - successfully | False - fail
    /// </summary>
    /// <param name="id">Id item</param>
    /// <param name="quantity">Quantity items</param>
    /// <returns>True - successfully | False - fail</returns>
    public bool DeleteItemFromInventory(int id, int quantity)
    {
        int totalQuantity = 0;
        List<Item> allItemsInInvById = new List<Item>();

        foreach (Item itemInInv in itemsInInv)
        {
            if (itemInInv.id == id)
            {
                allItemsInInvById.Add(itemInInv);
                totalQuantity += itemInInv.quantity;
            }
        }
        if (allItemsInInvById.Count == 0 || totalQuantity < quantity)
            return false;


        foreach (Item itemInInvById in allItemsInInvById)
        {
            if (quantity <= itemInInvById.quantity)
            {
                itemInInvById.quantity -= quantity;
                UpdateItemObj(itemInInvById.indexItemInList);
                return true;
            }
            else
            {
                quantity -= itemInInvById.quantity;
                DeleteItemSlotFromInventory(itemInInvById);
            }

        }
        return false;
    }
    /// <summary>
    /// Delete item slot from inventor by indexItemInList
    /// </summary>
    /// <param name="item">Item from this inventory</param>
    public bool DeleteItemSlotFromInventory(Item item)
    {
        for (int i = 0; i < itemsInInv.Count; i++)
            if (itemsInInv[i].indexItemInList == item.indexItemInList)
            {
                Destroy(slotContainer.GetChild(item.indexItemInList).GetChild(0).gameObject);
                itemsInInv.RemoveAt(i);
                return true;
            }
        return false;
    }


    /// <summary>
    /// Check number free slots in inventory./
    /// Returns: True - inventor have "number" free slots | False - fail. 
    /// </summary>
    /// <param name="number">Number free slots</param>
    public bool CheckFreeSlot(int number)
    {
        int freeSlotsNumber = 0;
        for (int i = 0; i < slotContainer.childCount; i++)
        {
            if (slotContainer.GetChild(i).childCount == 0)
                freeSlotsNumber++;
            if (freeSlotsNumber == number)
                return true;
        }
        return false;
    }


    /// <summary>
    /// Add GameObgect Item in inventory
    /// </summary>
    /// <param name="item">ItemEquip/ItemConsume/ItemOther</param>
    /// <param name="indexSlot">Index slot</param>
    protected void AddItemInObj(Item item, int indexSlot)
    {
        item.LoadResources();
        GameObject itemObj = (GameObject)Instantiate(prefabItem, slotContainer.GetChild(indexSlot), false);
        ItemOnObject _ItemOnObject = itemObj.GetComponent<ItemOnObject>();
        _ItemOnObject.Item = item;
        //itemObj.transform.SetParent(slotContainer.transform.GetChild(i));
        //itemObj.GetComponent<RectTransform>().localPosition = Vector3.zero;
        _ItemOnObject.GetTypeParentinventory = typeParentInv;
        _ItemOnObject.UpdateItem();
    }
    protected void UpdateItemObj(int indexItemInList)
    {
        slotContainer.GetChild(indexItemInList).GetChild(0).GetComponent<ItemOnObject>().UpdateItem();
    }

    protected List<int> GetAllFreeIndexInInventory()
    {
        List<int> result = new List<int>();
        for (int i = 0; i < slotContainer.childCount; i++)
        {
            if (slotContainer.GetChild(i).childCount == 0)
            {
                result.Add(i);
            }
        }
        return result;
    }
    protected bool IsFreeIndexInInventory(int index)
    {
        if (slotContainer.GetChild(index).childCount == 0)
            return true;
        return false;
    }
    


    //----- Extension methods for AddItemInInvenory(Item item) ---
    private bool AddEquipInInvExtension(Item item)
    {
        List<int> freeSlots = GetAllFreeIndexInInventory();
        if (freeSlots.Count == 0)
            return false;
        item.indexItemInList = freeSlots[0];
        itemsInInv.Add(item);
        AddItemInObj(item, freeSlots[0]);
        return true;
    }
    private bool AddConsumInInvExtension(Item item)
    {
        ItemConsume tempItem = item as ItemConsume;
        bool createNew = true;

        //Search similar item in inventory
        foreach (Item itemInInv in itemsInInv)
            if (itemInInv.id == item.id)
                //If number items < maxInStack
                if (itemInInv.quantity + item.quantity <= tempItem.maxInStack)
                {
                    itemInInv.quantity += item.quantity;
                    UpdateItemObj(itemInInv.indexItemInList);
                    return true;
                }
                else
                {
                    //if found first like currItem
                    if (createNew)
                        tempItem = itemInInv as ItemConsume;
                    //Else: remember min number
                    else if (tempItem.quantity > itemInInv.quantity)
                        tempItem = itemInInv as ItemConsume;
                    createNew = false;
                }

        List<int> freeSlots = GetAllFreeIndexInInventory();
        int numIteration = 0;
        int additionNumber = item.quantity;
        if (createNew)
            additionNumber = 0;

        additionNumber -= (tempItem.maxInStack - tempItem.quantity);
        while (additionNumber > tempItem.maxInStack)
        {
            additionNumber -= tempItem.maxInStack;
            numIteration++;
        }

        if (freeSlots.Count < numIteration)
            return false;

        //add residue
        tempItem.quantity = additionNumber;
        int iter = 0;
        if (createNew)
        {
            tempItem.indexItemInList = freeSlots[0];
            itemsInInv.Add(tempItem);
            AddItemInObj(tempItem, freeSlots[0]);
            iter++;
        }
        else
            UpdateItemObj(tempItem.indexItemInList);

        //add max stacks
        while (iter < numIteration)
        {
            ItemConsume newTempItem = tempItem.getCopy();
            newTempItem.quantity = newTempItem.maxInStack;
            newTempItem.indexItemInList = freeSlots[iter];
            itemsInInv.Add(newTempItem);
            AddItemInObj(newTempItem, freeSlots[iter]);
            iter++;
        }
        return true;

    }
    private bool AddOtherInInvExtension(Item item)
    {
        foreach (Item itemInInv in itemsInInv)
        {
            if (itemInInv.id == item.id)
            {
                itemInInv.quantity += item.quantity;
                UpdateItemObj(itemInInv.indexItemInList);
                return true;
            }
        }
        List<int> freeSlots = GetAllFreeIndexInInventory();
        if (freeSlots.Count == 0)
            return false;
        item.indexItemInList = freeSlots[0];
        itemsInInv.Add(item);
        AddItemInObj(item, freeSlots[0]);
        return true;
    }
    //^^^^^ Extension methods for AddItemInInvenory(Item item) ^^^

    #endregion add/delete items

}
