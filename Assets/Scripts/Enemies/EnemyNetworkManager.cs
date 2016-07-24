using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemyNetworkManager : NetworkBehaviour {

	// Use this for initialization
	void Start () {
        if(NetworkServer.active)
        {
            Debug.Log("PalapaKalapa");
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<FieldOfView>().enabled = true;
            GetComponent<EnemyAI>().enabled = true;
        }
	}
	
	// Update is called once per frame
	void Update () {

    }
}
