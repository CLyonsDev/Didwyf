using UnityEngine;
using System.Collections;

public class TentacleLogic : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        //LOWER DEPTH OF SEGMENT COLLIDERS!
        if(col.transform.tag == "Player")
            Debug.Log("Collided with " + col.transform.name);
    }
}
