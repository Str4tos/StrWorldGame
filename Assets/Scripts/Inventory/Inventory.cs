using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    //Prefabs
    [SerializeField]
    private GameObject prefabCanvasWithPanel;
    [SerializeField]
    private GameObject prefabSlot;
    [SerializeField]
    private GameObject prefabSlotContainer;
    [SerializeField]
    private GameObject prefabItem;
    [SerializeField]
    private GameObject prefabDraggingItemContainer;
    [SerializeField]
    private GameObject prefabPanel;

    //Itemdatabase
    [SerializeField]
    private ItemIngridientsDataBaseList itemDatabase;

    //GameObjects which are alive
    [SerializeField]
    private string inventoryTitle;
    [SerializeField]
    private RectTransform PanelRectTransform;
    [SerializeField]
    private Image PanelImage;
    [SerializeField]
    private GameObject SlotContainer;
    [SerializeField]
    private GameObject DraggingItemContainer;
    [SerializeField]
    private RectTransform SlotContainerRectTransform;
    [SerializeField]
    private GridLayoutGroup SlotGridLayout;
    [SerializeField]
    private RectTransform SlotGridRectTransform;

    //Inventory Settings
    [SerializeField]
    public bool mainInventory;
    [SerializeField]
    public List<Item> ItemsInInventory = new List<Item>();
    [SerializeField]
    public int height;
    [SerializeField]
    public int width;
    [SerializeField]
    public bool stackable;
    [SerializeField]
    public static bool inventoryOpen;


    //GUI Settings
    [SerializeField]
    public int slotSize;
    [SerializeField]
    public int iconSize;
    [SerializeField]
    public int paddingBetweenX;
    [SerializeField]
    public int paddingBetweenY;
    [SerializeField]
    public int paddingLeft;
    [SerializeField]
    public int paddingRight;
    [SerializeField]
    public int paddingBottom;
    [SerializeField]
    public int paddingTop;
    [SerializeField]
    public int positionNumberX;
    [SerializeField]
    public int positionNumberY;
    
	public int cash = 0;
	private Text txtCash;
	private StatusPlayer _StatusPlayer;

    //InputManager inputManagerDatabase;

    //event delegates for consuming, gearing
    public delegate void ItemDelegate(Item item);
    public static event ItemDelegate ItemConsumed;
    public static event ItemDelegate ItemEquip;
    public static event ItemDelegate UnEquipItem;

    public delegate void InventoryOpened();
    public static event InventoryOpened InventoryOpen;
    public static event InventoryOpened AllInventoriesClosed;

    void Start()
    {
        if (transform.GetComponent<Hotbar>() == null)
            this.gameObject.SetActive(false);
		if(mainInventory)
			txtCash = gameObject.transform.Find("CashCanvas/LabelCash").GetComponent<Text>();
		_StatusPlayer = GameObject.FindWithTag ("Player").GetComponent<StatusPlayer>();
        updateItemList();


        //if (!player)
        //{
        //    player = this.gameObject;
        //}
        //ItemDataC dataItem = database.GetComponent<ItemDataC>();
        //player.GetComponent<StatusC>().addAtk = 0;
        //player.GetComponent<StatusC>().addDef = 0;
        //player.GetComponent<StatusC>().addMatk = 0;
        //player.GetComponent<StatusC>().addMdef = 0;
        //player.GetComponent<StatusC>().weaponAtk = 0;
        //player.GetComponent<StatusC>().weaponMatk = 0;
        ////Set New Variable of Weapon
        //player.GetComponent<StatusC>().weaponAtk += dataItem.equipment[weaponEquip].attack;
        //player.GetComponent<StatusC>().addDef += dataItem.equipment[weaponEquip].defense;
        //player.GetComponent<StatusC>().weaponMatk += dataItem.equipment[weaponEquip].magicAttack;
        //player.GetComponent<StatusC>().addMdef += dataItem.equipment[weaponEquip].magicDefense;
        ////Set New Variable of Armor
        //player.GetComponent<StatusC>().weaponAtk += dataItem.equipment[armorEquip].attack;
        //player.GetComponent<StatusC>().addDef += dataItem.equipment[armorEquip].defense;
        //player.GetComponent<StatusC>().weaponMatk += dataItem.equipment[armorEquip].magicAttack;
        //player.GetComponent<StatusC>().addMdef += dataItem.equipment[armorEquip].magicDefense;
        //player.GetComponent<StatusC>().CalculateStatus();

        //inputManagerDatabase = (InputManager)Resources.Load("InputManager");
    }

    public void sortItems()
    {
        int empty = -1;
        for (int i = 0; i < SlotContainer.transform.childCount; i++)
        {
            if (SlotContainer.transform.GetChild(i).childCount == 0 && empty == -1)
                empty = i;
            else
            {
                if (empty > -1)
                {
                    if (SlotContainer.transform.GetChild(i).childCount != 0)
                    {
                        RectTransform rect = SlotContainer.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>();
                        SlotContainer.transform.GetChild(i).GetChild(0).transform.SetParent(SlotContainer.transform.GetChild(empty).transform);
                        rect.localPosition = Vector3.zero;
                        i = empty + 1;
                        empty = i;
                    }
                }
            }
        }
    }

    void Update()
    {
        updateItemIndex();
    }

    public void OnUpdateItemList()
    {
        updateItemList();
    }

    public void closeInventory()
    {
        this.gameObject.SetActive(false);
        checkIfAllInventoryClosed();
    }

    public void openInventory()
    {
        this.gameObject.SetActive(true);
		updateCashValue ();
		if (this.tag == "EquipmentSystem") {
			_StatusPlayer.RefreshStatsInInventory (this.gameObject.transform.Find ("Stats"));
		}
		if (InventoryOpen != null) {
			
			InventoryOpen ();
		}
    }

    public void checkIfAllInventoryClosed()
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("MainCanvas");

        for (int i = 0; i < canvas.transform.childCount; i++)
        {
            GameObject child = canvas.transform.GetChild(i).gameObject;
            if (!child.activeSelf && (child.tag == "EquipmentSystem" || child.tag == "Panel" || child.tag == "MainInventory" || child.tag == "CraftSystem"))
            {
                if (AllInventoriesClosed != null && i == canvas.transform.childCount - 1)
                    AllInventoriesClosed();
            }
            else if (child.activeSelf && (child.tag == "EquipmentSystem" || child.tag == "Panel" || child.tag == "MainInventory" || child.tag == "CraftSystem"))
                break;

            else if (i == canvas.transform.childCount - 1)
            {
                if (AllInventoriesClosed != null)
                    AllInventoriesClosed();
            }


        }
    }




    public void ConsumeItem(Item item)
    {
        if (ItemConsumed != null)
            ItemConsumed(item);
    }

    public void EquiptItem(Item item)
    {
        if (ItemEquip != null)
            ItemEquip(item);
    }

    public void UnEquipItem1(Item item)
    {
        if (UnEquipItem != null)
            UnEquipItem(item);
    }



    public void setImportantVariables()
    {
        PanelRectTransform = GetComponent<RectTransform>();
        SlotContainer = transform.GetChild(1).gameObject;
        SlotGridLayout = SlotContainer.GetComponent<GridLayoutGroup>();
        SlotGridRectTransform = SlotContainer.GetComponent<RectTransform>();
    }

    public void updateItemList()
    {
        ItemsInInventory.Clear();
        for (int i = 0; i < SlotContainer.transform.childCount; i++)
        {
            Transform trans = SlotContainer.transform.GetChild(i);
            if (trans.childCount != 0)
            {
                ItemsInInventory.Add(trans.GetChild(0).GetComponent<ItemOnObject>().item);
            }
        }
		if (mainInventory) {

		}

    }

	public void updateCashValue(){
		if (mainInventory) {
			txtCash.text = "" + cash;
		}
	}

	public void updateCashValue(int cash){
        if (mainInventory)
        {
            this.cash = cash;
            txtCash.text = "" + cash;
        }
	}

  

    //public void updateSlotAmount(int width, int height)
    //{
    //    if (prefabSlot == null)
    //        prefabSlot = Resources.Load("Prefabs/Slot - Inventory") as GameObject;

    //    if (SlotContainer == null)
    //    {
    //        SlotContainer = (GameObject)Instantiate(prefabSlotContainer);
    //        SlotContainer.transform.SetParent(PanelRectTransform.transform);
    //        SlotContainerRectTransform = SlotContainer.GetComponent<RectTransform>();
    //        SlotGridRectTransform = SlotContainer.GetComponent<RectTransform>();
    //        SlotGridLayout = SlotContainer.GetComponent<GridLayoutGroup>();
    //    }

    //    if (SlotContainerRectTransform == null)
    //        SlotContainerRectTransform = SlotContainer.GetComponent<RectTransform>();

    //    SlotContainerRectTransform.localPosition = Vector3.zero;

    //    List<Item> itemsToMove = new List<Item>();
    //    List<GameObject> slotList = new List<GameObject>();
    //    foreach (Transform child in SlotContainer.transform)
    //    {
    //        if (child.tag == "Slot") { slotList.Add(child.gameObject); }
    //    }

    //    while (slotList.Count > width * height)
    //    {
    //        GameObject go = slotList[slotList.Count - 1];
    //        ItemOnObject itemInSlot = go.GetComponentInChildren<ItemOnObject>();
    //        if (itemInSlot != null)
    //        {
    //            itemsToMove.Add(itemInSlot.item);
    //            ItemsInInventory.Remove(itemInSlot.item);
    //        }
    //        slotList.Remove(go);
    //        DestroyImmediate(go);
    //    }

    //    if (slotList.Count < width * height)
    //    {
    //        for (int i = slotList.Count; i < (width * height); i++)
    //        {
    //            GameObject Slot = (GameObject)Instantiate(prefabSlot);
    //            Slot.name = (slotList.Count + 1).ToString();
    //            Slot.transform.SetParent(SlotContainer.transform);
    //            slotList.Add(Slot);
    //        }
    //    }

    //    if (itemsToMove != null && ItemsInInventory.Count < width * height)
    //    {
    //        foreach (Item i in itemsToMove)
    //        {
    //            addItemToInventory(i.id);
    //        }
    //    }

    //    setImportantVariables();
    //}

    //public void updateSlotAmount()
    //{

    //    if (prefabSlot == null)
    //        prefabSlot = Resources.Load("Prefabs/Slot - Inventory") as GameObject;

    //    if (SlotContainer == null)
    //    {
    //        SlotContainer = (GameObject)Instantiate(prefabSlotContainer);
    //        SlotContainer.transform.SetParent(PanelRectTransform.transform);
    //        SlotContainerRectTransform = SlotContainer.GetComponent<RectTransform>();
    //        SlotGridRectTransform = SlotContainer.GetComponent<RectTransform>();
    //        SlotGridLayout = SlotContainer.GetComponent<GridLayoutGroup>();
    //    }

    //    if (SlotContainerRectTransform == null)
    //        SlotContainerRectTransform = SlotContainer.GetComponent<RectTransform>();
    //    SlotContainerRectTransform.localPosition = Vector3.zero;

    //    List<Item> itemsToMove = new List<Item>();
    //    List<GameObject> slotList = new List<GameObject>();
    //    foreach (Transform child in SlotContainer.transform)
    //    {
    //        if (child.tag == "Slot") { slotList.Add(child.gameObject); }
    //    }

    //    while (slotList.Count > width * height)
    //    {
    //        GameObject go = slotList[slotList.Count - 1];
    //        ItemOnObject itemInSlot = go.GetComponentInChildren<ItemOnObject>();
    //        if (itemInSlot != null)
    //        {
    //            itemsToMove.Add(itemInSlot.item);
    //            ItemsInInventory.Remove(itemInSlot.item);
    //        }
    //        slotList.Remove(go);
    //        DestroyImmediate(go);
    //    }

    //    if (slotList.Count < width * height)
    //    {
    //        for (int i = slotList.Count; i < (width * height); i++)
    //        {
    //            GameObject Slot = (GameObject)Instantiate(prefabSlot);
    //            Slot.name = (slotList.Count + 1).ToString();
    //            Slot.transform.SetParent(SlotContainer.transform);
    //            slotList.Add(Slot);
    //        }
    //    }

    //    if (itemsToMove != null && ItemsInInventory.Count < width * height)
    //    {
    //        foreach (Item i in itemsToMove)
    //        {
    //            addItemToInventory(i.id);
    //        }
    //    }

    //    setImportantVariables();
    //}

  
   

    void updateItemSize()
    {
        for (int i = 0; i < SlotContainer.transform.childCount; i++)
        {
            if (SlotContainer.transform.GetChild(i).childCount > 0)
            {
                SlotContainer.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(slotSize, slotSize);
                SlotContainer.transform.GetChild(i).GetChild(0).GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(slotSize, slotSize);
            }

        }
    }

    public bool checkIfItemAllreadyExist(int itemID, int itemValue)
    {
        updateItemList();
        int stack;
        for (int i = 0; i < ItemsInInventory.Count; i++)
        {
            if (ItemsInInventory[i].id == itemID)
            {
                stack = ItemsInInventory[i].quantity + itemValue;
                if (stack <= ItemsInInventory[i].maxInStack)
                {
                    ItemsInInventory[i].quantity = stack;
                    GameObject temp = getItemGameObject(ItemsInInventory[i]);
                    if (temp != null && temp.GetComponent<ConsumeItem>().duplication != null)
                        temp.GetComponent<ConsumeItem>().duplication.GetComponent<ItemOnObject>().item.quantity = stack;
                    return true;
                }
            }
        }
        return false;
    }

    public void addItemToInventory(int id)
    {
        for (int i = 0; i < SlotContainer.transform.childCount; i++)
        {
            if (SlotContainer.transform.GetChild(i).childCount == 0)
            {
                GameObject item = (GameObject)Instantiate(prefabItem);
                item.GetComponent<ItemOnObject>().item = itemDatabase.getItemByID(id);
                item.transform.SetParent(SlotContainer.transform.GetChild(i));
                item.GetComponent<RectTransform>().localPosition = Vector3.zero;
                item.transform.GetChild(0).GetComponent<Image>().sprite = item.GetComponent<ItemOnObject>().item.itemIcon;
                item.GetComponent<ItemOnObject>().item.indexItemInList = ItemsInInventory.Count - 1;
                break;
            }
        }

        stackableSettings();
        updateItemList();

    }

    public GameObject addItemToInventory(int id, int value)
    {
        for (int i = 0; i < SlotContainer.transform.childCount; i++)
        {
            if (SlotContainer.transform.GetChild(i).childCount == 0)
            {
                GameObject item = (GameObject)Instantiate(prefabItem);
                ItemOnObject itemOnObject = item.GetComponent<ItemOnObject>();
                itemOnObject.item = itemDatabase.getItemByID(id);
                if (itemOnObject.item.quantity <= itemOnObject.item.maxInStack && value <= itemOnObject.item.maxInStack)
                    itemOnObject.item.quantity = value;
                else
                    itemOnObject.item.quantity = 1;
                item.transform.SetParent(SlotContainer.transform.GetChild(i));
                item.GetComponent<RectTransform>().localPosition = Vector3.zero;
                item.transform.GetChild(0).GetComponent<Image>().sprite = itemOnObject.item.itemIcon;
                itemOnObject.item.indexItemInList = ItemsInInventory.Count - 1;
                //if (inputManagerDatabase == null)
                //    inputManagerDatabase = (InputManager)Resources.Load("InputManager");
                return item;
            }
        }

        stackableSettings();
        updateItemList();
        return null;

    }

    public void addItemToInventoryStorage(int itemID, int value)
    {

        for (int i = 0; i < SlotContainer.transform.childCount; i++)
        {
            if (SlotContainer.transform.GetChild(i).childCount == 0)
            {
                GameObject item = (GameObject)Instantiate(prefabItem);
                ItemOnObject itemOnObject = item.GetComponent<ItemOnObject>();
                itemOnObject.item = itemDatabase.getItemByID(itemID);
                if (itemOnObject.item.quantity < itemOnObject.item.maxInStack && value <= itemOnObject.item.maxInStack)
                    itemOnObject.item.quantity = value;
                else
                    itemOnObject.item.quantity = 1;
                item.transform.SetParent(SlotContainer.transform.GetChild(i));
                item.GetComponent<RectTransform>().localPosition = Vector3.zero;
                itemOnObject.item.indexItemInList = 999;
                //if (inputManagerDatabase == null)
                //    inputManagerDatabase = (InputManager)Resources.Load("InputManager");
                updateItemSize();
                stackableSettings();
                break;
            }
        }
        stackableSettings();
        updateItemList();
    }

    public void stackableSettings(bool stackable, Vector3 posi)
    {
        for (int i = 0; i < SlotContainer.transform.childCount; i++)
        {
            if (SlotContainer.transform.GetChild(i).childCount > 0)
            {
                ItemOnObject item = SlotContainer.transform.GetChild(i).GetChild(0).GetComponent<ItemOnObject>();
                if (item.item.maxInStack > 1)
                {
                    RectTransform textRectTransform = SlotContainer.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<RectTransform>();
                    Text text = SlotContainer.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<Text>();
                    text.text = "" + item.item.quantity;
                    text.enabled = stackable;
                    textRectTransform.localPosition = posi;
                }
            }
        }
    }


    public void deleteAllItems()
    {
        for (int i = 0; i < SlotContainer.transform.childCount; i++)
        {
            if (SlotContainer.transform.GetChild(i).childCount != 0)
            {
                Destroy(SlotContainer.transform.GetChild(i).GetChild(0).gameObject);
            }
        }
    }

    public List<Item> getItemList()
    {
        List<Item> theList = new List<Item>();
        for (int i = 0; i < SlotContainer.transform.childCount; i++)
        {
            if (SlotContainer.transform.GetChild(i).childCount != 0)
                theList.Add(SlotContainer.transform.GetChild(i).GetChild(0).GetComponent<ItemOnObject>().item);
        }
        return theList;
    }

    public void stackableSettings()
    {
        for (int i = 0; i < SlotContainer.transform.childCount; i++)
        {
            if (SlotContainer.transform.GetChild(i).childCount > 0)
            {
                ItemOnObject item = SlotContainer.transform.GetChild(i).GetChild(0).GetComponent<ItemOnObject>();
                if (item.item.maxInStack > 1)
                {
                    RectTransform textRectTransform = SlotContainer.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<RectTransform>();
                    Text text = SlotContainer.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<Text>();
                    text.text = "" + item.item.quantity;
                    text.enabled = stackable;
                    textRectTransform.localPosition = new Vector3(positionNumberX, positionNumberY, 0);
                }
                else
                {
                    Text text = SlotContainer.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<Text>();
                    text.enabled = false;
                }
            }
        }

    }

    public GameObject getItemGameObjectByName(Item item)
    {
        for (int k = 0; k < SlotContainer.transform.childCount; k++)
        {
            if (SlotContainer.transform.GetChild(k).childCount != 0)
            {
                GameObject itemGameObject = SlotContainer.transform.GetChild(k).GetChild(0).gameObject;
                Item itemObject = itemGameObject.GetComponent<ItemOnObject>().item;
                if (itemObject.name.Equals(item.name))
                {
                    return itemGameObject;
                }
            }
        }
        return null;
    }

    public GameObject getItemGameObject(Item item)
    {
        for (int k = 0; k < SlotContainer.transform.childCount; k++)
        {
            if (SlotContainer.transform.GetChild(k).childCount != 0)
            {
                GameObject itemGameObject = SlotContainer.transform.GetChild(k).GetChild(0).gameObject;
                Item itemObject = itemGameObject.GetComponent<ItemOnObject>().item;
                if (itemObject.Equals(item))
                {
                    return itemGameObject;
                }
            }
        }
        return null;
    }

    public void deleteItem(Item item)
    {
        for (int i = 0; i < ItemsInInventory.Count; i++)
        {
            if (item.Equals(ItemsInInventory[i]))
                ItemsInInventory.RemoveAt(item.indexItemInList);
        }
    }

    

    public void deleteItemFromInventory(Item item)
    {
        for (int i = 0; i < ItemsInInventory.Count; i++)
        {
            if (item.Equals(ItemsInInventory[i]))
                ItemsInInventory.RemoveAt(i);
        }
    }

    public void deleteItemFromInventoryWithGameObject(Item item)
    {
        for (int i = 0; i < ItemsInInventory.Count; i++)
        {
            if (item.Equals(ItemsInInventory[i]))
            {
                ItemsInInventory.RemoveAt(i);
            }
        }

        for (int k = 0; k < SlotContainer.transform.childCount; k++)
        {
            if (SlotContainer.transform.GetChild(k).childCount != 0)
            {
                GameObject itemGameObject = SlotContainer.transform.GetChild(k).GetChild(0).gameObject;
                Item itemObject = itemGameObject.GetComponent<ItemOnObject>().item;
                if (itemObject.Equals(item))
                {
                    Destroy(itemGameObject);
                    break;
                }
            }
        }
    }

    public int getPositionOfItem(Item item)
    {
        for (int i = 0; i < SlotContainer.transform.childCount; i++)
        {
            if (SlotContainer.transform.GetChild(i).childCount != 0)
            {
                Item item2 = SlotContainer.transform.GetChild(i).GetChild(0).GetComponent<ItemOnObject>().item;
                if (item.Equals(item2))
                    return i;
            }
        }
        return -1;
    }

    public void addItemToInventory(int ignoreSlot, int itemID, int itemValue)
    {

        for (int i = 0; i < SlotContainer.transform.childCount; i++)
        {
            if (SlotContainer.transform.GetChild(i).childCount == 0 && i != ignoreSlot)
            {
                GameObject item = (GameObject)Instantiate(prefabItem);
                ItemOnObject itemOnObject = item.GetComponent<ItemOnObject>();
                itemOnObject.item = itemDatabase.getItemByID(itemID);
                if (itemOnObject.item.quantity < itemOnObject.item.maxInStack && itemValue <= itemOnObject.item.maxInStack)
                    itemOnObject.item.quantity = itemValue;
                else
                    itemOnObject.item.quantity = 1;
                item.transform.SetParent(SlotContainer.transform.GetChild(i));
                item.GetComponent<RectTransform>().localPosition = Vector3.zero;
                itemOnObject.item.indexItemInList = 999;
                updateItemSize();
                stackableSettings();
                break;
            }
        }
        stackableSettings();
        updateItemList();
    }




    public void updateItemIndex()
    {
        for (int i = 0; i < ItemsInInventory.Count; i++)
        {
            ItemsInInventory[i].indexItemInList = i;
        }
    }
}
