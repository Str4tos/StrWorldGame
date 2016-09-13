using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TooltipCustom : MonoBehaviour
{
	public bool isActive = false;

	private GameObject prefabText;

    private RectTransform _RectTransform;
    private float windowHeigh;


    // Use this for initialization
    void Start()
    {
        prefabText = Resources.Load("Prefabs/GUI/ItemText") as GameObject;
        _RectTransform = GetComponent<RectTransform>();
        windowHeigh = _RectTransform.sizeDelta.y;
    }


    public void ShowTooltip(ItemEquip itemOnSlot, ItemEquip equipedItem)
    {
        ShowTooltip(itemOnSlot);
        if (!equipedItem.IsEmpty())
            return;
        
        CreateNewText("if you replace this item, the following stat changes will occur:");
        windowHeigh += 20.0f;
        
        foreach (ItemAttribute onSlotAttr in itemOnSlot.itemAttributes)
            foreach (ItemAttribute equipedAttr in equipedItem.itemAttributes)
                if (onSlotAttr.attribute == equipedAttr.attribute)
                    if (onSlotAttr.value < equipedAttr.value)
                    {
                        CreateNewText(" -" + (equipedAttr.value - onSlotAttr.value) + " " + onSlotAttr.attribute.ToString());
                        break;
                    }
                    else if (onSlotAttr.value > equipedAttr.value)
                    {
                        CreateNewText(" +" + (onSlotAttr.value - equipedAttr.value) + " " + onSlotAttr.attribute.ToString());
                        break;
                    }
        UpdateHeigh();

    }
    public void ShowTooltip(ItemEquip itemOnSlot)
    {
		isActive = true;
        gameObject.SetActive(true);
        //Name + enchant
        string tempString = itemOnSlot.name;
        if (itemOnSlot.enchantLevel > 0)
            tempString += " +" + itemOnSlot.enchantLevel;
        CreateNewText(tempString);
        
        //Type
        CreateNewText(itemOnSlot.itemType.ToString() + ": " + itemOnSlot.itemEquipType.ToString());

        //Attributes
        foreach (ItemAttribute attribute in itemOnSlot.itemAttributes)
            CreateNewText(" +" + attribute.value.ToString("#") + " " + attribute.attribute.ToString());

        //Requires
        if (itemOnSlot.requestLevel > 1)
            CreateNewText("Requires Level " + itemOnSlot.requestLevel);

        if (itemOnSlot.requestStrenght > 0)
            CreateNewText("Requires Strenght " + itemOnSlot.requestStrenght);

        if (itemOnSlot.requestAgility > 0)
            CreateNewText("Requires Agility " + itemOnSlot.requestAgility);

        if (itemOnSlot.requestVitality > 0)
            CreateNewText("Requires Vitality " + itemOnSlot.requestVitality);

        if (itemOnSlot.requestEnergy > 0)
            CreateNewText("Requires Energy " + itemOnSlot.requestEnergy);

        //Description
        if (itemOnSlot.description != "")
            CreateNewText(itemOnSlot.description);

        //Sell price
        if (itemOnSlot.salePrice > 0)
            CreateNewText("Sell price: " + itemOnSlot.salePrice);

        UpdateHeigh();
    }
    public void ShowTooltip(ItemConsume itemOnSlot)
    {
		isActive = true;
        gameObject.SetActive(true);
        //Name
        CreateNewText(itemOnSlot.name);

        //Type
        CreateNewText(itemOnSlot.itemType.ToString());

        //Description
        if (itemOnSlot.description != "")
            CreateNewText("Use: " + itemOnSlot.description);

        //Sell price
        if (itemOnSlot.salePrice > 0)
            CreateNewText("Sell price: " + (itemOnSlot.salePrice * itemOnSlot.quantity));

        UpdateHeigh();
    }
    public void ShowTooltip(ItemOther itemOnSlot)
    {
		isActive = true;
        gameObject.SetActive(true);
        //Name
        CreateNewText(itemOnSlot.name);

        //Type
        CreateNewText(itemOnSlot.itemOtherType.ToString());

        //Description
        if (itemOnSlot.description != "")
            CreateNewText("Use: " + itemOnSlot.description);

        //Sell price
        if (itemOnSlot.salePrice > 0)
            CreateNewText("Sell price: " + (itemOnSlot.salePrice * itemOnSlot.quantity));

        UpdateHeigh();
    }
    


    public void HideToolTip()
    {
		isActive = false;
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        windowHeigh = 3.0f;
        UpdateHeigh();
        gameObject.SetActive(false);
    }

    public float WindowHeigh { get { return windowHeigh; } }

    private void CreateNewText(string text)
    {
        GameObject result = Instantiate(prefabText, transform) as GameObject;
        Text textResult = result.GetComponent<Text>();
        textResult.text = text;
        windowHeigh += 20.0f;
    }
    private void UpdateHeigh()
    {
        _RectTransform.sizeDelta = new Vector2(_RectTransform.sizeDelta.x, windowHeigh);
    }

}
