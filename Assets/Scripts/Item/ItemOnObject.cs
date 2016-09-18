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
    private Transform currTransform;

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
        if (currTransform == null)
            currTransform = transform;
		if(image == null)
            image = currTransform.GetChild(0).GetComponent<Image>();
		if(quantity == null)
            quantity = currTransform.FindChild("Text").GetComponent<Text>();
        image.sprite = item.iconSprite;
        if (item.itemType != ItemType.Equip && item.quantity > 1)
            quantity.text = "" + item.quantity;
		
    }

    public void SetText(string text)
    {
        quantity.text = text;
    }
}
