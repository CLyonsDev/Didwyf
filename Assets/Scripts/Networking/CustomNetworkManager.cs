using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomNetworkManager : NetworkManager {

    /*public void StartupHost()
    {
        if (!NetworkClient.active && !NetworkServer.active)
        {
            SetPort();
            NetworkManager.singleton.StartHost();
            Debug.Log(Network.player.externalIP + " " + Network.player.externalPort);
            Debug.Log(Network.player.ipAddress);
        }
        else
        Debug.Log("couldnt start host");
    }

    public void JoinGame()
    {
        if (!NetworkClient.active && !NetworkServer.active)
        {
            SetIPAddress();
            SetPort();
            NetworkManager.singleton.StartClient();
        }
        else
        {
            Debug.LogError("what the shit");
        }
    }

    public void LeaveGame()
    {
        UnityEngine.Cursor.visible = true;
        NetworkManager.singleton.StopHost();
    }

    void SetIPAddress()
    {
        string ipAddress = GameObject.Find("InputFieldIPAddress").transform.FindChild("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    void SetPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            StartCoroutine(SetupMenuButtons());
        }
    }

    IEnumerator SetupMenuButtons()
    {
        yield return new WaitForSeconds(0.15f);

        GameObject.Find("HostButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("HostButton").GetComponent<Button>().onClick.AddListener(StartupHost);

        GameObject.Find("JoinButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("JoinButton").GetComponent<Button>().onClick.AddListener(JoinGame);

    }*/
}
