using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using System.IO;

public class XMLStorageRecipes {

    private const int quantityIdInFile = 50;

    private XmlReader GetXmlReader(int id)
    {
        int minIdInFile = (id / quantityIdInFile) * quantityIdInFile;
        string nameFile = minIdInFile + "-" + (minIdInFile + quantityIdInFile - 1);
        TextAsset _TextAsset = Resources.Load("XML/Craft/" + nameFile) as TextAsset;
        TextReader _TextReader = new StringReader(_TextAsset.text);
        XmlReader xmlReader = XmlReader.Create(_TextReader);
        xmlReader.MoveToContent();
        return xmlReader;
    }

    public RecipeCraft GetRecipeByRecipeId(int recipeId)
    {
        XmlReader xmlReader = GetXmlReader(recipeId);
        while (xmlReader.Read())
        {
            if (xmlReader.Name == "recipe")
                if (xmlReader.GetAttribute("recipe_id") == recipeId.ToString())
                {
                    return LoadRecipeCraft(xmlReader);
                }
                else
                    xmlReader.Skip();
        }
        return null;
    }

    public RecipeCraft GetRecipeByResultId(int resultId)
    {
        XmlReader xmlReader = GetXmlReader(resultId);
        while (xmlReader.Read())
        {
            if (xmlReader.Name == "recipe")
                if (xmlReader.GetAttribute("result_id") == resultId.ToString())
                {
                    return LoadRecipeCraft(xmlReader);
                }
                else
                    xmlReader.Skip();
        }
        return null;
    }


    private RecipeCraft LoadRecipeCraft(XmlReader xmlReader)
    {
        int recipe_id = int.Parse(xmlReader.GetAttribute("recipe_id"));
        int result_id = int.Parse(xmlReader.GetAttribute("result_id"));
        int chance = int.Parse(xmlReader.GetAttribute("chance"));
        RecipeCraft result = new RecipeCraft(recipe_id, result_id, chance);

        if (xmlReader.ReadToDescendant("ingridien"))
        {
            do
            {
                int id = int.Parse(xmlReader.GetAttribute("id"));
                int number = int.Parse(xmlReader.GetAttribute("number"));
                result.AddIngridient(id, number);
            } while (xmlReader.ReadToNextSibling("ingridien"));
        }
        return result;
    }

}
