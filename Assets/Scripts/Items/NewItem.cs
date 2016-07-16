using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class NewItem : NetworkBehaviour {

    GameObject inventoryPanel;

    public Sprite itemSprite;

    public ItemEntry currentItem;

    bool clicking = false;

	// Use this for initialization
	void Start () {
        GetComponent<Image>().sprite = itemSprite;
        inventoryPanel = GameObject.Find("InventoryPanel");
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
            clicking = true;
        else if (Input.GetMouseButtonUp(0))
            clicking = false;
    }

    public void SendHoveredItem()
    {
        if (!clicking)
            inventoryPanel.GetComponent<InventoryController>().selectedItem = this.transform;
    }

    public void ClearHoveredItem()
    {
        if (!clicking)
            inventoryPanel.GetComponent<InventoryController>().selectedItem = null;
    }
}
