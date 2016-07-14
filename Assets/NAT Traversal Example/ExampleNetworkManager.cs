using NATTraversal;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking.Types;

[HelpURL("http://grabblesgame.com/nat-traversal/docs/class_n_a_t_traversal_1_1_network_manager.html")]
public class ExampleNetworkManager : NATTraversal.NetworkManager
{

    public GameObject otherObPrefab;

    public override void Start()
    {
        base.Start();
    }

#if UNITY_5_4
    public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        if (!success || matchList.Count == 0) return;
        
        MatchInfoSnapshot match = matchList[matchList.Count - 1];
        Debug.Log("Match list returned, starting client");
        StartClientAll(match);
    }
#else
    public override void OnMatchList(ListMatchResponse matchList)
    {
        if (!matchList.success || matchList.matches.Count == 0) return;

        MatchDesc match = matchList.matches[0];
        matchID = match.networkId;
        StartClientAll(match);
    }
#endif

    // This will never actually be called (thanks Unity) but you have to define it and pass
    // it in to DropConnection anyway or you can't leave the match..
#if UNITY_5_4
    private void OnMatchDropped(bool success, string extendedInfo)
    {
        Debug.LogWarning("Match dropped");
    }
#else
    public void OnMatchDropped(BasicResponse resp)
    {
        Debug.LogWarning("Match dropped");
    }
#endif

    public void StartupHost()
    {
        if (matchMaker == null) matchMaker = gameObject.AddComponent<NetworkMatch>();

        //matchMaker.CreateMatch("test", 10, true, "", OnMatchCreate);
        StartHostAll("Hello World", 6);
    }

    public void JoinAnyGame()
    {
        matchMaker.ListMatches(0, 1, "", true, 0, 0, OnMatchList);
    }

    public void DisconnectFromServer()
    {
        if (matchID != NetworkID.Invalid && matchmakingNodeID != NodeID.Invalid)
        {
            if (NetworkServer.active)
            {
#if UNITY_5_4
                matchMaker.DestroyMatch(matchID, 0, OnMatchDropped);
#else
                    matchMaker.DestroyMatch(matchID, OnMatchDropped);
#endif
            }
            else
            {
#if UNITY_5_4
                matchMaker.DropConnection(matchID, matchmakingNodeID, 0, OnMatchDropped);
#else
                    matchMaker.DropConnection(matchID, matchmakingNodeID, OnMatchDropped);
#endif
            }
        }

        if (NetworkServer.active)
        {
            NetworkServer.SetAllClientsNotReady();
            StopHost();
        }
        else
        {
            StopClient();
        }
    }

    /*void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "Host"))
        {
            if (matchMaker == null) matchMaker = gameObject.AddComponent<NetworkMatch>();

            //matchMaker.CreateMatch("test", 10, true, "", OnMatchCreate);
            StartHostAll("Hello World", 6);
        }
        if (GUI.Button(new Rect(10, 110, 150, 100), "Join"))
        {
            if (matchMaker == null) matchMaker = gameObject.AddComponent<NetworkMatch>();

#if UNITY_5_4
            matchMaker.ListMatches(0, 1, "", true, 0, 0, OnMatchList);
#else
            matchMaker.ListMatches(0, 1, "", OnMatchList);
#endif
        }
        if (GUI.Button(new Rect(10, 210, 150, 100), "Disconnect"))
        {
            if (matchID != NetworkID.Invalid && matchmakingNodeID != NodeID.Invalid)
            {
                if (NetworkServer.active)
                {
#if UNITY_5_4
                    matchMaker.DestroyMatch(matchID, 0, OnMatchDropped);
#else
                    matchMaker.DestroyMatch(matchID, OnMatchDropped);
#endif
                }
                else
                {
#if UNITY_5_4
                    matchMaker.DropConnection(matchID, matchmakingNodeID, 0, OnMatchDropped);
#else
                    matchMaker.DropConnection(matchID, matchmakingNodeID, OnMatchDropped);
#endif
                }
            }

            if (NetworkServer.active)
            {
                NetworkServer.SetAllClientsNotReady();
                StopHost();
            }
            else
            {
                StopClient();
            }
        }

        if (NetworkServer.active)
        {
            if (GUI.Button(new Rect(10, 310, 150, 100), "Send To All"))
            {
                NetworkServer.SendToAll(MsgType.OtherTestMessage, new EmptyMessage());
            }
        }
        
        GUI.Label(new Rect(10, 410, 300, 100), "Is connected to Facilitator: " + natHelper.isConnectedToFacilitator);
    }*/

    public override void OnDoneConnectingToFacilitator(ulong guid)
    {
        if (guid == 0)
        {
            Debug.Log("Failed to connect to Facilitator");
        }
        else
        {
            Debug.Log("Facilitator connected");
        }
    }

    private void OnTestMessage(NetworkMessage netMsg)
    {
        Debug.Log("Received test message");
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        // Test sending a large message that requires fragmentation
        byte[] data = new byte[40000];
        for (int i = 0; i < data.Length; i++) data[i] = (byte)UnityEngine.Random.Range(0, 255);
        LargeMessage msg = new LargeMessage(data);

        conn.Send(MsgType.LargeMessage, msg);
    }

    /// <summary>
    /// Test AddPlayerForConnection and SpawnWithClientAuthority functionality
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    /// <param name="playerControllerId">Id of the new player.</param>
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

        GameObject otherOb = Instantiate(otherObPrefab);
        NetworkServer.SpawnWithClientAuthority(otherOb, conn);

        player.GetComponent<NATTraversalExamplePlayer>().RpcSetAssignedOb(otherOb.GetComponent<NetworkIdentity>().netId);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        NetworkServer.RegisterHandler(MsgType.OtherTestMessage, OnTestMessage);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        RegisterHandlerClient(MsgType.OtherTestMessage, OnTestMessage);
        RegisterHandlerClient(MsgType.LargeMessage, OnLargeMessage);
    }

    /// <summary>
    /// Test receiving large fragmented messages because why not
    /// </summary>
    /// <param name="msg">The MSG.</param>
    void OnLargeMessage(NetworkMessage msg)
    {
        LargeMessage largeMsg = msg.ReadMessage<LargeMessage>();
        Debug.Log("Received large message: " + largeMsg.lotsOfData.Length);
    }

}

class MsgType : NATTraversal.MsgType
{
    public static short LargeMessage = Highest + 1;
    public static short OtherTestMessage = Highest + 2;
}

class LargeMessage : MessageBase
{
    public byte[] lotsOfData;

    public LargeMessage() { }

    public LargeMessage(byte[] data)
    {
        lotsOfData = data;
    }
}
