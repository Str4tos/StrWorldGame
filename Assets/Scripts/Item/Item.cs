using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Item
{
    public string name;                                    
    public int id;                                          
    public string description;
    public ItemType itemType;                          
    //public Sprite itemIcon;                                     
    //public GameObject itemDropModel; 
    public float salePrice;
    public int quantity = 1;           
    public int maxInStack = 1;
    public int indexItemInList = 999;

    //Equipment
	public ItemRequirements itemRequirements = new ItemRequirements();
	public List<ItemAttribute> itemAttributes = new List<ItemAttribute>();

    //Consume
    public int numberTicks;
    public float delayTick;

    
//    public Item(){}
//
//    public Item(string name, int id, string description, Sprite itemIcon, GameObject itemDropModel, int maxStack)                 //function to create a instance of the Item
//    {
//        this.name = name;
//        this.id = id;
//        this.description = description;
//        this.itemIcon = itemIcon;
//        this.itemDropModel = itemDropModel;
//        this.maxInStack = maxStack;
//    }
//
    public Item getCopy()
    {
        return (Item)this.MemberwiseClone();        
    }   
    
    
}


