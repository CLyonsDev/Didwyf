using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Player : NetworkBehaviour
{
    public string playerName;

    public GameObject gameManager;

    public GameObject canvasGO;

    bool canvasEnabled = true;

    public List<ItemEntry> inventory = new List<ItemEntry>();

    // Use this for initialization
    void Start()
    {
        if (!isLocalPlayer)
            return;

        canvasGO = GameObject.Find("Canvas");

        canvasGO.SetActive(false);

        //Debug.Log("RpcCalcStats");

        GetComponent<CharacterBase>().CmdRandomizeStats(GetComponent<NetworkIdentity>().netId);
        GetComponent<CharacterBase>().CmdGenerateStats(GetComponent<NetworkIdentity>().netId);
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.B))
        {
            //CmdTakeDamage(5, "Environment");
            GetComponent<CharacterBase>().CmdReportDamage(GetComponent<NetworkIdentity>().netId, 5, "Environment");
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            GetComponent<CharacterBase>().CmdHeal(5, "An Omniscient Deity");
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            GetComponent<CharacterBase>().CmdRequestRespawn(GetComponent<NetworkIdentity>().netId);
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            GetComponent<CharacterBase>().RandomizeStats();
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

        if(Input.GetKeyDown(KeyCode.L))
        {
            GetComponent<CharacterBase>().CmdAddStats(GetComponent<NetworkIdentity>().netId, 100, 0, 0, 0);
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            canvasEnabled = !canvasEnabled;
            SwitchCanvasEnabled();
        }
        
        //We left off here. We were going to make it so that you can right-click loot, and it does the above logic to grant the player that item.
        if(Input.GetMouseButtonDown(1))
        {
            Debug.Log("Looking for loot");

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
