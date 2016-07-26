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

        //StartCoroutine(DisableinventoryUIGOAfterADelay());
        //Debug.Log("RpcCalcStats");

        /*For some reason it can't find the netID of the player fml*/
        //GetComponent<CharacterBase>().CmdRandomizeStats(GetComponent<NetworkIdentity>().netId);
        //GetComponent<CharacterBase>().CmdGenerateStats(GetComponent<NetworkIdentity>().netId);

        //StartCoroutine(DisableinventoryUIGOAfterADelay());
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
            //CmdTakeDamage(5, "Environment");
            Debug.Log("Dealing 5 damage to NetID " + GetComponent<NetworkIdentity>().netId);
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

                GameObject dummy = Instantiate(dummyGO, Vector3.zero, Quaternion.identity) as GameObject;
                RequestItem(GetComponent<NetworkIdentity>().netId, GameObject.Find("GameManager").GetComponent<NetworkIdentity>().netId, dummy.GetComponent<NetworkIdentity>().netId, Random.Range(0, XMLManager.ins.itemDB.list.Count ));
                StartCoroutine(RefreshInventory());
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
        
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity) && hit.transform.tag == "Enemy")
            {
                if (Vector3.Distance(transform.position, hit.transform.position) < GetComponent<CharacterBase>().weaponRange)
                    CmdAttack(hit.transform.gameObject.GetComponent<NetworkIdentity>().netId, GetComponent<NetworkIdentity>().netId);
                else
                    Debug.Log("Not in range!");
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            Debug.Log("Looking for loot");
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity) && hit.transform.tag == "Loot")
            {
                int index = hit.transform.gameObject.GetComponent<CollectableItem>().itemIndex;
                RequestItem(GetComponent<NetworkIdentity>().netId, GameObject.Find("GameManager").GetComponent<NetworkIdentity>().netId, hit.transform.gameObject.GetComponent<NetworkIdentity>().netId,index);
                StartCoroutine(RefreshInventory());
            }
        }
    }

    [Command]
    private void CmdAttack(NetworkInstanceId targetID, NetworkInstanceId playerID)
    {
        if (!isLocalPlayer)
            return;

        GameObject enemy = NetworkServer.FindLocalObject(targetID);
        CharacterBase player = NetworkServer.FindLocalObject(playerID).GetComponent<CharacterBase>();

        EnemyBase eb = enemy.GetComponent<EnemyBase>();

        int roll = (Random.Range(0, 20));
        int modRoll = (roll + player.strength / 2);

        //Debug.LogError(modRoll + " (" + roll + " + " + (player.strength / 2) + ")");
        if (modRoll >= eb.armorRating)
        {
            SendAttack(enemy.GetComponent<NetworkIdentity>().netId, roll == 20, GetComponent<CharacterBase>().weaponCritModifier);
        }
    }

    void SendAttack(NetworkInstanceId targetID, bool isCrit, float critMult)
    {
        if (!isLocalPlayer)
            return;

        if (!Network.isServer)
        {
            CmdSendAttack(targetID, isCrit, critMult);
        }
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
    void CmdSendAttack(NetworkInstanceId targetID, bool isCrit, float critMult)
    {
        GameObject target = NetworkServer.FindLocalObject(targetID);
        if (isCrit)
            target.GetComponent<EnemyBase>().TakeDamage(target.GetComponent<NetworkIdentity>().netId, Mathf.Round(Random.Range(GetComponent<CharacterBase>().totalDamageMin, GetComponent<CharacterBase>().totalDamageMax)) * critMult, transform.name);
        else
            target.GetComponent<EnemyBase>().TakeDamage(target.GetComponent<NetworkIdentity>().netId, Mathf.Round(Random.Range(GetComponent<CharacterBase>().totalDamageMin, GetComponent<CharacterBase>().totalDamageMax)), transform.name);
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
            //uim.RefreshInventory();
            //player.GetComponent<Player>().StartCoroutine(RefreshInventory(objectID));
            //player.GetComponent<Player>().StartCoroutine(RefreshInventory());
            //gm.GetComponent<InventoryUIManager>().CmdRefreshInventory();
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
            StartCoroutine(RefreshInventory());
            //gm.GetComponent<InventoryUIManager>().CmdRefreshInventory();
        }
        else
        {
            Debug.LogError("Could not find the Item Manager on object " + objectID + " are you sure that the ID is correct?");
        }
    }

    /*IEnumerator RefreshInventory(NetworkInstanceId objectID)
    {
        //InventoryUIManager ui;
        yield return new WaitForSeconds(0.1f);

        if (!Network.isServer)
        {
            ClientScene.FindLocalObject(objectID).GetComponent<InventoryUIManager>().RefreshInventory();

        }
        else
        {
            ClientScene.FindLocalObject(objectID).GetComponent<InventoryUIManager>().RefreshInventory();
        }
        //ui.RefreshInventory();
        
    }*/

    IEnumerator RefreshInventory()
    {
        //InventoryUIManager ui;

        if (!isLocalPlayer)
            yield return null;

        yield return new WaitForSeconds(0.1f);
        GameObject.Find("GameManager").GetComponent<InventoryUIManager>().RefreshInventory();
        //ui.RefreshInventory();

    }

    /*IEnumerator DisableinventoryUIGOAfterADelay()
    {
        yield return new WaitForSeconds(0.15f);
        inventoryUIGO.SetActive(false);
    }*/
}
