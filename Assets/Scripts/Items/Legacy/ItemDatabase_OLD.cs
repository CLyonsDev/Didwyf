using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ItemDatabase_OLD : NetworkBehaviour
{

    public Sprite[] sprites;

    public static ItemDatabase_OLD _instance;

    public static List<Item> itemList = new List<Item>();
    public static List<Item> sortedItemList = new List<Item>();

	// Use this for initialization
	void Awake () {
        _instance = this;
    }

    public void RpcAddItem(Item item)
    {
        itemList.Add(item);
        RpcSortAllItems();
    }

    [ClientRpc]
    public void RpcSortAllItems()
    {
        sortedItemList.Clear();
        foreach(Item i in itemList)
        {
            sortedItemList.Add(i);
        }
    }
}
