using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class EquipmentSystem : MonoBehaviour
{
    [SerializeField]
    public int slotsInTotal;
    [SerializeField]
    public ItemType[] itemTypeOfSlots = new ItemType[12];

    void Start()
    {
        ConsumeItem.eS = GetComponent<EquipmentSystem>();
    }

}

