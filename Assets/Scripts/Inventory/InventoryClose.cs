using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class InventoryClose : MonoBehaviour, IPointerClickHandler
{

    InventoryCustom inv;
    void Start()
    {
        inv = transform.parent.GetComponent<InventoryCustom>();

    }
		
	public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            inv.CloseInventory();
        }
    }
}
