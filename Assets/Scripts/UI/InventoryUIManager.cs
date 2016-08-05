using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class InventoryUIManager : NetworkBehaviour {

    public GameObject localPlayer, slotPrefab, itemPrefab;

    Transform removedTrans; //Triggered

    public List<GameObject> inventorySlots = new List<GameObject>();

    public List<ItemEntry> playerInv = new List<ItemEntry>();

    int slotChoice;

    int loopCount;

    /*[SyncVar]*/ public int openSlots = -1;

    // Use this for initialization
    void Start () {

        openSlots = inventorySlots.Count - 1;

        loopCount = 0;

        localPlayer = GameObject.FindGameObjectWithTag("LocalPlayer");

        GrabInventorySlots();

        RefreshInventory(false);
    }
	
	// Update is called once per frame
	void Update () {

        if(removedTrans == null)
        {
            removedTrans = GameObject.Find("RemovedItems").transform;
        }

        if (localPlayer == null)
        {
            localPlayer = GameObject.FindGameObjectWithTag("LocalPlayer");
            GrabInventorySlots();
            RefreshInventory(false);
        }
        if (inventorySlots.Count == 0)
            GrabInventorySlots();
        if (openSlots == -1)
        {
            if (inventorySlots.Count == 0)
                openSlots = -1;
            else
            {
                openSlots = inventorySlots.Count;
                RefreshInventory(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.H))
            RefreshInventory(false);
    }

    void GrabInventorySlots()
    {
        inventorySlots.Clear();
        inventorySlots.AddRange(GameObject.FindGameObjectsWithTag("ItemSlot"));
        openSlots = inventorySlots.Count;
        //inventorySlots.Sort(IComparer<>);
    }

    public void RefreshInventory(bool clearSprites)
    {
        /*if (openSlots <= 0)
        {
            Debug.LogError("No open slots. Cannot Refresh inventory!");
            return;
        }*/

        if (localPlayer == null)
            return;

        playerInv = localPlayer.GetComponent<Player>().inventory;

        if(!clearSprites)
        {
            Debug.Log("Requesting an Inventory Update.");
            openSlots--;
            PopulateInventory();
        }
        else
        {
            Debug.LogWarning("Clearing Inventory.");
            openSlots = inventorySlots.Count - 1;
            ClearSprites();
        }
    }

    public void ClearSprites()
    {
        foreach(GameObject slot in inventorySlots)
        {
            Debug.Log("Slot " + slot.transform.name + " has " + slot.transform.childCount + " children.");
            if(slot.transform.childCount > 0)
            {
                Debug.LogError("Found " + slot.transform.childCount + " children! Exterminating...");

                Transform itemToRemove = slot.transform.GetChild(0);
                itemToRemove.SetParent(removedTrans);
                itemToRemove.gameObject.SetActive(false);
            }
        }
    }

    public void PopulateInventory()
    {
        if (playerInv.Count == 0)
            return;

        slotChoice = 0;

        if(loopCount >= inventorySlots.Count - 1)
        {
            Debug.LogError("Inventory Full!");
            return;
        }

        while (inventorySlots[slotChoice].transform.childCount != 0)
        {
            loopCount++;
            slotChoice++;
        }

        loopCount = 0;

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
