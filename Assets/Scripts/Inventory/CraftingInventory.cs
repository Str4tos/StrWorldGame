using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CraftingInventory : Inventory {


    protected override void Start()
    {
        Initiate();
        base.Start();
        
        typeParentInv = TypeParentInv.storage;
	}

    public void OpenCraftInventory(ItemOther recipeItem)
    {


        OpenInventory();
    }

	void Update () {
	
	}
}
