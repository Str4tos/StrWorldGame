using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemAttribute
{

    public ItemAtributeType attribute;
    public float value;
    public ItemAttribute(ItemAtributeType attribute, float value)
    {
        this.attribute = attribute;
        this.value = value;
    }

    public ItemAttribute() { }

}

