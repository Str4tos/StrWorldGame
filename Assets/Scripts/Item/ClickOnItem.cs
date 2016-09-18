using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ClickOnItem : MonoBehaviour, IPointerDownHandler
{
    private Item item;
    private ItemOnObject _ItemOnObject;

    private GuiPlayer _PlayerGui;
    private Tooltip tooltip;                                //the tooltip as a GameObject
    private RectTransform canvasRectTransform;                    //the panel(Inventory Background) RectTransform
    private RectTransform tooltipRectTransform;                  //the tooltip RectTransform
    private RectTransform slotRectTransform;
    private Canvas mainCanvas;

    // Use this for initialization
    void Start()
    {
        _PlayerGui = GameObject.FindWithTag("Player").GetComponent<GuiPlayer>();
        tooltip = _PlayerGui.ToolTip;
        tooltipRectTransform = _PlayerGui.ToolTip.gameObject.GetComponent<RectTransform>();
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

            _ItemOnObject = GetComponent<ItemOnObject>();
            item = _ItemOnObject.Item;

            switch (_ItemOnObject.GetTypeParentinventory)
            {
                case Inventory.TypeParentInv.bag:
                    BagUsage();
                    break;

                case Inventory.TypeParentInv.equip:

                    if (_PlayerGui.InvBag.AddItemToInventory(_PlayerGui.InvCharacter.GetEquipItem(item)))
                        _PlayerGui.InvCharacter.UnEquip(item);
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
            if (tooltip == null)
                tooltip = _PlayerGui.ToolTip;
            if (tooltip.isActive)
                tooltip.HideToolTip();
            _ItemOnObject = GetComponent<ItemOnObject>();
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
                case ItemType.Equip:
                    ItemEquip tempEquip = _PlayerGui.InvBag.GetEquipItem(item);
                    if (tempEquip != null && _PlayerGui.InvStorage.AddItemToInventory(tempEquip))
                        _PlayerGui.InvBag.DelItemFromInventory(tempEquip);
                    return;
                case ItemType.Consume:
                    ItemConsume tempConsume = _PlayerGui.InvBag.GetConsumeItem(item);
                    if (tempConsume != null && _PlayerGui.InvStorage.AddItemToInventory(tempConsume))
                        _PlayerGui.InvBag.DelItemFromInventory(tempConsume);
                    return;
                case ItemType.Other:
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

        if (item.itemType == ItemType.Equip)
        {
            ItemEquip itemForEquip = _PlayerGui.InvBag.GetEquipItem(item);
            ItemEquip itemForBag = _PlayerGui.InvCharacter.Equip(itemForEquip);
            _PlayerGui.InvBag.DelItemFromInventory(itemForEquip);
            if (itemForBag != null)
                _PlayerGui.InvBag.AddItemToInventory(itemForBag, itemForBag.indexItemInList, false);

            return;
        }
        if (item.itemType == ItemType.Consume)
        {
            // Add methods for usage consume items
        }
        if (item.itemType == ItemType.Other)
        {
            _PlayerGui.InvCrafting.OpenInventory(item as ItemOther);
        }
    }
    private void StorageUsage()
    {
        switch (item.itemType)
        {
            case ItemType.Equip:
                ItemEquip tempEquip = _PlayerGui.InvStorage.GetEquipItem(item);
                if (tempEquip != null && _PlayerGui.InvBag.AddItemToInventory(tempEquip))
                    _PlayerGui.InvStorage.DelItemFromInventory(tempEquip);
                return;
            case ItemType.Consume:
                ItemConsume tempConsume = _PlayerGui.InvStorage.GetConsumeItem(item);
                if (tempConsume != null && _PlayerGui.InvBag.AddItemToInventory(tempConsume))
                    _PlayerGui.InvStorage.DelItemFromInventory(tempConsume);
                return;
            case ItemType.Other:
                ItemOther tempOther = _PlayerGui.InvStorage.GetOtherItem(item);
                if (tempOther != null && _PlayerGui.InvBag.AddItemToInventory(tempOther))
                    _PlayerGui.InvStorage.DelItemFromInventory(tempOther);
                return;
        }
    }
    private void ActivateTooltip(PointerEventData data)
    {
        if (_ItemOnObject.GetTypeParentinventory == Inventory.TypeParentInv.bag
            && item.itemType == ItemType.Equip)
        {
            ItemEquip tempEquip = _PlayerGui.InvBag.GetEquipItem(item);
            tooltip.ShowTooltip(tempEquip, _PlayerGui.InvCharacter.GetEquipItem(tempEquip.itemEquipType));
        }
        else if (item.itemType == ItemType.Equip)
            tooltip.ShowTooltip(_PlayerGui.InvBag.GetEquipItem(item));
        else if (item.itemType == ItemType.Consume)
            tooltip.ShowTooltip(_PlayerGui.InvBag.GetConsumeItem(item));
        else if (item.itemType == ItemType.Other)
            tooltip.ShowTooltip(_PlayerGui.InvBag.GetOtherItem(item));

        Vector3[] slotCorners = new Vector3[4];                     //get the corners of the slot
        GetComponent<RectTransform>().GetWorldCorners(slotCorners); //get the corners of the slot                

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, slotCorners[2], data.pressEventCamera, out localPointerPosition))   // and set the localposition of the tooltip...
        {
            float positionY = localPointerPosition.y;
            float restHeigh = positionY - tooltip.WindowHeigh;
            if (restHeigh < canvasRectTransform.rect.yMin)
            {
                restHeigh -= canvasRectTransform.rect.yMin;
                positionY -= restHeigh;
            }
            tooltipRectTransform.localPosition = new Vector3(localPointerPosition.x, positionY);
        }

    }
}
