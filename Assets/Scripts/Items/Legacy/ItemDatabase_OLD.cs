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

        /*ITEM CREATION IS DONE HERE
        Item i0 = new Item("Steel Sword", Item.Type.Weapon, "Common", 100, 0, 25, 0);
        Item i0 = gameObject.AddComponent<Item>();
        itemList.Add(i0);

        Item i1 = new Item("Leather Glove", Item.Type.Armor, "Abundant", 15, 10, 0, 1);
        itemList.Add(i1);

        Item i2 = new Item("Healh Potion", Item.Type.Consumable, "Uncommon", 150, 0, 0, 2);
        itemList.Add(i2);

        Item i3 = new Item("Strange Stone", Item.Type.Misc, "Unique", 0, 0, 0, 3);
        itemList.Add(i3);

        Item i4 = new Item("Scroll of Infinite Wishes", Item.Type.Consumable, "Mythical", 999999, 0, 0, 4);
        itemList.Add(i4);

        Item i5 = new Item("Sharp Knife", Item.Type.Weapon, "Common", 20, 0, 10, 5);
        itemList.Add(i5);
        */
        //RpcSortAllItems();
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
