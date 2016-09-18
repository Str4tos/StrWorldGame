using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RecipeCraft
{
    public int RecipeID;

    public int CraftResultID;
    public int Сhance;

    public List<Ingredient> Ingredients;

    public RecipeCraft() { }
    public RecipeCraft(int RecipeID, int CraftResultID, int Chance)
    {
        this.RecipeID = RecipeID;
        this.CraftResultID = CraftResultID;
        this.Сhance = Chance;
        Ingredients = new List<Ingredient>();
    }

    public void AddIngridient(int item_id, int number){
        Ingredients.Add(new Ingredient(item_id, number));
    }

    public class Ingredient
    {
        public int id;
        public int requiredNum;
        public Item item;

        public Ingredient() { }
        public Ingredient(int id, int requiredNum)
        {
            this.id = id;
            this.requiredNum = requiredNum;
        }

        public bool IsEnough()
        {
            if (item.quantity < requiredNum)
                return false;
            return true;
        }

        public string GetQuantityText()
        {
            return item.quantity + "/" + requiredNum;
        }
    }
}

	
