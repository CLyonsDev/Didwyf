using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class XMLManager : MonoBehaviour {

    public static XMLManager ins;

	void Awake () {
        ins = this;
	}

    //List of Items
    public ItemDatabase itemDB;

    //Save
    public void SaveItems()
    {
        //Open a new XML file
        XmlSerializer serializer = new XmlSerializer(typeof(ItemDatabase));
        FileStream stream = new FileStream(Application.dataPath + "/StreamingAssets/XML/item_data.xml", FileMode.Create);
        serializer.Serialize(stream, itemDB);
        stream.Close();
    }

    //Load
	public void LoadItems()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(ItemDatabase));
        FileStream stream = new FileStream(Application.dataPath + "/StreamingAssets/XML/item_data.xml", FileMode.Open);
        if (stream == null)
        {
            Debug.LogError("Could not find file!");
            return;
        }
        itemDB = (ItemDatabase)serializer.Deserialize(stream);
        stream.Close();
    }

}

[System.Serializable]
public class ItemEntry
{
    public string itemName;
    public int itemID;

    public int damageMin;
    public int damageMax;
    public int critModifier;
    public float weaponRange;
    public float attackDelay;
    public int armor;

    public ItemType type;
    public int value;
    public string rarity;

    public string spritePath = "Sprites/Items/";
}

[System.Serializable]
public class ItemDatabase
{

    [XmlArray("Weapons")]
    public List<ItemEntry> list = new List<ItemEntry>();
}

public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
    Misc
}

#if UNITY_EDITOR
[CustomEditor(typeof(XMLManager))]
public class XMLManagerButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //DrawDefaultInspector();
        XMLManager script = (XMLManager)target;
        if (GUILayout.Button("Save Items to XML File"))
        {
            script.SaveItems();
        }
        if (GUILayout.Button("Refresh XML File"))
        {
            script.LoadItems();
        }
    }
}
#endif