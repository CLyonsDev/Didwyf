using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Player : NetworkBehaviour
{
    public string playerName;

    public GameObject gameManager;

    public GameObject inventoryUIGO, dummyGO;

    bool canvasEnabled = true;

    CharacterBase cb;

    public List<ItemEntry> inventory = new List<ItemEntry>();

    // Use this for initialization
    void Start()
    {

        if (!isLocalPlayer)
        {
            Debug.LogWarning("Not Local Player");
            return;
        }

        inventoryUIGO = GameObject.Find("Canvas").transform.GetChild(3).gameObject;
        cb = GetComponent<CharacterBase>();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if(inventoryUIGO == null)
        {
            inventoryUIGO = GameObject.Find("InventoryScreen");          
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Dealing 5 damage to NetID " + GetComponent<NetworkIdentity>().netId);
            GetComponent<CharacterBase>().CmdReportAttack(GetComponent<NetworkIdentity>().netId, GetComponent<NetworkIdentity>().netId, GameObject.Find("GameManager").GetComponent<NetworkIdentity>().netId, 99999, 5, "Wrathful Diety");
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            GetComponent<CharacterBase>().CmdHeal(GetComponent<NetworkIdentity>().netId, 5, "An Omniscient Deity");
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

                GameObject dummy = Instantiate(dummyGO, Vector3.zero, Quaternion.identity) as GameObject;
                RequestItem(GetComponent<NetworkIdentity>().netId, GameObject.Find("GameManager").GetComponent<NetworkIdentity>().netId, dummy.GetComponent<NetworkIdentity>().netId, Random.Range(0, XMLManager.ins.itemDB.list.Count ));
                StartCoroutine(RefreshInventory(0.1f, false));
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
            if(GameObject.Find("InfoBox(Clone)") != null)
                Destroy(GameObject.Find("InfoBox(Clone)"));
        }

        if(Input.GetKeyDown(KeyCode.Keypad5))
        {
            CmdReloadLevel();
        }

        if(Input.GetMouseButtonDown(1))
        {
            //Debug.Log("Looking for loot");
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity) && hit.transform.tag == "Loot")
            {
                if(Vector3.Distance(transform.position, hit.transform.position) <= GetComponent<CharacterBase>().grabRange)
                {
                    int index = hit.transform.gameObject.GetComponent<CollectableItem>().itemIndex;
                    RequestItem(GetComponent<NetworkIdentity>().netId, GameObject.Find("GameManager").GetComponent<NetworkIdentity>().netId, hit.transform.gameObject.GetComponent<NetworkIdentity>().netId, index);
                    StartCoroutine(RefreshInventory(0.1f, false));
                }   
            }
        }
    }

    public void StartRefreshInventoryCoroutine(float delay, bool clearSprites)
    {
        Debug.LogWarning("StartRefreshInventoryCoroutine");
        StartCoroutine(RefreshInventory(delay, clearSprites));
    }

    [Command]
    public void CmdReloadLevel()
    {
        NetworkManager.singleton.ServerChangeScene(NetworkManager.networkSceneName);
    }

    [Command]
    public void CmdNextLevel()
    {
        NetworkManager.singleton.ServerChangeScene("TutorialDungeon");
    }

    void SwitchCanvasEnabled()
    {
        inventoryUIGO.SetActive(canvasEnabled);
    }

    void RequestItem(NetworkInstanceId playerID, NetworkInstanceId objectID, NetworkInstanceId itemGOID, int itemIndex)
    {
        if (!isLocalPlayer)
            return;

        Debug.Log("Here goes nothing...");
        if (!Network.isServer)
        {
            Debug.LogWarning("Not Server.");
            CmdRequestItem(playerID, objectID, itemGOID, itemIndex);
        }
        else
        {
            Debug.LogError("Not Client.");
            RpcRequestItem(playerID, objectID, itemGOID, itemIndex);
        }

        //StartCoroutine(RefreshInventory(objectID));
    }

    public void RemoveItem(NetworkInstanceId playerID, NetworkInstanceId gamemanagerID, ItemEntry itemToRemove)
    {
        Debug.Log("Requesting to remove the item " + itemToRemove.itemName + ".");
        int index = inventory.IndexOf(itemToRemove);
        CmdRemoveItem(playerID, gamemanagerID, index);
    }

    [Command]
    void CmdRemoveItem(NetworkInstanceId playerID, NetworkInstanceId gamemanagerID, int index)
    {
        Debug.Log("Asking to remove.");
        ItemManager im = NetworkServer.FindLocalObject(gamemanagerID).GetComponent<ItemManager>();

        if(im != null)
        {
            Debug.Log("Server has been asked to remove the item.");
            im.RemoveItem(GetComponent<NetworkIdentity>().netId, index);
        }
    }


    [Command]
    void CmdRequestItem(NetworkInstanceId playerID, NetworkInstanceId gameManagerID, NetworkInstanceId itemGOID, int itemIndex)
    {
        Debug.Log("Requesting an item from " + gameManagerID + " for player " + playerID);

        GameObject player = NetworkServer.FindLocalObject(playerID);

        GameObject gameManager = NetworkServer.FindLocalObject(gameManagerID);

        ItemManager im = gameManager.GetComponent<ItemManager>();
        InventoryUIManager uim = gameManager.GetComponent<InventoryUIManager>();

        if (im != null)
        {
            Debug.Log("Server has been asked for the item.");
            im.RequestItem(GetComponent<NetworkIdentity>().netId, itemGOID, itemIndex);
        }
        else
        {
            Debug.LogError("Could not find the Item Manager on object " + gameManagerID + " are you sure that the ID is correct?");
        }
    }

    [ClientRpc]
    void RpcRequestItem(NetworkInstanceId playerID, NetworkInstanceId objectID, NetworkInstanceId itemGOID, int itemIndex)
    {
        Debug.Log("Requesting an item from " + objectID + " for player " + playerID);

        ItemManager im = ClientScene.FindLocalObject(objectID).GetComponent<ItemManager>();

        if (im != null)
        {
            Debug.Log("Server has been asked for the item.");
            im.RequestItem(GetComponent<NetworkIdentity>().netId, itemGOID, itemIndex);
            StartCoroutine(RefreshInventory(0.1f, false));
        }
        else
        {
            Debug.LogError("Could not find the Item Manager on object " + objectID + " are you sure that the ID is correct?");
        }
    }

    public IEnumerator RefreshInventory(float delay, bool clearSprites)
    {
        if (!isLocalPlayer)
            yield return null;

        yield return new WaitForSeconds(delay);
        GameObject.Find("GameManager").GetComponent<InventoryUIManager>().RefreshInventory(clearSprites);
    }
}
