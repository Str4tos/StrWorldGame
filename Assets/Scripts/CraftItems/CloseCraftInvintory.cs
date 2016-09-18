using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CloseCraftInvintory : MonoBehaviour, IPointerClickHandler
{

    CraftingInventory inv;
    void Start()
    {
        inv = transform.parent.parent.GetComponent<CraftingInventory>();

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            inv.CloseInventory();
        }
    }
}
