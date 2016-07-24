using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ArrowLogic : NetworkBehaviour {

    public Transform target;

    public float speed = 50;

    bool stuck;

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().AddForce(-transform.right * speed, ForceMode.Impulse);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!NetworkServer.active)
            return;

        if(stuck)
        {
            transform.SetParent(target, true);
        }

        transform.LookAt(target);
        transform.Rotate(0, 90, 0);
        
	}

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Collided with " + col.transform.name);
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<MeshCollider>().enabled = false;
        stuck = true;
    }
}
