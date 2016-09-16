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
        public int number;

        public Ingredient() { }
        public Ingredient(int id, int number)
        {
            this.id = id;
            this.number = number;
        }
    }
}

	
