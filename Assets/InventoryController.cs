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
                if(selectedSlot.childCount > 0)
                {
                    selectedSlot.GetChild(0).SetParent(originalSlot.transform);
                    foreach (Transform t in originalSlot)
                        t.localPosition = Vector3.zero;
                }
                selectedItem.SetParent(selectedSlot);
            }
            selectedItem.localPosition = Vector3.zero;
        }
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
            NetworkServer.Spawn(newItem);

        }
    }
}
