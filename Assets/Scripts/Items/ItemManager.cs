using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ItemManager : NetworkBehaviour {


    public void RequestItem(NetworkInstanceId playerID, int itemID)
    {
        if (!Network.isServer)
        {
            Debug.LogWarning("NotServerItemManager");
            CmdRequestItem(playerID, itemID);
            RpcRequestItem(playerID, itemID);
        }
        else   
        {
            Debug.LogWarning("ServerItemManager");
            RpcRequestItem(playerID, itemID);
        }
    }


    //A client sided command that requests an item from the server
    [ClientRpc]
    public void RpcRequestItem(NetworkInstanceId playerID, int itemID)
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
        //CmdSendItemToPlayer(playerID, itemID);

        player.GetComponent<Player>().inventory.Add(XMLManager.ins.itemDB.list[itemID]);
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
        //CmdSendItemToPlayer(playerID, itemID);

        player.GetComponent<Player>().inventory.Add(XMLManager.ins.itemDB.list[itemID]);
        Debug.Log("Sent item to player.");
    }
}
