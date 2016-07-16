using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Player : CharacterBase
{

    public string playerName;

    public GameObject gameManager;

    public GameObject canvasGO;

    bool canvasEnabled = true;

    public List<ItemEntry> inventory = new List<ItemEntry>();

    // Use this for initialization
    void Start()
    {

        playerName = ("Player " + Random.Range(0, 10000).ToString());
        transform.name = playerName;

        canvasGO = GameObject.Find("Canvas");

        canvasGO.SetActive(false);
        //RpcCalcStats();
        //Debug.Log("RpcCalcStats");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            RpcTakeDamage(5, "Environment");
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            RpcRespawn();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("You pressed G.");

            if (isLocalPlayer)
            {
                Debug.Log("You are the player.");

                RequestItem(GetComponent<NetworkIdentity>().netId, GameObject.Find("GameManager").GetComponent<NetworkIdentity>().netId);
            }
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            canvasEnabled = !canvasEnabled;
            SwitchCanvasEnabled();
        }
    }

    void SwitchCanvasEnabled()
    {
        canvasGO.SetActive(canvasEnabled);
    }

    void RequestItem(NetworkInstanceId playerID, NetworkInstanceId objectID)
    {
        Debug.Log("Here goes nothing...");
        if (!Network.isServer)
        {
            Debug.LogWarning("Not Server.");
            CmdRequestItem(playerID, objectID);
        }
        else
        {
            Debug.LogError("Not Client.");
            RpcRequestItem(playerID, objectID);
        }

        StartCoroutine(RefreshInventory(objectID));
    }


    [Command]
    void CmdRequestItem(NetworkInstanceId playerID, NetworkInstanceId objectID)
    {
        Debug.Log("Requesting an item from " + objectID + " for player " + playerID);

        ItemManager im = NetworkServer.FindLocalObject(objectID).GetComponent<ItemManager>();

        if (im != null)
        {
            Debug.Log("Server has been asked for the item.");
            im.RequestItem(GetComponent<NetworkIdentity>().netId, Random.Range(0, XMLManager.ins.itemDB.list.Count - 1));
            //StartCoroutine(RefreshInventory(objectID));
            //gm.GetComponent<InventoryUIManager>().CmdRefreshInventory();
        }
        else
        {
            Debug.LogError("Could not find the Item Manager on object " + objectID + " are you sure that the ID is correct?");
        }
    }

    [ClientRpc]
    void RpcRequestItem(NetworkInstanceId playerID, NetworkInstanceId objectID)
    {
        Debug.Log("Requesting an item from " + objectID + " for player " + playerID);

        ItemManager im = ClientScene.FindLocalObject(objectID).GetComponent<ItemManager>();

        if (im != null)
        {
            Debug.Log("Server has been asked for the item.");
            im.RequestItem(GetComponent<NetworkIdentity>().netId, 0);

            //gm.GetComponent<InventoryUIManager>().CmdRefreshInventory();
        }
        else
        {
            Debug.LogError("Could not find ItemManager or InventoryUIManager!");
            return;
        }
    }

    IEnumerator RefreshInventory(NetworkInstanceId objectID)
    {
        Debug.Log("RefreshInventory");
        //InventoryUIManager ui;
        yield return new WaitForSeconds(0.1f);

        if (!Network.isServer)
        {
            ClientScene.FindLocalObject(objectID).GetComponent<InventoryUIManager>().RefreshInventory();
            Debug.Log("Found UIManager.");

        }
        else
        {
            ClientScene.FindLocalObject(objectID).GetComponent<InventoryUIManager>().RefreshInventory();
            Debug.Log("Found UIManager.");

        }

        Debug.Log("Refreshing...");
        //ui.RefreshInventory();

    }
}
