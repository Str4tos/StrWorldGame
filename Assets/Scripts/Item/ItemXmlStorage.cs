using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


public class ItemXmlStorage : MonoBehaviour
{

    private readonly string[] arrayFiles = new string[] { "Items_0-49.xml", "Items_50-99.xml", "Items_100-149.xml" };
    private const string defaultSpritePath = "Textures/Icons/Item/I_Map";
    private const string defaultDropModelPath = "Prefabs/Inventory/DraggingItem";
    
    //public void SaveItems()
    //{
    //    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ItemDatabaseCustom));
    //    FileStream fileStram = new FileStream(Application.dataPath + "/Resources/XML/itemsEquip.xml", FileMode.Create);
    //    xmlSerializer.Serialize(fileStram, ItemDatabaseCustom);
    //    fileStram.Close();
    //}

    public ItemCustom GetItemById(int id)
    {
        XmlReader xmlReader = GetXmlReader(id);
        ItemTypeCustom tempItemType;
        while (xmlReader.Read())
        {
            switch (xmlReader.Name)
            {
                case "item":
                    tempItemType = ItemTypeCustom.Other;
                    break;
                case "equip":
                    tempItemType = ItemTypeCustom.Equip;
                    break;
                case "consume":
                    tempItemType = ItemTypeCustom.Consume;
                    break;
                default: continue;
            }

            if (xmlReader.GetAttribute("id") == id.ToString())
            {
                ItemCustom tempItem = new ItemCustom(id, xmlReader.GetAttribute("name"));
                tempItem.itemType = tempItemType;

                xmlReader.ReadToDescendant("description"); //<description>       
                tempItem.description = xmlReader.ReadElementContentAsString();

                //<icon path=.../>                                          
                if (xmlReader.ReadToNextSibling("icon") && xmlReader.GetAttribute(0) != "")
                    tempItem.iconPath = xmlReader.GetAttribute(0);
                else
                    tempItem.iconPath = defaultSpritePath;

                //<model path=.../>                                        
                if (xmlReader.ReadToNextSibling("model") && xmlReader.GetAttribute(0) != "")
                    tempItem.dropModelPath = xmlReader.GetAttribute(0);
                else
                    tempItem.dropModelPath = defaultDropModelPath;

                if (xmlReader.ReadToNextSibling("salePrice")) //<salePrice>..</salePrice> 
                    tempItem.salePrice = xmlReader.ReadElementContentAsInt();
                else
                    tempItem.salePrice = 0;

                return tempItem;
            }
            else
            {
                xmlReader.Skip();
            }

        }
        return null;
    }

    public ItemTypeCustom GetItemTypeById(int id)
    {
        XmlReader xmlReader = GetXmlReader(id);
        ItemTypeCustom tempItemType;
        while (xmlReader.Read())
        {
            switch (xmlReader.Name)
            {
                case "item":
                    tempItemType = ItemTypeCustom.Other;
                    break;
                case "equip":
                    tempItemType = ItemTypeCustom.Equip;
                    break;
                case "consume":
                    tempItemType = ItemTypeCustom.Consume;
                    break;
                default: continue;
            }

            if (xmlReader.GetAttribute("id") == id.ToString())
                return tempItemType;
            else
                xmlReader.Skip();
        }
        return ItemTypeCustom.Other;
    }

    /// <summary>
    /// Return Item Equip clear from xml storage
    /// </summary>
    /// <param name="id">Id item</param>
    public ItemEquip GetItemEquipById(int id)
    {
        XmlReader xmlReader = GetXmlReader(id);
        while (xmlReader.Read())
        {
            if (xmlReader.Name == "equip")
                if (xmlReader.GetAttribute("id") == id.ToString())
                {
                    //<equip ...>
                    return LoadEquipItem(xmlReader, id);
                }
                else
                    xmlReader.Skip();

            if (xmlReader.Name == "consume" || xmlReader.Name == "item")
                xmlReader.Skip();
        }
        return null;
    }
    /// <summary>
    /// Return Item Consume clear from xml storage
    /// </summary>
    /// <param name="id">Id item</param>
    public ItemConsume GetItemConsumeById(int id)
    {
        XmlReader xmlReader = GetXmlReader(id);
        while (xmlReader.Read())
        {
            if (xmlReader.Name == "consume")
                if (xmlReader.GetAttribute("id") == id.ToString())
                {
                    //<consume ...>
                    return LoadConsumeItem(xmlReader, id);
                }
                else
                    xmlReader.Skip();

            if (xmlReader.Name == "equip" || xmlReader.Name == "item")
                xmlReader.Skip();
        }
        return null;
    }
    /// <summary>
    /// Return Item Other clear from xml storage
    /// </summary>
    /// <param name="id">Id item</param>
    public ItemOther GetItemOtherById(int id)
    {
        XmlReader xmlReader = GetXmlReader(id);
        while (xmlReader.Read())
        {
            if (xmlReader.Name == "item")
                if (xmlReader.GetAttribute("id") == id.ToString())
                {
                    //<item ...>
                    return LoadOtherItem(xmlReader, id);
                }
                else
                    xmlReader.Skip();

            if (xmlReader.Name == "equip" || xmlReader.Name == "consume")
                xmlReader.Skip();
        }
        return null;
    }

    public int GetConsumeMaxInStackById(int id)
    {
        XmlReader xmlReader = GetXmlReader(id);
        while (xmlReader.Read())
        {
            if (xmlReader.Name == "item" || xmlReader.Name == "equip")
            {
                xmlReader.Skip();
                continue;
            }
            if (xmlReader.Name == "consume")
            {
                if (xmlReader.GetAttribute("id") == id.ToString())
                {
                    if (xmlReader.ReadToFollowing("maxInStack")) //<maxInStack>
                        return xmlReader.ReadElementContentAsInt();
                    else
                        return 99;
                }
                xmlReader.Skip();
            }
        }
        return 1;
    }
    private XmlReader GetXmlReader(int id)
    {
        int numFile = id / 50;
        XmlReader xmlReader = XmlReader.Create(Application.dataPath + "/Resources/XML/" + arrayFiles[numFile]);  
        xmlReader.MoveToContent();
        return xmlReader;
    }

    private ItemEquip LoadEquipItem(XmlReader xmlReader, int id)
    {
        ItemEquip itemEquip = new ItemEquip(id, xmlReader.GetAttribute("name"));
        itemEquip.itemEquipType = (ItemEquipType)System.Enum.Parse(typeof(ItemEquipType), xmlReader.GetAttribute("type"));


        xmlReader.ReadToDescendant("description"); //<description>       
        itemEquip.description = xmlReader.ReadElementContentAsString();

        //<icon path=.../>                                          
        if (xmlReader.ReadToNextSibling("icon") && xmlReader.GetAttribute(0) != "")
            itemEquip.iconPath = xmlReader.GetAttribute(0);
        else
            itemEquip.iconPath = defaultSpritePath;

        //<model path=.../>                                        
        if (xmlReader.ReadToNextSibling("model") && xmlReader.GetAttribute(0) != "")
            itemEquip.dropModelPath = xmlReader.GetAttribute(0);
        else
            itemEquip.dropModelPath = defaultDropModelPath;

        if (xmlReader.ReadToNextSibling("salePrice")) //<salePrice>..</salePrice> 
            itemEquip.salePrice = xmlReader.ReadElementContentAsInt();
        else
            itemEquip.salePrice = 0;

        if (xmlReader.ReadToNextSibling("requests"))
        { //<requests lvl=.. str=.. agi=.. vit=.. ene=.. />
            int.TryParse(xmlReader.GetAttribute("lvl"), out itemEquip.requestLevel);
            int.TryParse(xmlReader.GetAttribute("str"), out itemEquip.requestStrenght);
            int.TryParse(xmlReader.GetAttribute("agi"), out itemEquip.requestAgility);
            int.TryParse(xmlReader.GetAttribute("vit"), out itemEquip.requestVitality);
            int.TryParse(xmlReader.GetAttribute("ene"), out itemEquip.requestEnergy);
        }
        else
        {
            itemEquip.requestLevel = 1;
            itemEquip.requestStrenght = 0;
            itemEquip.requestAgility = 0;
            itemEquip.requestVitality = 0;
            itemEquip.requestEnergy = 0;
        }

        //<atributes>
        if (xmlReader.ReadToNextSibling("atributes") && xmlReader.ReadToDescendant("add"))//<add..
        {
            ItemAtributeType tempType;
            float tempValue;
            do
            {
                tempType = (ItemAtributeType)System.Enum.Parse(typeof(ItemAtributeType), xmlReader.GetAttribute(0));
                tempValue = xmlReader.ReadElementContentAsFloat();
                itemEquip.itemAttributes.Add(new ItemAttribute(tempType, tempValue));
            } while (xmlReader.ReadToNextSibling("add"));
        }

        xmlReader.Close();
        return itemEquip;
    }
    private ItemOther LoadOtherItem(XmlReader xmlReader, int id)
    {
        ItemOther itemOther = new ItemOther(id, xmlReader.GetAttribute("name"));
        itemOther.itemOtherType = (ItemOtherType)System.Enum.Parse(typeof(ItemOtherType), xmlReader.GetAttribute("type"));

        xmlReader.ReadToDescendant("description"); //<description>       
        itemOther.description = xmlReader.ReadElementContentAsString();

        //<icon path=.../>                                          
        if (xmlReader.ReadToNextSibling("icon") && xmlReader.GetAttribute(0) != "")
            itemOther.iconPath = xmlReader.GetAttribute(0);
        else
            itemOther.iconPath = defaultSpritePath;

        //<model path=.../>                                        
        if (xmlReader.ReadToNextSibling("model") && xmlReader.GetAttribute(0) != "")
            itemOther.dropModelPath = xmlReader.GetAttribute(0);
        else
            itemOther.dropModelPath = defaultDropModelPath;

        if (xmlReader.ReadToNextSibling("salePrice")) //<salePrice>..</salePrice> 
            itemOther.salePrice = xmlReader.ReadElementContentAsInt();
        else
            itemOther.salePrice = 0;

        xmlReader.Close();
        return itemOther;
    }
    private ItemConsume LoadConsumeItem(XmlReader xmlReader, int id)
    {
        ItemConsume itemConsume = new ItemConsume(id, xmlReader.GetAttribute("name"));

        xmlReader.ReadToDescendant("description"); //<description>       
        itemConsume.description = xmlReader.ReadElementContentAsString();

        //<icon path=.../>                                          
        if (xmlReader.ReadToNextSibling("icon") && xmlReader.GetAttribute(0) != "")
            itemConsume.iconPath = xmlReader.GetAttribute(0);
        else
            itemConsume.iconPath = defaultSpritePath;

        //<model path=.../>                                        
        if (xmlReader.ReadToNextSibling("model") && xmlReader.GetAttribute(0) != "")
            itemConsume.dropModelPath = xmlReader.GetAttribute(0);
        else
            itemConsume.dropModelPath = defaultDropModelPath;

        if (xmlReader.ReadToNextSibling("salePrice")) //<salePrice>..</salePrice> 
            itemConsume.salePrice = xmlReader.ReadElementContentAsInt();
        else
            itemConsume.salePrice = 0;

        if (xmlReader.ReadToFollowing("numberTicks")) //<numberTicks>
            itemConsume.numberTicks = xmlReader.ReadElementContentAsInt();
        else
            itemConsume.numberTicks = 1;

        if (xmlReader.ReadToFollowing("delayTick")) //<delayTick>
            itemConsume.delayTick = xmlReader.ReadElementContentAsFloat();
        else
            itemConsume.delayTick = 1.0f;

        if (xmlReader.ReadToFollowing("maxInStack")) //<maxInStack>
            itemConsume.maxInStack = xmlReader.ReadElementContentAsInt();
        else
            itemConsume.maxInStack = 99;

        //<atributes>
        if (xmlReader.ReadToNextSibling("atributes") && xmlReader.ReadToDescendant("add"))//<add..
        {
            ItemAtributeType tempType;
            float tempValue;
            do
            {
                tempType = (ItemAtributeType)System.Enum.Parse(typeof(ItemAtributeType), xmlReader.GetAttribute(0));
                tempValue = xmlReader.ReadElementContentAsFloat();
                itemConsume.itemAttributes.Add(new ItemAttribute(tempType, tempValue));
            } while (xmlReader.ReadToNextSibling("add"));
        }

        xmlReader.Close();
        return itemConsume;
    }

}

