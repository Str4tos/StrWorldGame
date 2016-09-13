using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemOnObjectCustom : MonoBehaviour
{

    private ItemCustom item;
    private Text quantity;                                      
    private Image image;
    private InventoryCustom.TypeParentInv typeParentinventory;

    public ItemCustom Item
    {
        get { return item; }
        set { item = value; }
    }

    public InventoryCustom.TypeParentInv GetTypeParentinventory
    {
        get { return typeParentinventory; }
        set { typeParentinventory = value; }
    }

    public void UpdateItem()
    {
		if(image == null)
			image = transform.GetChild(0).GetComponent<Image>();
		if(quantity == null)
			quantity = transform.GetChild(1).GetComponent<Text>();
        image.sprite = item.iconSprite;
        if (item.itemType != ItemTypeCustom.Equip && item.quantity > 1)
            quantity.text = "" + item.quantity;
		else
			quantity.text = "";
    }
}
