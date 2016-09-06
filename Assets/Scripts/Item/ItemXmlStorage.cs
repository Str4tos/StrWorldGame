using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


public class ItemXmlStorage : MonoBehaviour {

    public static ItemXmlStorage ins;

    public ItemDatabase itemDatabase;


    void Awake()
    {
        ins = this;

    }

    public void SaveItems()
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(ItemDatabase));
        FileStream fileStram = new FileStream(Application.dataPath + "/Resources/XML/items.xml",FileMode.Create);
        xmlSerializer.Serialize(fileStram, itemDatabase);
        fileStram.Close();
    }

    void Start()
    {
        SaveItems();
    }


}


//[System.Serializable]
//public class ItemDatabase
//{
//    public List<Item> list = new List<Item>();
//}
