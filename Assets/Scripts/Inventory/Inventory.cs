using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

public class Inventory : MonoBehaviour
{

    protected GameObject prefabItem;

    //Items
    protected List<ItemEquip> itemsEquipInInv;
    protected List<ItemConsume> itemsConsumeInInv;
    protected List<ItemOther> itemsOtherInInv;
    protected List<ItemTypesData> itemTypesDB;

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
        ItemType tempItemType = GetItemTypeByIdInInv(id);
        switch (tempItemType)
        {
            case ItemType.Other:
                return AddOthInInvExtension(id, quantity);
            case ItemType.Consume:
                return AddConsInInvExtension(id, quantity);
            case ItemType.Equip:
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
        XmlStorageItem xmlStorage = new XmlStorageItem();
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
    public void DelItemFromInventory(Item item)
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
        ItemType tempTypeItem = GetItemTypeByIdInInv(id);
        switch (tempTypeItem)
        {
            case ItemType.Other:
                for (int i = 0; i < itemsOtherInInv.Count; i++)
                    if (itemsOtherInInv[i].id == id)
                    {
                        totalQuantity += itemsOtherInInv[i].quantity;
                        items[i] = itemsOtherInInv[i].quantity;
                    }
                break;
            case ItemType.Consume:
                for (int i = 0; i < itemsConsumeInInv.Count; i++)
                    if (itemsConsumeInInv[i].id == id)
                    {
                        totalQuantity += itemsConsumeInInv[i].quantity;
                        items[i] = itemsConsumeInInv[i].quantity;
                    }
                break;
            case ItemType.Equip:
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
                IncOrDecQuantityItemByIndex((int)item.Key, -(int)item.Value, tempTypeItem);
                RemoveItemFromInvByIndex((int)item.Key, tempTypeItem);
            }
            if (quantity == 0)
            {
                return true;
            }
        }
        return false;
    }

    public virtual ItemEquip GetEquipItem(Item item)
    {
        foreach (ItemEquip itemEquip in itemsEquipInInv)
            if (itemEquip.indexItemInList == item.indexItemInList)
                return itemEquip;
        return null;
    }
    public ItemConsume GetConsumeItem(Item item)
    {
        foreach (ItemConsume itemConsume in itemsConsumeInInv)
            if (itemConsume.indexItemInList == item.indexItemInList)
                return itemConsume;
        return null;
    }
    public ItemOther GetOtherItem(Item item)
    {
        foreach (ItemOther itemOther in itemsOtherInInv)
            if (itemOther.indexItemInList == item.indexItemInList)
                return itemOther;
        return null;
    }
    public ItemEquip GetItemEquipById(int id, bool elseLoadFromXml)
    {
        //if item has already in inventory
        foreach (ItemEquip itemEquipInInv in itemsEquipInInv)
            if (itemEquipInInv.id == id)
                return itemEquipInInv;

        //else add from xml storage item
        if (elseLoadFromXml)
        {
            XmlStorageItem xmlStorage = new XmlStorageItem();
            return xmlStorage.GetItemEquipById(id);
        }
        return null;
    }
    public ItemConsume GetItemConsumeById(int id, bool elseLoadFromXml)
    {
        //if item has already in inventory
        foreach (ItemConsume itemConsumeInInv in itemsConsumeInInv)
            if (itemConsumeInInv.id == id)
                return itemConsumeInInv;

        //else add from xml storage item
        if (elseLoadFromXml)
        {
            XmlStorageItem xmlStorage = new XmlStorageItem();
            return xmlStorage.GetItemConsumeById(id);
        }

        return null;
    }
    public ItemOther GetItemOtherById(int id, bool elseLoadFromXml)
    {
        //if item has already in inventory
        foreach (ItemOther itemOtherinInv in itemsOtherInInv)
            if (itemOtherinInv.id == id)
                return itemOtherinInv;

        //Add new item from xml storage
        if (elseLoadFromXml)
        {
            XmlStorageItem xmlStorage = new XmlStorageItem();
            return xmlStorage.GetItemOtherById(id);
        }

        return null;
    }
    public Item GetItemById(int id, bool elseLoadFromXml)
    {
        foreach (ItemOther itemOtherinInv in itemsOtherInInv)
            if (itemOtherinInv.id == id)
                return itemOtherinInv;
        foreach (ItemConsume itemConsumeInInv in itemsConsumeInInv)
            if (itemConsumeInInv.id == id)
                return itemConsumeInInv;
        foreach (ItemEquip itemEquipInInv in itemsEquipInInv)
            if (itemEquipInInv.id == id)
                return itemEquipInInv;
        if (elseLoadFromXml)
        {
            XmlStorageItem storageItem = new XmlStorageItem();
            return storageItem.GetResultItemById(id);
        }
        return null;
    }


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
    protected void AddItemInObj(Item tempItem, int indexSlot)
    {
        if (itemTypesDB != null)
            itemTypesDB.Add(new ItemTypesData(tempItem.id, tempItem.itemType));
        tempItem.LoadResources();
        GameObject itemObj = (GameObject)Instantiate(prefabItem, slotContainer.GetChild(indexSlot), false);
        ItemOnObject _ItemOnObject = itemObj.GetComponent<ItemOnObject>();
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
        List<int> freeSlotsIndex = GetAllFreeIndexInInventory();
        if (freeSlotsIndex.Count < quantity)
            return false;

        XmlStorageItem xmlStorage = new XmlStorageItem();
        ItemEquip tempItemEquip = GetItemEquipById(id, true);

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
        bool createNew = false;
        XmlStorageItem xmlStorage = new XmlStorageItem();
        List<int> freeSlotsIndex = GetAllFreeIndexInInventory();
        ItemConsume tempItemConsume = GetItemConsumeById(id, false);

        //else add from xml storage item
        if (tempItemConsume == null)
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
        ItemOther tempItemOther = GetItemOtherById(id, false);

        if (tempItemOther == null)
        {
            XmlStorageItem xmlStorage = new XmlStorageItem();
            List<int> freeSlotsIndex = GetAllFreeIndexInInventory();
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
    protected ItemType GetItemTypeByIdInInv(int id)
    {
        XmlStorageItem xmlStorage = new XmlStorageItem();
        foreach (ItemTypesData tempData in itemTypesDB)
            if (tempData.id == id)
                return tempData.itemType;
        return xmlStorage.GetItemTypeById(id);
    }

    protected void IncOrDecQuantityItemByIndex(int index, int incOrDec, ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Other:
                itemsOtherInInv[index].quantity += incOrDec;
                UpdateItemObj(itemsOtherInInv[index].indexItemInList);
                break;
            case ItemType.Consume:
                itemsConsumeInInv[index].quantity += incOrDec;
                UpdateItemObj(itemsConsumeInInv[index].indexItemInList);
                break;
            case ItemType.Equip:
                itemsEquipInInv[index].quantity += incOrDec;
                UpdateItemObj(itemsEquipInInv[index].indexItemInList);
                break;
        }
    }
    protected void RemoveItemFromInvByIndex(int index, ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Other:
                Destroy(slotContainer.GetChild(itemsOtherInInv[index].indexItemInList).GetChild(0).gameObject);
                itemsOtherInInv.RemoveAt(index);
                break;
            case ItemType.Consume:
                Destroy(slotContainer.GetChild(itemsConsumeInInv[index].indexItemInList).GetChild(0).gameObject);
                itemsConsumeInInv.RemoveAt(index);
                break;
            case ItemType.Equip:
                Destroy(slotContainer.GetChild(itemsEquipInInv[index].indexItemInList).GetChild(0).gameObject);
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
        slotContainer.GetChild(indexItemInList).GetChild(0).GetComponent<ItemOnObject>().UpdateItem();
    }

    #endregion add/delete items

}
