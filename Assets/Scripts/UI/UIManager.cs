using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class UIManager : NetworkBehaviour
{

    public GameObject InventoryGO;
    public GameObject player;

	// Use this for initialization
	void Start () {
        RpcToggleInventory();
	}
	
	// Update is called once per frame
	void Update () {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("LocalPlayer");

    
        if (Input.GetKeyDown(KeyCode.I))
        {
            RpcToggleInventory();
        }
	}

    [ClientRpc]
    void RpcToggleInventory()
    {
        if(player != null)
        {
            player.GetComponent<Movement>().enabled = (!InventoryGO.activeInHierarchy);
            player.GetComponent<Rotation>().enabled = (!InventoryGO.activeInHierarchy);
        }

        InventoryGO.SetActive(!InventoryGO.activeInHierarchy);
        //InventoryGO.GetComponent<InventoryController>().
    }
}
