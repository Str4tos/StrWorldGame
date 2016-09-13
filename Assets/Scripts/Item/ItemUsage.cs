using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ItemUsage : MonoBehaviour, IPointerDownHandler
{
    private ItemCustom item;
    private ItemOnObjectCustom _ItemOnObject;

    private PlayerGuiCustom _PlayerGui;
    private TooltipCustom tooltip;                                     //The tooltip script
    private GameObject tooltipGameObject;                        //the tooltip as a GameObject
    private RectTransform canvasRectTransform;                    //the panel(Inventory Background) RectTransform
    private RectTransform tooltipRectTransform;                  //the tooltip RectTransform
    private RectTransform slotRectTransform;
    private Canvas mainCanvas;

    // Use this for initialization
    void Start()
    {
        _PlayerGui = GameObject.FindWithTag("Player").GetComponent<PlayerGuiCustom>();
        tooltipGameObject = GameObject.FindGameObjectWithTag("Tooltip");
        if (tooltipGameObject != null)
        {
            tooltip = tooltipGameObject.GetComponent<TooltipCustom>();
            tooltipRectTransform = tooltipGameObject.GetComponent<RectTransform>();
        }
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
            if (tooltip.isActive)
                tooltip.HideToolTip();

            _ItemOnObject = GetComponent<ItemOnObjectCustom>();
            item = _ItemOnObject.Item;

            switch (_ItemOnObject.GetTypeParentinventory)
            {
                case InventoryCustom.TypeParentInv.bag:
                    BagUsage();
                    break;

                case InventoryCustom.TypeParentInv.equip:

                    if (_PlayerGui.InvBag.AddItemToInventory(_PlayerGui.InvCharacter.GetEquipItem(item)))
                        _PlayerGui.InvCharacter.UnEquip(item);
                    break;

                case InventoryCustom.TypeParentInv.storage:
                    StorageUsage();
                    break;

                case InventoryCustom.TypeParentInv.shop:
                    // Add methods for shop
                    break;

                case InventoryCustom.TypeParentInv.hotbar:
                    if (item.itemType == ItemTypeCustom.Consume)
                    {
                        // Add methods for usage hotbar
                    }
                    if (item.itemType == ItemTypeCustom.Equip)
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
            if (tooltip.isActive)
                tooltip.HideToolTip();
            _ItemOnObject = GetComponent<ItemOnObjectCustom>();
            item = _ItemOnObject.Item;
            ActivateTooltip(data);
            item = null;
            _ItemOnObject = null;
        }

    }


    private void BagUsage()
    {
        if (_PlayerGui.InvStorage.IsActive())
        {
            switch (item.itemType)
            {
                case ItemTypeCustom.Equip:
                    ItemEquip tempEquip = _PlayerGui.InvBag.GetEquipItem(item);
                    if (tempEquip != null && _PlayerGui.InvStorage.AddItemToInventory(tempEquip))
                        _PlayerGui.InvBag.DelItemFromInventory(tempEquip);
                    return;
                case ItemTypeCustom.Consume:
                    ItemConsume tempConsume = _PlayerGui.InvBag.GetConsumeItem(item);
                    if (tempConsume != null && _PlayerGui.InvStorage.AddItemToInventory(tempConsume))
                        _PlayerGui.InvBag.DelItemFromInventory(tempConsume);
                    return;
                case ItemTypeCustom.Other:
                    ItemOther tempOther = _PlayerGui.InvBag.GetOtherItem(item);
                    if (tempOther != null && _PlayerGui.InvStorage.AddItemToInventory(tempOther))
                        _PlayerGui.InvBag.DelItemFromInventory(tempOther);
                    return;
            }
        }
        if (_PlayerGui.InvShop.IsActive())
        {
            // Add methods for shop
            return;
        }
        //if(Inventory Crafting.isActive()) 
        // Add methods for Crafting inventory

        if (item.itemType == ItemTypeCustom.Equip)
        {
            ItemEquip itemForEquip = _PlayerGui.InvBag.GetEquipItem(item);
            ItemEquip itemForBag = _PlayerGui.InvCharacter.Equip(itemForEquip);
            _PlayerGui.InvBag.DelItemFromInventory(itemForEquip);
            if (itemForBag != null)
                _PlayerGui.InvBag.AddItemToInventory(itemForBag, itemForBag.indexItemInList, false);

            return;
        }
        if (item.itemType == ItemTypeCustom.Consume)
        {
            // Add methods for usage consume items
        }
    }
    private void StorageUsage()
    {
        switch (item.itemType)
        {
            case ItemTypeCustom.Equip:
                ItemEquip tempEquip = _PlayerGui.InvStorage.GetEquipItem(item);
                if (tempEquip != null && _PlayerGui.InvBag.AddItemToInventory(tempEquip))
                    _PlayerGui.InvStorage.DelItemFromInventory(tempEquip);
                return;
            case ItemTypeCustom.Consume:
                ItemConsume tempConsume = _PlayerGui.InvStorage.GetConsumeItem(item);
                if (tempConsume != null && _PlayerGui.InvBag.AddItemToInventory(tempConsume))
                    _PlayerGui.InvStorage.DelItemFromInventory(tempConsume);
                return;
            case ItemTypeCustom.Other:
                ItemOther tempOther = _PlayerGui.InvStorage.GetOtherItem(item);
                if (tempOther != null && _PlayerGui.InvBag.AddItemToInventory(tempOther))
                    _PlayerGui.InvStorage.DelItemFromInventory(tempOther);
                return;
        }
    }
    private void ActivateTooltip(PointerEventData data)
    {
        if (_ItemOnObject.GetTypeParentinventory == InventoryCustom.TypeParentInv.bag
            && item.itemType == ItemTypeCustom.Equip)
        {
            ItemEquip tempEquip = _PlayerGui.InvBag.GetEquipItem(item);
            tooltip.ShowTooltip(tempEquip, _PlayerGui.InvCharacter.GetEquipItem(tempEquip.itemEquipType));
        }
        else if (item.itemType == ItemTypeCustom.Equip)
            tooltip.ShowTooltip(_PlayerGui.InvBag.GetEquipItem(item));
        else if (item.itemType == ItemTypeCustom.Consume)
            tooltip.ShowTooltip(_PlayerGui.InvBag.GetConsumeItem(item));
        else if (item.itemType == ItemTypeCustom.Other)
            tooltip.ShowTooltip(_PlayerGui.InvBag.GetOtherItem(item));



        if (tooltip.isActive)
        {
            //            Rect slotRect = RectTransformUtility.PixelAdjustRect(slotRectTransform, mainCanvas);
            //			float yMinSlot = slotRect.yMin;
            //            if (canvasRectTransform.rect.height < slotRect.yMin + tooltip.WindowHeigh)
            //                yMinSlot = canvasRectTransform.rect.height - tooltip.WindowHeigh;
            //            tooltipRectTransform.localPosition = new Vector3(slotRect.xMax, yMinSlot);



            Vector3[] slotCorners = new Vector3[4];                     //get the corners of the slot
            GetComponent<RectTransform>().GetWorldCorners(slotCorners); //get the corners of the slot                

            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, slotCorners[2], data.pressEventCamera, out localPointerPosition))   // and set the localposition of the tooltip...
            {

                //tooltipRectTransform.localPosition = localPointerPosition;          //at the right bottom side of the slot
                float positionY = localPointerPosition.y; // + (slotRectTransform.sizeDelta.y * 0.9f)
                float restHeigh = positionY - tooltip.WindowHeigh;
                if (restHeigh < canvasRectTransform.rect.yMin)
                {
                    restHeigh -= canvasRectTransform.rect.yMin;
                    positionY -= restHeigh;
                }
                tooltipRectTransform.localPosition = new Vector3(localPointerPosition.x, positionY);
                //+tooltip.WindowHeigh
            }

        }



    }
}
