using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryController : NetworkBehaviour {

    public Transform selectedItem, selectedSlot, originalSlot;

    public GameObject slotPrefab, itemPrefab, localPlayer;

    GameObject[] inventorySlots;

    RaycastHit hit;

	// Use this for initialization
	void Start () {
        StartCoroutine(LateStart());
	}
	
	// Update is called once per frame
	void Update () {

        if (localPlayer == null)
        {
            localPlayer = GameObject.FindGameObjectWithTag("LocalPlayer");
        }

            //******************//
            //  Click Pressed   //
            //******************//

            if (Input.GetMouseButtonDown(0) && selectedItem != null)
        {
            originalSlot = selectedItem.parent;
            //selectedItem.parent = selectedItem.parent.parent;
            //selectedItem.SetAsLastSibling();
        }

        //*******************//
        //  Click Released   //
        //*******************//

        if (Input.GetMouseButton(0) && selectedItem != null)
        {
            selectedItem.position = Input.mousePosition;
            selectedItem.GetComponent<Image>().raycastTarget = false;

        }

        else if (Input.GetMouseButtonUp(0) && selectedItem != null)
        {
            selectedItem.GetComponent<Image>().raycastTarget = true;

            if (selectedItem == null)
                selectedItem.parent = originalSlot;
            else
            {
                //Swap item positions
                if(selectedSlot.childCount > 0 && selectedSlot.GetChild(0).GetComponent<Text>() == null)
                {
                    selectedSlot.GetChild(0).SetParent(originalSlot.transform);
                    foreach (Transform t in originalSlot)
                        t.localPosition = Vector3.zero;
                }
                selectedItem.SetParent(selectedSlot);
            }
            selectedItem.localPosition = Vector3.zero;

            if(originalSlot.transform.tag == "CharacterEquipmentSlot")
            {
                EquipItem(new ItemEntry());
            }

            if (selectedSlot.transform.tag == "CharacterEquipmentSlot") //If we are placing the item into a "Hand Slot"
            {
                EquipItem(selectedItem.GetComponent<NewItem>().currentItem);
            }

            if (selectedSlot.transform.tag == "Trash")
            {
                Debug.LogWarning("Destroying Item");
                localPlayer.GetComponent<Player>().RemoveItem(localPlayer.GetComponent<NetworkIdentity>().netId, GameObject.Find("GameManager").GetComponent<NetworkIdentity>().netId, selectedItem.GetComponent<NewItem>().currentItem);
                Destroy(selectedItem.gameObject);
            }
        }
    }

    void EquipItem(ItemEntry equipedItem)
    {
        localPlayer.GetComponent<CharacterBase>().EquipItem(equipedItem);
    }

    [Command]
    void CmdEquipItem(NetworkInstanceId playerID, ItemEntry equipedItem)
    {
        RpcEquipItem(playerID, equipedItem);
    }

    [ClientRpc]
    void RpcEquipItem(NetworkInstanceId playerID, ItemEntry equipedItem)
    {
        GameObject player = ClientScene.FindLocalObject(playerID);

        player.GetComponent<CharacterBase>().equipedItem = equipedItem;
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.1f);

        localPlayer = GameObject.FindGameObjectWithTag("LocalPlayer");
        if (localPlayer == null)
        {
            Debug.LogError("Could not find a local player!");
            yield return null;
        }


        inventorySlots = GameObject.FindGameObjectsWithTag("ItemSlot");

        for (int i = 0; i < localPlayer.GetComponent<Player>().inventory.Count; i++)
        {
            GameObject newItem = Instantiate(itemPrefab) as GameObject;
            newItem.transform.SetParent(inventorySlots[i].transform, false);
            newItem.GetComponent<ItemLogic>().curItem = ItemDatabase_OLD.itemList[i];
            newItem.name = ItemDatabase_OLD.itemList[i].itemName;
            if(NetworkServer.active)
                NetworkServer.Spawn(newItem);
        }
    }
}
