using UnityEngine;
using System.Collections;

[System.Serializable]
public class AttributeItem
{

    public AtributeTypeItem attribute;
    public float value;
    public AttributeItem(AtributeTypeItem attribute, float value)
    {
        this.attribute = attribute;
        this.value = value;
    }

    public AttributeItem() { }

}

