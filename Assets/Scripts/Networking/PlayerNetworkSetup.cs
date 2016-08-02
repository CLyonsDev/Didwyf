using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerNetworkSetup : NetworkBehaviour {

	// Use this for initialization
	void Start() {
        /*
        ////////////////////////////////////////////////////////////////////////////////////////
        |TO-DO: MAKE A SINGLE SCRIPT THAT WHEN ENABLED ENABLES ALL OF THE BELOW ON AWAKE/START.|
        \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        */
        if (isLocalPlayer)
        {
            GetComponent<Movement>().enabled = true;
            GetComponent<Rotation>().enabled = true;
            GetComponent<Player>().enabled = true;
            //GetComponent<Player>().enabled = false;
            GetComponent<UIStatManager>().enabled = true;
            GetComponent<Attack>().enabled = true;
            GetComponent<Interact>().enabled = true;
            GetComponent<ChangeLevel>().enabled = true;
            transform.tag = "LocalPlayer";
        }
        else
        {
            transform.tag = "Player";
        }  
    }
}
