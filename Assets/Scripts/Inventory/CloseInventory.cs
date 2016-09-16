using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CloseInventory : MonoBehaviour, IPointerClickHandler
{

    Inventory inv;
    void Start()
    {
        inv = transform.parent.GetComponent<Inventory>();

    }
		
	public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            inv.CloseInventory();
        }
    }
}
