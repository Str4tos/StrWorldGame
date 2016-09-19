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
                    if (_PlayerGui.InvBag.AddItemToInventory(item))
                    _PlayerGui.InvStorage.DeleteItemSlotFromInventory(item);
                    break;

                case Inventory.TypeParentInv.shop:
                    //---Soon
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
               if (_PlayerGui.InvStorage.AddItemToInventory(item))
                        _PlayerGui.InvBag.DeleteItemSlotFromInventory(item);
        }
        if (_PlayerGui.InvShop.IsActive())
        {
            // Add methods for shop In progress
            return;
        }
        //---Soon
        //if(Inventory Crafting.isActive()) 
        // Add methods for Crafting inventory

        if (item.itemType == ItemType.Equip)
        {
            ItemEquip itemForBag = _PlayerGui.InvCharacter.Equip(item as ItemEquip);
            _PlayerGui.InvBag.DeleteItemSlotFromInventory(item);
            if (itemForBag != null)
                _PlayerGui.InvBag.AddItemToInventory(itemForBag, itemForBag.indexItemInList, false, false);
            return;
        }
        if (item.itemType == ItemType.Consume)
        {
            //---Soon
            // Add methods for usage consume items
        }
        if (item.itemType == ItemType.Other)
        {
            _PlayerGui.InvCrafting.OpenInventory(item as ItemOther);
        }
    }
    private void ActivateTooltip(PointerEventData data)
    {
        if (_ItemOnObject.GetTypeParentinventory == Inventory.TypeParentInv.bag
            && item.itemType == ItemType.Equip)
        {
            ItemEquip tempItemEquip = item as ItemEquip;
            tooltip.ShowTooltip(item as ItemEquip, _PlayerGui.InvCharacter.GetEquipItem(tempItemEquip.itemEquipType));
        }
        else if (item.itemType == ItemType.Equip)
            tooltip.ShowTooltip(item as ItemEquip);
        else if (item.itemType == ItemType.Consume)
            tooltip.ShowTooltip(item as ItemConsume);
        else if (item.itemType == ItemType.Other)
            tooltip.ShowTooltip(item as ItemOther);

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
