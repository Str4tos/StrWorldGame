using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemAttributePercentList : ScriptableObject
{
    [SerializeField]
    public List<ItemAttributePercent> itemAttributeList = new List<ItemAttributePercent>();

}
