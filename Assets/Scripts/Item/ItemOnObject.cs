using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemOnObject : MonoBehaviour
{

    private Item item;
    private Text quantity;                                      
    private Image image;
    private Inventory.TypeParentInv typeParentinventory;

    public Item Item
    {
        get { return item; }
        set { item = value; }
    }

    public Inventory.TypeParentInv GetTypeParentinventory
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
        if (item.itemType != ItemType.Equip && item.quantity > 1)
            quantity.text = "" + item.quantity;
		else
			quantity.text = "";
    }
}
