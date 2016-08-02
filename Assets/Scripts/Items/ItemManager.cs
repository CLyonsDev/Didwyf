using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ItemManager : NetworkBehaviour {


    public void RequestItem(NetworkInstanceId playerID, NetworkInstanceId itemGOID, int itemID)
    {
        if (Network.isClient)
        {
            if(GetComponent<InventoryUIManager>().openSlots <= 0)
            {
                Debug.LogError("Inventory is full, could not give item.");
                return;
            }
            Debug.LogWarning("NotServerItemManager");
            RpcRequestItem(playerID, itemGOID, itemID);
        }
        else   
        {
            Debug.LogWarning("ServerItemManager");
            RpcRequestItem(playerID, itemGOID, itemID);
        }
    }

    public void RemoveItem(NetworkInstanceId playerID, int index)
    {
        if(!Network.isServer)
        {
            GetComponent<InventoryUIManager>().openSlots++;
            RpcRemoveItem(playerID, index);
        }
    }

    [ClientRpc]
    void RpcRemoveItem(NetworkInstanceId playerID, int index)
    {
        GameObject player = ClientScene.FindLocalObject(playerID);
        Debug.Log("Server is removing item.");
        player.GetComponent<Player>().inventory.RemoveAt(index);

    }


    //A client sided command that requests an item from the server
    [ClientRpc]
    public void RpcRequestItem(NetworkInstanceId playerID, NetworkInstanceId itemGOID, int itemID)
    {

        GameObject player = ClientScene.FindLocalObject(playerID);

        if(XMLManager.ins.itemDB.list[itemID] == null)
        {
            Debug.LogError("Could not find an item with an ID of " + itemID + " in the database! Are you sure you entered it correctly?");
            return;
        }else if(player == null)
        {
            Debug.LogError("Could not find a player with an ID of " + playerID + "! Are you sure you entered it correctly?");
            return;
        }
        Debug.Log("Requesting " + XMLManager.ins.itemDB.list[itemID].itemName + " (ID: " + XMLManager.ins.itemDB.list[itemID].itemID + ")");

        player.GetComponent<Player>().inventory.Add(XMLManager.ins.itemDB.list[itemID]);
        if(itemGOID != null)
            NetworkServer.Destroy(ClientScene.FindLocalObject(itemGOID));
        Debug.Log("Sent item to player.");
    }

    [Command]
    public void CmdRequestItem(NetworkInstanceId playerID, int itemID)
    {

        GameObject player = NetworkServer.FindLocalObject(playerID);

        if (XMLManager.ins.itemDB.list[itemID] == null)
        {
            Debug.LogError("Could not find an item with an ID of " + itemID + " in the database! Are you sure you entered it correctly?");
            return;
        }
        else if (player == null)
        {
            Debug.LogError("Could not find a player with an ID of " + playerID + "! Are you sure you entered it correctly?");
            return;
        }
        Debug.Log("Requesting " + XMLManager.ins.itemDB.list[itemID].itemName + " (ID: " + XMLManager.ins.itemDB.list[itemID].itemID + ")");

        player.GetComponent<Player>().inventory.Add(XMLManager.ins.itemDB.list[itemID]);
        Debug.Log("Sent item to player.");
    }
}
