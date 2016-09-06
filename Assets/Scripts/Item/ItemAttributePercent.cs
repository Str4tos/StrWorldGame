using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemAttributePercent
{

    public string attributeName;
    public float attributeValue;
    public ItemAttributePercent(string attributeName, float attributeValue)
    {
        this.attributeName = attributeName;
        this.attributeValue = attributeValue;
    }

    public ItemAttributePercent() { }

}

