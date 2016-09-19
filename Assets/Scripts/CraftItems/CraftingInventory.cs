using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CraftingInventory : MonoBehaviour {

    private bool isActive;

    private BagInventory playerBag;

    private Transform slotContainerTran;
    private Transform slotResultTran;
    private Text nameResult;
    private Text chanceResult;

    private RecipeCraft currRecipe;
    private Item resultItem;
    private ItemOther recipeItem;
    private List<RecipeCraft.Ingredient> ingridients;

    private Color redSlotColor;
    private Color greenSlotColor;
    private GameObject itemCraftPrefab;

    public Inventory.TypeParentInv typeParentInv;

    void Start()
    {
        typeParentInv = Inventory.TypeParentInv.craft;

        itemCraftPrefab = Resources.Load("Prefabs/Inventory/ItemCraft") as GameObject;

        Transform currTransform = transform;
        slotResultTran = currTransform.FindChild("ResultSlot");
        nameResult = currTransform.FindChild("Name").GetComponent<Text>();
        chanceResult = currTransform.FindChild("Chance").GetComponent<Text>();
        slotContainerTran = currTransform.FindChild("Slots");

        redSlotColor = new Color32(255, 70, 70, 50);
        greenSlotColor = new Color32(70, 255, 70, 50);

        isActive = false;
        gameObject.SetActive(false);
	}

    public bool IsActive()
    {
        return isActive;
    }

    public void OpenInventory(ItemOther recipeItem)
    {
        if (recipeItem.itemOtherType != ItemOtherType.Recipe)
            return;

        if (isActive)
        {
            DestroyItemsObj();
        }
        else
        {
            gameObject.SetActive(true);
            isActive = true;
        }

        this.recipeItem = recipeItem;

        if (playerBag == null)
            playerBag = GameObject.FindWithTag("InvBag").GetComponent<BagInventory>();

        XMLStorageRecipes storageRecipes = new XMLStorageRecipes();
        XmlStorageItem storagetItem = new XmlStorageItem();

        currRecipe = storageRecipes.GetRecipeByRecipeId(recipeItem.RecipeId);

        if (resultItem == null || resultItem.id != currRecipe.CraftResultID)
            resultItem = storagetItem.GetItemById(currRecipe.CraftResultID);

        ingridients = currRecipe.Ingredients;
        AddResultItemInObj(resultItem);

        for (int i = 0; i < ingridients.Count; i++)
        {
            Item tempItem = playerBag.GetItemById(ingridients[i].id, true);
            ingridients[i].item = tempItem;
            AddItemInObj(ingridients[i], i);
        }

        nameResult.text = resultItem.name;
        chanceResult.text = currRecipe.Сhance + "%";

    }

    public void CloseInventory()
    {
        DestroyItemsObj();
        gameObject.SetActive(false);
        isActive = false;
    }

    public void CreateItem()
    {
        Debug.Log("Create Item");
        if (!playerBag.CheckFreeSlot(1))
        {
            // Return Error
            return;
        }
        Debug.Log("Create Item2");
        foreach (RecipeCraft.Ingredient ingridient in ingridients)
            if (!ingridient.IsEnough())
            {
                // Return Error
                return;
            }
        Debug.Log("Create Item3");
        foreach (RecipeCraft.Ingredient ingridient in ingridients)
        {
            playerBag.DeleteItemFromInventory(ingridient.id, ingridient.requiredNum);
        }
        Debug.Log("Create Item4");
        playerBag.DeleteItemFromInventory(recipeItem.id, 1); // Fixing this: delete 1 recipe not stack

        float randomChance = Random.value;

        bool crafting = randomChance < currRecipe.Сhance / 100.0f;

        if (crafting)
        {
            playerBag.AddItemToInventory(resultItem as ItemEquip);
        }
        // else return Fail message

        CheckAllIngridients();

    }



    private void DestroyItemsObj()
    {
        for (int i = 0; i < currRecipe.Ingredients.Count; i++)
            Destroy(slotContainerTran.GetChild(i).GetChild(0).gameObject);
        Destroy(slotResultTran.GetChild(0).gameObject);
    }

    private void CheckAllIngridients()
    {
        for (int i = 0; i < ingridients.Count; i++)
        {
            Transform itemTran = slotContainerTran.GetChild(i);
            if (ingridients[i].IsEnough())
                itemTran.GetChild(0).GetChild(1).GetComponent<Image>().color = greenSlotColor; //Child id 1 - Availability
            else
                itemTran.GetChild(0).GetChild(1).GetComponent<Image>().color = redSlotColor; //Child id 1 - Availability

            itemTran.GetChild(0).GetComponent<ItemOnObject>().SetText(ingridients[i].GetQuantityText());
        }
    }

    private void AddItemInObj(RecipeCraft.Ingredient ingridient, int index)
    {
        ingridient.item.LoadResources();
        GameObject itemObj = (GameObject)Instantiate(itemCraftPrefab, slotContainerTran.GetChild(index), false);
        ItemOnObject _ItemOnObject = itemObj.GetComponent<ItemOnObject>();
        _ItemOnObject.Item = ingridient.item;
        _ItemOnObject.GetTypeParentinventory = typeParentInv;
        _ItemOnObject.UpdateItem();

        if (ingridient.IsEnough())
            itemObj.transform.GetChild(1).GetComponent<Image>().color = greenSlotColor; //Child id 1 - Availability
        else
            itemObj.transform.GetChild(1).GetComponent<Image>().color = redSlotColor; //Child id 1 - Availability

        _ItemOnObject.SetText(ingridient.GetQuantityText());
    }

    private void AddResultItemInObj(Item tempItem)
    {
        tempItem.LoadResources();
        GameObject itemObj = (GameObject)Instantiate(itemCraftPrefab, slotResultTran, false);
        ItemOnObject _ItemOnObject = itemObj.GetComponent<ItemOnObject>();
        _ItemOnObject.Item = tempItem;
        _ItemOnObject.GetTypeParentinventory = typeParentInv;
        _ItemOnObject.UpdateItem();
    }

}
