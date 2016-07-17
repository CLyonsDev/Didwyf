using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryUIManager : NetworkBehaviour {

    public GameObject localPlayer, slotPrefab, itemPrefab;

    public List<GameObject> inventorySlots = new List<GameObject>();

    public List<ItemEntry> playerInv = new List<ItemEntry>();

    int slotChoice;

    int loopCount;


    // Use this for initialization
    void Start () {
        if (!isLocalPlayer)
            return;

        loopCount = 0;

        localPlayer = GameObject.FindGameObjectWithTag("LocalPlayer");

        GrabInventorySlots();

        RefreshInventory();
    }
	
	// Update is called once per frame
	void Update () {
	    if(localPlayer == null)
        {
            localPlayer = GameObject.FindGameObjectWithTag("LocalPlayer");
            GrabInventorySlots();
            RefreshInventory();
        }
        if (inventorySlots.Count == 0)
            GrabInventorySlots();
	}

    void GrabInventorySlots()
    {
        inventorySlots.AddRange(GameObject.FindGameObjectsWithTag("ItemSlot"));
        inventorySlots.Sort(delegate (GameObject i1, GameObject i2)
        {
            return i1.name.CompareTo(i2.name);
        });
    }

    /*public void RefreshInventory()
    {
        if (!Network.isServer)
            CmdRefreshInventory();
        else
            RpcRefreshInventory();
    }*/

    public void RefreshInventory()
    {
        playerInv = localPlayer.GetComponent<Player>().inventory;
        Debug.Log("Requesting an Inventory Update.");
        PopulateInventory();
    }

    public void PopulateInventory()
    {
        /*for (int i = 0; i < playerInv.Count; i++)
        {
            if (inventorySlots[i].transform.childCount > 0)
                Destroy(inventorySlots[i].transform.GetChild(0).gameObject);

            Sprite itemSprite = Resources.Load<Sprite>(playerInv[i].spritePath);
            GameObject newItem = Instantiate(itemPrefab) as GameObject;
            newItem.transform.SetParent(inventorySlots[i].transform, false);
            newItem.GetComponent<NewItem>().itemSprite = itemSprite;
            newItem.GetComponent<NewItem>().currentItem = playerInv[i];
        }*/

        if (playerInv.Count == 0)
            return;

        slotChoice = playerInv.Count - 1;

        if(loopCount >= inventorySlots.Count + 1)
        {
            Debug.LogError("Inventory Full!");
            return;
        }

        while (inventorySlots[slotChoice].transform.childCount != 0)
        {
            loopCount++;
            Debug.Log(slotChoice);
            //Debug.Log("Slot " + inventorySlots[slotChoice] + " has " + inventorySlots[slotChoice].transform.childCount + " children.");
            slotChoice = Random.Range(0, inventorySlots.Count - 1);
            Debug.Log(slotChoice);
        }

        Sprite itemSprite = Resources.Load<Sprite>(playerInv[playerInv.Count - 1].spritePath);
        GameObject newItem = Instantiate(itemPrefab) as GameObject;
        newItem.transform.SetParent(inventorySlots[slotChoice].transform, false);
        newItem.GetComponent<NewItem>().itemSprite = itemSprite;
        newItem.GetComponent<NewItem>().currentItem = playerInv[playerInv.Count - 1];

        Debug.Log("Inventory Updated.");
    }

    [ClientRpc]
    public void RpcRefreshInventory()
    {
        playerInv = localPlayer.GetComponent<Player>().inventory;
        Debug.Log("Requesting an Inventory Update.");
        RpcPopulateInventory();
    }

    //Populates inventory spaces with items
    [ClientRpc]
    public void RpcPopulateInventory()
    {
        for(int i = 0; i < playerInv.Count; i++)
        {
            Sprite itemSprite = Resources.Load<Sprite>(playerInv[i].spritePath);
            GameObject newItem = Instantiate(itemPrefab) as GameObject;
            newItem.transform.SetParent(inventorySlots[i].transform, false);
            newItem.GetComponent<NewItem>().itemSprite = itemSprite;
            newItem.GetComponent<NewItem>().currentItem = playerInv[i];
        }

        Debug.Log("Inventory Updated.");
    }



    [Command]
    public void CmdRefreshInventory()
    {
        playerInv = localPlayer.GetComponent<Player>().inventory;
        Debug.Log("Requesting an Inventory Update.");
        CmdPopulateInventory();
    }

    [Command]
    public void CmdPopulateInventory()
    {
        for (int i = 0; i < playerInv.Count; i++)
        {
            Sprite itemSprite = Resources.Load<Sprite>(playerInv[i].spritePath);
            GameObject newItem = Instantiate(itemPrefab) as GameObject;
            newItem.transform.SetParent(inventorySlots[i].transform, false);
            newItem.GetComponent<NewItem>().itemSprite = itemSprite;
            newItem.GetComponent<NewItem>().currentItem = playerInv[i];
        }

        Debug.Log("Inventory Updated.");
    }
}
