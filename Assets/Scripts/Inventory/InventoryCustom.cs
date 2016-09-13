using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

public class InventoryCustom : MonoBehaviour
{

    protected GameObject prefabItem;

    //Items
    protected List<ItemEquip> itemsEquipInInv;
    protected List<ItemConsume> itemsConsumeInInv;
    protected List<ItemOther> itemsOtherInInv;
    protected List<ItemTypesData> itemTypesDB;

    protected GameObject slotContainer;
    protected bool isActive;


    public delegate void InventoryEvent();
    public event InventoryEvent InventoryInit;
    public event InventoryEvent InventoryOpen;

    public enum TypeParentInv { bag, equip, hotbar, storage, shop };
    [HideInInspector]
    public TypeParentInv typeParentInv;

    public bool IsActive()
    {
        return isActive;
    }

    protected virtual void Start()
    {
        slotContainer = transform.Find("Slots").gameObject;
        prefabItem = Resources.Load("Prefabs/Inventory/Item") as GameObject;
        isActive = false;
        gameObject.SetActive(false);

        if (InventoryInit != null)
            InventoryInit();
    }
    protected void Initiate()
    {
        itemsEquipInInv = new List<ItemEquip>();
        itemsConsumeInInv = new List<ItemConsume>();
        itemsOtherInInv = new List<ItemOther>();
        itemTypesDB = new List<ItemTypesData>();
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
        //checkIfAllInventoryClosed();
    }

    public void LoadSerialization(InventoryData loadedInventory)
    {
        itemsEquipInInv = loadedInventory.itemsEquipInInv;
        itemsConsumeInInv = loadedInventory.itemsConsumeInInv;
        itemsOtherInInv = loadedInventory.itemsOtherInInv;
        SyncItemsInInventory();
    }

    #region add/delete items

    protected bool SyncItemsInInventory()
    {
        foreach (ItemEquip ItemEquip in itemsEquipInInv)
            AddItemInObj(ItemEquip, ItemEquip.indexItemInList);
        foreach (ItemConsume itemConsume in itemsConsumeInInv)
            AddItemInObj(itemConsume, itemConsume.indexItemInList);
        foreach (ItemOther itemOther in itemsOtherInInv)
            AddItemInObj(itemOther, itemOther.indexItemInList);
        return true;
    }

    /// <summary>
    /// Add item to inventory search by id in free slot./
    /// Returns: True - successfully | False - fail
    /// </summary>
    /// <param name="id">Id item</param>
    /// <param name="quantity">Quantity items</param>
    /// <returns>True - successfully | False - fail</returns>
    public bool AddItemToInventory(int id, int quantity)
    {
        ItemTypeCustom tempItemType = GetItemTypeByIdInInv(id);
        switch (tempItemType)
        {
            case ItemTypeCustom.Other:
                return AddOthInInvExtension(id, quantity);
            case ItemTypeCustom.Consume:
                return AddConsInInvExtension(id, quantity);
            case ItemTypeCustom.Equip:
                return AddEquInInvExtension(id, quantity, 0);
        }
        return false;
    }
    /// <summary>
    /// Add equip item to inventory in free slot./
    /// Returns: True - successfully | False - fail
    /// </summary>
    /// <param name="itemEquip">Equip item</param>
    /// <returns>True - successfully | False - fail</returns>
    public bool AddItemToInventory(ItemEquip itemEquip)
    {
        List<int> freeSlotsIndex = GetAllFreeIndexInInventory();
        if (freeSlotsIndex.Count == 0)
            return false;
        AddItemInSlot(itemEquip, freeSlotsIndex[0], true);
        return true;
    }
    /// <summary>
    /// Add consume item to inventory in free slot./
    /// Returns: True - successfully | False - fail
    /// </summary>
    /// <param name="itemConsume">Consume item</param>
    /// <returns>True - successfully | False - fail</returns>
    public bool AddItemToInventory(ItemConsume itemConsume)
    {
        ItemConsume tempItemConsume = new ItemConsume();
        List<int> freeSlotsIndex = GetAllFreeIndexInInventory();
        ItemXmlStorage xmlStorage = new ItemXmlStorage();
        int maxInStack = xmlStorage.GetConsumeMaxInStackById(itemConsume.id);

        foreach (ItemConsume itemConsumeInInv in itemsConsumeInInv)
            if (itemConsumeInInv.id == itemConsume.id)
            {
                tempItemConsume = itemConsumeInInv;
                break;
            }

        if (tempItemConsume.IsEmpty())
        {
            if (freeSlotsIndex.Count == 0)
                return false;
            return AddConsInInvExtension(itemConsume, 0, maxInStack, true, freeSlotsIndex);
        }
        else
        {
            return AddConsInInvExtension(tempItemConsume, itemConsume.quantity, maxInStack, false, freeSlotsIndex);
        }
    }
    /// <summary>
    /// Add other item to inventory in free slot./
    /// Returns: True - successfully | False - fail
    /// </summary>
    /// <param name="itemOther">Other item</param>
    /// <returns>True - successfully | False - fail</returns>
    public bool AddItemToInventory(ItemOther itemOther)
    {
        ItemOther tempItemOther = new ItemOther();
        //if item has already in inventory
        foreach (ItemOther itemOtherinInv in itemsOtherInInv)
            if (itemOtherinInv.id == itemOther.id)
            {
                tempItemOther = itemOtherinInv;
                break;
            }

        if (tempItemOther.IsEmpty())
        {
            List<int> freeSlotsIndex = GetAllFreeIndexInInventory();
            if (freeSlotsIndex.Count == 0)
                return false;
            AddItemInSlot(itemOther, freeSlotsIndex[0], true);
        }
        else
        {
            //update quntity item ha already
            tempItemOther.quantity += itemOther.quantity;
            UpdateItemObj(tempItemOther.indexItemInList);
        }
        return true;
    }
    /// <summary>
    /// Add equip item to inventory in indexSlot./
    /// Returns: True - successfully | False - slot not free
    /// </summary>
    /// <param name="itemEquip">Equip item</param>
    /// <param name="indexSlot">Index Slot</param>
    /// <param name="checkIsVoid">Check is free iindex in inventory</param>
    /// <returns>True - successfully | False - fail</returns>
    public bool AddItemToInventory(ItemEquip itemEquip, int indexSlot, bool checkIsVoid)
    {
        if (checkIsVoid && !IsFreeIndexInInventory(indexSlot))
            return false;
        AddItemInSlot(itemEquip, indexSlot, true);
        return true;
    }
    /// <summary>
    /// Add consume item to inventory in indexSlot./
    /// Returns: True - successfully | False - slot not free
    /// </summary>
    /// <param name="itemConsume">Consume item</param>
    /// <param name="indexSlot">Index Slot</param>
    /// <param name="checkIsVoid">Check is free iindex in inventory</param>
    /// <returns>True - successfully | False - fail</returns>
    public bool AddItemToInventory(ItemConsume itemConsume, int indexSlot, bool checkIsVoid)
    {
        if (checkIsVoid && !IsFreeIndexInInventory(indexSlot))
            return false;
        AddItemInSlot(itemConsume, indexSlot, true);
        return true;
    }
    /// <summary>
    /// Add other item to inventory in indexSlot./
    /// Returns: True - successfully | False - slot not free
    /// </summary>
    /// <param name="itemOther">Other item</param>
    /// <param name="indexSlot">Index Slot</param>
    /// <param name="checkIsVoid">Check is free iindex in inventory</param>
    /// <returns>True - successfully | False - fail</returns>
    public bool AddItemToInventory(ItemOther itemOther, int indexSlot, bool checkIsVoid)
    {
        if (checkIsVoid && !IsFreeIndexInInventory(indexSlot))
            return false;
        AddItemInSlot(itemOther, indexSlot, true);
        return true;
    }


    /// <summary>
    /// Delete item(stack) for this inventory
    /// </summary>
    /// <param name="itemOther">Specimen item in inventory</param>
    public void DelItemFromInventory(ItemCustom item)
    {
        //If item is Equip
        for (int i = 0; i < itemsEquipInInv.Count; i++)
            if (itemsEquipInInv[i].indexItemInList == item.indexItemInList)
            {
                RemoveItemFromInvByIndex(i, item.itemType);
                return;
            }

        //If item is Consume
        for (int i = 0; i < itemsConsumeInInv.Count; i++)
            if (itemsConsumeInInv[i].indexItemInList == item.indexItemInList)
            {
                RemoveItemFromInvByIndex(i, item.itemType);
                return;
            }

        //If item is Other
        for (int i = 0; i < itemsOtherInInv.Count; i++)
            if (itemsOtherInInv[i].indexItemInList == item.indexItemInList)
            {
                RemoveItemFromInvByIndex(i, item.itemType);
                return;
            }
    }
    public void DelItemFromInventory(ItemEquip itemEquip)
    {
        for (int i = 0; i < itemsEquipInInv.Count; i++)
            if (itemsEquipInInv[i].indexItemInList == itemEquip.indexItemInList)
                RemoveItemFromInvByIndex(i, itemEquip.itemType);
    }
    public void DelItemFromInventory(ItemConsume itemConsume)
    {
        for (int i = 0; i < itemsConsumeInInv.Count; i++)
            if (itemsConsumeInInv[i].indexItemInList == itemConsume.indexItemInList)
                RemoveItemFromInvByIndex(i, itemConsume.itemType);
    }
    public void DelItemFromInventory(ItemOther itemOther)
    {
        for (int i = 0; i < itemsOtherInInv.Count; i++)
            if (itemsOtherInInv[i].indexItemInList == itemOther.indexItemInList)
                RemoveItemFromInvByIndex(i, itemOther.itemType);
    }
    /// <summary>
    /// Delete item by id from inventory./
    /// Returns: True - successfully | False - fail
    /// </summary>
    /// <param name="id">Id item</param>
    /// <param name="quantity">Quantity items</param>
    /// <returns>True - successfully | False - fail</returns>
    public bool DelItemFromInventory(int id, int quantity)
    {
        int totalQuantity = 0;
        Hashtable items = new Hashtable();
        ItemTypeCustom tempTypeItem = GetItemTypeByIdInInv(id);
        switch (tempTypeItem)
        {
            case ItemTypeCustom.Other:
                for (int i = 0; i < itemsOtherInInv.Count; i++)
                    if (itemsOtherInInv[i].id == id)
                    {
                        totalQuantity += itemsOtherInInv[i].quantity;
                        items[i] = itemsOtherInInv[i].quantity;
                    }
                break;
            case ItemTypeCustom.Consume:
                for (int i = 0; i < itemsConsumeInInv.Count; i++)
                    if (itemsConsumeInInv[i].id == id)
                    {
                        totalQuantity += itemsConsumeInInv[i].quantity;
                        items[i] = itemsConsumeInInv[i].quantity;
                    }
                break;
            case ItemTypeCustom.Equip:
                for (int i = 0; i < itemsEquipInInv.Count; i++)
                    if (itemsEquipInInv[i].id == id)
                    {
                        totalQuantity += itemsEquipInInv[i].quantity;
                        items[i] = itemsEquipInInv[i].quantity;
                    }
                break;
        }

        if (totalQuantity < quantity)
            return false;

        foreach (DictionaryEntry item in items)
        {
            if (quantity < (int)item.Value)
            {
                IncOrDecQuantityItemByIndex((int)item.Key, -quantity, tempTypeItem);
                return true;
            }
            else
            {
                quantity -= (int)item.Value;
                RemoveItemFromInvByIndex((int)item.Key, tempTypeItem);
            }
            if (quantity == 0)
            {
                return true;
            }
        }
        return false;
    }

    public virtual ItemEquip GetEquipItem(ItemCustom item)
    {
        foreach (ItemEquip itemEquip in itemsEquipInInv)
            if (itemEquip.indexItemInList == item.indexItemInList)
                return itemEquip;
        return null;
    }
    public ItemConsume GetConsumeItem(ItemCustom item)
    {
        foreach (ItemConsume itemConsume in itemsConsumeInInv)
            if (itemConsume.indexItemInList == item.indexItemInList)
                return itemConsume;
        return null;
    }
    public ItemOther GetOtherItem(ItemCustom item)
    {
        foreach (ItemOther itemOther in itemsOtherInInv)
            if (itemOther.indexItemInList == item.indexItemInList)
                return itemOther;
        return null;
    }

    protected void AddItemInSlot(ItemEquip itemEquip, int indexSlot, bool createCopy)
    {
        ItemEquip result;
        if (createCopy)
            result = itemEquip.getCopy();
        else
            result = itemEquip;
        result.indexItemInList = indexSlot;
        itemsEquipInInv.Add(result);
        AddItemInObj(result, indexSlot);
    }
    protected void AddItemInSlot(ItemConsume itemConsume, int indexSlot, bool createCopy)
    {
        ItemConsume result;
        if (createCopy)
            result = itemConsume.getCopy();
        else
            result = itemConsume;
        result.indexItemInList = indexSlot;
        itemsConsumeInInv.Add(result);
        AddItemInObj(result, indexSlot);
    }
    protected void AddItemInSlot(ItemOther itemOther, int indexSlot, bool createCopy)
    {
        ItemOther result;
        if (createCopy)
            result = itemOther.getCopy();
        else
            result = itemOther;
        result.indexItemInList = indexSlot;
        itemsOtherInInv.Add(result);
        AddItemInObj(result, indexSlot);
    }
    protected void AddItemInObj(ItemCustom tempItem, int indexSlot)
    {
        if (itemTypesDB != null)
            itemTypesDB.Add(new ItemTypesData(tempItem.id, tempItem.itemType));
        tempItem.LoadResources();
        GameObject itemObj = (GameObject)Instantiate(prefabItem, slotContainer.transform.GetChild(indexSlot), false);
        ItemOnObjectCustom _ItemOnObject = itemObj.GetComponent<ItemOnObjectCustom>();
        _ItemOnObject.Item = tempItem;
        //itemObj.transform.SetParent(slotContainer.transform.GetChild(i));
        //itemObj.GetComponent<RectTransform>().localPosition = Vector3.zero;
        _ItemOnObject.GetTypeParentinventory = typeParentInv;
        _ItemOnObject.UpdateItem();
    }
    /// <summary>
    /// Add Equip in inventory extension method
    /// </summary>
    protected bool AddEquInInvExtension(int id, int quantity, int enchantLvl)
    {
        ItemXmlStorage xmlStorage = new ItemXmlStorage();
        List<int> freeSlotsIndex = GetAllFreeIndexInInventory();
        ItemEquip tempItemEquip = new ItemEquip();

        if (freeSlotsIndex.Count < quantity)
            return false;

        //if item has already in inventory
        foreach (ItemEquip itemEquipInInv in itemsEquipInInv)
            if (itemEquipInInv.id == id)
            {
                tempItemEquip = itemEquipInInv;
                break;
            }

        //else add from xml storage item
        if (tempItemEquip.IsEmpty())
        {
            tempItemEquip = xmlStorage.GetItemEquipById(id);
            if (tempItemEquip == null)
                return false;
        }
        for (int i = 0; i < quantity; i++)
        {
            ItemEquip newTempItem = tempItemEquip.getCopy();
            newTempItem.enchantLevel = enchantLvl;
            AddItemInSlot(newTempItem, freeSlotsIndex[i], false);
        }

        return false;
    }
    /// <summary>
    /// Add Consume in inventory extension method
    /// </summary>
    protected bool AddConsInInvExtension(int id, int quantity)
    {
        ItemXmlStorage xmlStorage = new ItemXmlStorage();
        bool createNew = false;
        List<int> freeSlotsIndex = GetAllFreeIndexInInventory();
        ItemConsume tempItemConsume = new ItemConsume();

        //if item has already in inventory
        foreach (ItemConsume itemConsumeInInv in itemsConsumeInInv)
            if (itemConsumeInInv.id == id)
            {
                tempItemConsume = itemConsumeInInv;
                break;
            }

        //else add from xml storage item
        if (tempItemConsume.IsEmpty())
        {
            if (freeSlotsIndex.Count == 0)
                return false;
            tempItemConsume = xmlStorage.GetItemConsumeById(id);
            if (tempItemConsume == null)
                return false;
            createNew = true;
        }

        int maxInStack = xmlStorage.GetConsumeMaxInStackById(id);
        return AddConsInInvExtension(tempItemConsume, quantity, maxInStack, createNew, freeSlotsIndex);

    }
    /// <summary>
    /// Add Consume in inventory second extension method
    /// </summary>
    protected bool AddConsInInvExtension(ItemConsume itemConsume, int quantity, int maxInStack, bool createNew, List<int> freeSlotsIndex)
    {
        int numIteration = 0;
        if (itemConsume.quantity + quantity <= maxInStack)
        {
            itemConsume.quantity += quantity;
            if (createNew)
                AddItemInSlot(itemConsume, freeSlotsIndex[0], true);
            else
                UpdateItemObj(itemConsume.indexItemInList);
            return true;
        }

        //else quantity > maxInStack
        quantity -= (maxInStack - itemConsume.quantity);
        while (quantity > maxInStack)
        {
            quantity -= maxInStack;
            numIteration++;
        }

        if (freeSlotsIndex.Count < numIteration)
            return false;

        //add residue
        itemConsume.quantity = quantity;
        int iter = 0;
        if (createNew)
        {
            AddItemInSlot(itemConsume, freeSlotsIndex[0], true);
            iter++;
        }
        else
            UpdateItemObj(itemConsume.indexItemInList);

        //add max stacks
        while (iter < numIteration)
        {
            ItemConsume newTempItem = itemConsume.getCopy();
            newTempItem.quantity = maxInStack;
            AddItemInSlot(newTempItem, freeSlotsIndex[iter], false);
            iter++;
        }
        return true;
    }
    /// <summary>
    /// Add Other item in inventory extension method
    /// </summary>
    protected bool AddOthInInvExtension(int id, int quantity)
    {
        ItemXmlStorage xmlStorage = new ItemXmlStorage();
        List<int> freeSlotsIndex = GetAllFreeIndexInInventory();
        ItemOther tempItemOther = new ItemOther();

        //if item has already in inventory
        foreach (ItemOther itemOtherinInv in itemsOtherInInv)
            if (itemOtherinInv.id == id)
            {
                tempItemOther = itemOtherinInv;
                break;
            }

        if (tempItemOther.IsEmpty())
        {
            //Add new item from xml storage
            if (freeSlotsIndex.Count == 0)
                return false;

            tempItemOther = xmlStorage.GetItemOtherById(id);
            if (tempItemOther == null)
                return false;

            tempItemOther.quantity = quantity;
            AddItemInSlot(tempItemOther, freeSlotsIndex[0], false);
        }
        else
        {
            //update quntity item ha already
            tempItemOther.quantity += quantity;
            UpdateItemObj(tempItemOther.indexItemInList);
        }
        return true;
    }

    protected List<int> GetAllFreeIndexInInventory()
    {
        List<int> result = new List<int>();
        for (int i = 0; i < slotContainer.transform.childCount; i++)
        {
            if (slotContainer.transform.GetChild(i).childCount == 0)
            {
                result.Add(i);
            }
        }
        return result;
    }
    protected bool IsFreeIndexInInventory(int index)
    {
        if (slotContainer.transform.GetChild(index).childCount == 0)
            return true;
        return false;
    }
    protected ItemTypeCustom GetItemTypeByIdInInv(int id)
    {
        ItemXmlStorage xmlStorage = new ItemXmlStorage();
        foreach (ItemTypesData tempData in itemTypesDB)
            if (tempData.id == id)
                return tempData.itemType;
        return xmlStorage.GetItemTypeById(id);
    }

    protected void IncOrDecQuantityItemByIndex(int index, int incOrDec, ItemTypeCustom itemType)
    {
        switch (itemType)
        {
            case ItemTypeCustom.Other:
                itemsOtherInInv[index].quantity += incOrDec;
                UpdateItemObj(itemsOtherInInv[index].indexItemInList);
                break;
            case ItemTypeCustom.Consume:
                itemsConsumeInInv[index].quantity += incOrDec;
                UpdateItemObj(itemsConsumeInInv[index].indexItemInList);
                break;
            case ItemTypeCustom.Equip:
                itemsEquipInInv[index].quantity += incOrDec;
                UpdateItemObj(itemsEquipInInv[index].indexItemInList);
                break;
        }
    }
    protected void RemoveItemFromInvByIndex(int index, ItemTypeCustom itemType)
    {
        switch (itemType)
        {
            case ItemTypeCustom.Other:
                Destroy(slotContainer.transform.GetChild(itemsOtherInInv[index].indexItemInList).GetChild(0).gameObject);
                itemsOtherInInv.RemoveAt(index);
                break;
            case ItemTypeCustom.Consume:
                Destroy(slotContainer.transform.GetChild(itemsConsumeInInv[index].indexItemInList).GetChild(0).gameObject);
                itemsConsumeInInv.RemoveAt(index);
                break;
            case ItemTypeCustom.Equip:
                Destroy(slotContainer.transform.GetChild(itemsEquipInInv[index].indexItemInList).GetChild(0).gameObject);
                itemsEquipInInv.RemoveAt(index);
                break;
        }
    }
    protected void RemoveItemFromTypeData(int id)
    {
        for (int i = 0; i < itemTypesDB.Count; i++)
            if (itemTypesDB[i].id == id)
            {
                itemTypesDB.RemoveAt(i);
                break;
            }
    }
    protected void UpdateItemObj(int indexItemInList)
    {
        slotContainer.transform.GetChild(indexItemInList).GetChild(0).GetComponent<ItemOnObjectCustom>().UpdateItem();
    }

    #endregion add/delete items

}
