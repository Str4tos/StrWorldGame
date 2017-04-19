using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ClickOnItem : MonoBehaviour, IPointerDownHandler
{
    private Item item;
    private ItemOnObject _ItemOnObject;
    
    private RectTransform canvasRectTransform;                    //the panel(Inventory Background) RectTransform
    private RectTransform tooltipRectTransform;                  //the tooltip RectTransform
    private RectTransform slotRectTransform;
    private Canvas mainCanvas;

    // Use this for initialization
    void Start()
    {
        tooltipRectTransform = PlayerInterface.Instance.tooltipItem.GetComponent<RectTransform>();
        slotRectTransform = transform.parent.GetComponent<RectTransform>();
        mainCanvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
        canvasRectTransform = mainCanvas.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerDown(PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Right)
        {
            PlayerInterface.Instance.HideTooltip();

            _ItemOnObject = GetComponent<ItemOnObject>();
            item = _ItemOnObject.Item;

            switch (_ItemOnObject.GetTypeParentinventory)
            {
                case Inventory.TypeParentInv.bag:
                    BagUsage();
                    break;

                case Inventory.TypeParentInv.equip:

                    if (PlayerInterface.Instance.bagInv.AddItemToInventory(PlayerInterface.Instance.characterInv.GetEquipItem(item)))
                        PlayerInterface.Instance.characterInv.UnEquip(item);
                    break;

                case Inventory.TypeParentInv.storage:
                    StorageUsage();
                    break;

                case Inventory.TypeParentInv.shop:
                    // Add methods for shop
                    break;

                case Inventory.TypeParentInv.hotbar:
                    if (item.itemType == ItemType.Consume)
                    {
                        // Add methods for usage hotbar
                    }
                    if (item.itemType == ItemType.Equip)
                    {
                        // Add methods for usage hotbar
                    }

                    break;
            }
            item = null;
            _ItemOnObject = null;

        }
        if (data.button == PointerEventData.InputButton.Left)
        {
            PlayerInterface.Instance.HideTooltip();
            _ItemOnObject = GetComponent<ItemOnObject>();
            item = _ItemOnObject.Item;
            ActivateTooltip(data);
            item = null;
            _ItemOnObject = null;
        }

    }


    private void BagUsage()
    {
        if (PlayerInterface.Instance.storageInv.IsActive())
        {
            switch (item.itemType)
            {
                case ItemType.Equip:
                    ItemEquip tempEquip = PlayerInterface.Instance.bagInv.GetEquipItem(item);
                    if (tempEquip != null && PlayerInterface.Instance.storageInv.AddItemToInventory(tempEquip))
                        PlayerInterface.Instance.bagInv.DelItemFromInventory(tempEquip);
                    return;
                case ItemType.Consume:
                    ItemConsume tempConsume = PlayerInterface.Instance.bagInv.GetConsumeItem(item);
                    if (tempConsume != null && PlayerInterface.Instance.storageInv.AddItemToInventory(tempConsume))
                        PlayerInterface.Instance.bagInv.DelItemFromInventory(tempConsume);
                    return;
                case ItemType.Other:
                    ItemOther tempOther = PlayerInterface.Instance.bagInv.GetOtherItem(item);
                    if (tempOther != null && PlayerInterface.Instance.storageInv.AddItemToInventory(tempOther))
                        PlayerInterface.Instance.bagInv.DelItemFromInventory(tempOther);
                    return;
            }
        }
        if (PlayerInterface.Instance.shopInv.IsActive())
        {
            // Add methods for shop
            return;
        }
        //if(Inventory Crafting.isActive()) 
        // Add methods for Crafting inventory

        if (item.itemType == ItemType.Equip)
        {
            ItemEquip itemForEquip = PlayerInterface.Instance.bagInv.GetEquipItem(item);
            ItemEquip itemForBag = PlayerInterface.Instance.characterInv.Equip(itemForEquip);
            PlayerInterface.Instance.bagInv.DelItemFromInventory(itemForEquip);
            if (itemForBag != null)
                PlayerInterface.Instance.bagInv.AddItemToInventory(itemForBag, itemForBag.indexItemInList, false);

            return;
        }
        if (item.itemType == ItemType.Consume)
        {
            // Add methods for usage consume items
        }
        if (item.itemType == ItemType.Other)
        {
            PlayerInterface.Instance.craftInv.OpenInventory(item as ItemOther);
        }
    }
    private void StorageUsage()
    {
        switch (item.itemType)
        {
            case ItemType.Equip:
                ItemEquip tempEquip = PlayerInterface.Instance.storageInv.GetEquipItem(item);
                if (tempEquip != null && PlayerInterface.Instance.storageInv.AddItemToInventory(tempEquip))
                    PlayerInterface.Instance.storageInv.DelItemFromInventory(tempEquip);
                return;
            case ItemType.Consume:
                ItemConsume tempConsume = PlayerInterface.Instance.storageInv.GetConsumeItem(item);
                if (tempConsume != null && PlayerInterface.Instance.bagInv.AddItemToInventory(tempConsume))
                    PlayerInterface.Instance.storageInv.DelItemFromInventory(tempConsume);
                return;
            case ItemType.Other:
                ItemOther tempOther = PlayerInterface.Instance.storageInv.GetOtherItem(item);
                if (tempOther != null && PlayerInterface.Instance.bagInv.AddItemToInventory(tempOther))
                    PlayerInterface.Instance.storageInv.DelItemFromInventory(tempOther);
                return;
        }
    }
    private void ActivateTooltip(PointerEventData data)
    {
        if (_ItemOnObject.GetTypeParentinventory == Inventory.TypeParentInv.bag
            && item.itemType == ItemType.Equip)
        {
            ItemEquip tempEquip = PlayerInterface.Instance.bagInv.GetEquipItem(item);
            PlayerInterface.Instance.tooltipItem.ShowTooltip(tempEquip, PlayerInterface.Instance.characterInv.GetEquipItem(tempEquip.itemEquipType));
        }
        else if (item.itemType == ItemType.Equip)
            PlayerInterface.Instance.tooltipItem.ShowTooltip(PlayerInterface.Instance.bagInv.GetEquipItem(item));
        else if (item.itemType == ItemType.Consume)
            PlayerInterface.Instance.tooltipItem.ShowTooltip(PlayerInterface.Instance.bagInv.GetConsumeItem(item));
        else if (item.itemType == ItemType.Other)
            PlayerInterface.Instance.tooltipItem.ShowTooltip(PlayerInterface.Instance.bagInv.GetOtherItem(item));

        Vector3[] slotCorners = new Vector3[4];                     //get the corners of the slot
        GetComponent<RectTransform>().GetWorldCorners(slotCorners); //get the corners of the slot                

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, slotCorners[2], data.pressEventCamera, out localPointerPosition))   // and set the localposition of the tooltip...
        {
            float positionY = localPointerPosition.y;
            float restHeigh = positionY - PlayerInterface.Instance.tooltipItem.WindowHeigh;
            if (restHeigh < canvasRectTransform.rect.yMin)
            {
                restHeigh -= canvasRectTransform.rect.yMin;
                positionY -= restHeigh;
            }
            tooltipRectTransform.localPosition = new Vector3(localPointerPosition.x, positionY);
        }

    }
}
