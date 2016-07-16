using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CollectLoot : NetworkBehaviour {

    GameObject player;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("LocalPlayer");
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if(hit.transform.tag == "Loot")
                {
                    //Debug.Log("Avast mateys, here be loot!");
                    if(Vector3.Distance(hit.transform.position, player.transform.position) <= 5)
                    {
                        Debug.Log("Picked up " + hit.transform.name);
                        ItemDatabase_OLD._instance.RpcAddItem(hit.transform.gameObject.GetComponent<Item>());
                        Destroy(hit.transform.gameObject, 0.1f);
                    }
                    else
                    {
                        Debug.Log(Vector3.Distance(hit.transform.position, player.transform.position));
                    }
                }
            }
        }
	}
}
