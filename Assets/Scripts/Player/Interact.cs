using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Interact : NetworkBehaviour {

    public LayerMask lm;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, lm))
            {
                if (hit.transform.parent.transform.tag == "SpinnyThing")
                {
                    //Debug.Log("Spinnnnnnnnnnn");
                    CmdSpinThing(hit.transform.parent.transform.gameObject.GetComponent<NetworkIdentity>().netId, GameObject.Find("PuzzleManager").GetComponent<NetworkIdentity>().netId);
                }
            }
        }
	}

    [Command]
    void CmdSpinThing(NetworkInstanceId netID, NetworkInstanceId puzzleManagerID)
    {
        RpcSpinThing(netID, puzzleManagerID);
    }

    [ClientRpc]
    void RpcSpinThing(NetworkInstanceId netID, NetworkInstanceId puzzleManagerID)
    {
        ClientScene.FindLocalObject(netID).GetComponent<SpinnyThing>().Spin();
        ClientScene.FindLocalObject(puzzleManagerID).GetComponent<SpinnyPuzzle>().CheckPassCode();
    }
}
