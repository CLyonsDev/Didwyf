using UnityEngine;
using System.Collections;

public class SlotController : MonoBehaviour {

    GameObject inventoryPanel; //TODO: Make single reference in Game Manager.

    // Use this for initialization
    void Start () {
        inventoryPanel = GameObject.Find("InventoryPanel");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OooonMouseEnter()
    {
        inventoryPanel.GetComponent<InventoryController>().selectedSlot = this.transform;
    }
}
