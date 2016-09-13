using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class InventoryClose : MonoBehaviour, IPointerDownHandler
{

    InventoryCustom inv;
    void Start()
    {
        inv = transform.parent.GetComponent<InventoryCustom>();

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            inv.CloseInventory();
        }
    }
}
