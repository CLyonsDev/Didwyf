using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class DisplayIP : MonoBehaviour {
	// Update is called once per frame
	void Update () {
        GetComponent<Text>().text = "Your IP Address is: \n" + Network.player.ipAddress;
	}
}
