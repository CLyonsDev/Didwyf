using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking.Match;

public class NetworkManager_Custom_V2 : NATTraversal.NetworkManager {

    /*public void StartupHost()
    {
        SetPort();
        StartHostAll("Test", 4);
        Debug.Log(NetworkServer.listenPort);
        Debug.Log(client.serverPort);
        Debug.Log(client.serverIp);
    }

    public void JoinGame()
    {
        SetIPAddress();
        SetPort();
        StartClientAll();
    }

    public void SetIPAddress()
    {
        string ipAddress = GameObject.Find("InputFieldIPAddress").transform.FindChild("Text").GetComponent<Text>().text;
        //NetworkManager.singleton.networkAddress = ipAddress;
        networkAddress = ipAddress;
        Debug.Log(ipAddress);
    }

    void SetPort()
    {
        //NetworkManager.singleton.networkPort = 7777;
        networkPort = 7777;
    }

    void OnLevelWasLoaded(int level)
    {
        if(level == 0)
        {
            //SetupMenuSceneButtons();
            StartCoroutine(SetupMenuSceneButtons());
        }else
        {
            SetupOtherSceneButtons();
        }
    }

    IEnumerator SetupMenuSceneButtons()
    {
        yield return new WaitForSeconds(0.15f);
        GameObject.Find("HostButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("HostButton").GetComponent<Button>().onClick.AddListener(StartupHost);

        GameObject.Find("JoinButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("JoinButton").GetComponent<Button>().onClick.AddListener(JoinGame);
    }

    void SetupOtherSceneButtons()
    {
        GameObject.Find("DisconnectButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("DisconnectButton").GetComponent<Button>().onClick.AddListener(NetworkManager.singleton.StopHost);
    }

    void StartGame()
    {
        StartHostAll("Test", 4);
        matchMaker.CreateMatch("Test", 4, true, "", OnMatchCreate);
    }

    void JoinGame()
    {

    }

    public override void OnMatchList(ListMatchResponse matchList)
    {
        MatchDesc match
    }*/
}
