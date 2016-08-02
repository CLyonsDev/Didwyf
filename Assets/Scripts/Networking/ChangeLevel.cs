using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ChangeLevel : NetworkBehaviour {

    public string nextScene = "TestBossFight";
    GameObject networkManagerGO;

	// Use this for initialization
	void Start () {
        networkManagerGO = GameObject.Find("NetworkManager_New");

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.P))
            CmdChangeScene(nextScene);
	}

    [Command]
    void CmdChangeScene(string nextScene)
    {
        RpcChangeScene(nextScene);
    }

    [ClientRpc]
    public void RpcChangeScene(string nextScene)
    {
        //GameObject netMan = ClientScene.FindLocalObject(netManagerID);
        GameObject netMan = GameObject.Find("NetworkManager_New");

        netMan.GetComponent<ExampleNetworkManager>().ServerChangeScene(nextScene);
    }
}
