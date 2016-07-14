using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class ItemLogic : NetworkBehaviour {

    GameObject inventoryPanel;

    public Item curItem;

    Sprite[] spriteList;

    [SyncVar]
    bool clicking = false;

    // Use this for initialization
    void Start () {
        spriteList = GameObject.Find("InventoryScreen").GetComponent<ItemDatabase>().sprites;
        inventoryPanel = GameObject.Find("InventoryPanel");
        //Debug.Log(curItem.itemName);
        if (curItem.itemSprite == null)
            GetComponent<Image>().sprite = spriteList[curItem.itemSpriteIndex];
        else
            GetComponent<Image>().sprite = curItem.itemSprite;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
            clicking = true;
        else if (Input.GetMouseButtonUp(0))
            clicking = false;
	}

    [ClientRpc]
    public void RpcSendHoveredItem()
    {
        if(!clicking)
            inventoryPanel.GetComponent<InventoryController>().selectedItem = this.transform;
    }

    [ClientRpc]
    public void RpcClearHoveredItem()
    {
        if(!clicking)
            inventoryPanel.GetComponent<InventoryController>().selectedItem = null;
    }
}
