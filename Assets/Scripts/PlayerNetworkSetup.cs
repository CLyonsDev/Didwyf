using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerNetworkSetup : NetworkBehaviour {

	// Use this for initialization
	void Start() {
        if(isLocalPlayer)
        {
            GetComponent<Movement>().enabled = true;
            GetComponent<Rotation>().enabled = true;
            GetComponent<Player>().enabled = true;
            //GetComponent<Player>().enabled = false;
            GetComponent<UIStatManager>().enabled = true;
            transform.tag = "LocalPlayer";

        }
        else
        {
            transform.tag = "Player";
        }  
    }
}
