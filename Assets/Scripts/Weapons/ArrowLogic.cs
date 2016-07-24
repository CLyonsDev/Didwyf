using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ArrowLogic : NetworkBehaviour {

    public Transform target;

    public float speed = 10f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (!NetworkServer.active)
            return;

        transform.LookAt(target);
        //GetComponent<Rigidbody>().AddForce(-transform.forward * speed, ForceMode.VelocityChange);
	}

    void OnTriggerEnter(Collider col)
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
