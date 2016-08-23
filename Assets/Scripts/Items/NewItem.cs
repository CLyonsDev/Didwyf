using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class NewItem : NetworkBehaviour {

    GameObject inventoryPanel, canvasGO;

    public GameObject popupGO;

    public Sprite itemSprite;

    public ItemEntry currentItem;

    bool clicking = false;

	// Use this for initialization
	void Start () {
        GetComponent<Image>().sprite = itemSprite;
        inventoryPanel = GameObject.Find("InventoryPanel");
        canvasGO = GameObject.Find("Canvas");
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            clicking = true;
            Destroy(GameObject.Find("InfoBox(Clone)"));
        } 
        else if (Input.GetMouseButtonUp(0))
            clicking = false;
    }

    public void SendHoveredItem()
    {
        if (!clicking)
        {
            inventoryPanel.GetComponent<InventoryController>().selectedItem = this.transform;
            GameObject newPopup = Instantiate(popupGO, Input.mousePosition, transform.rotation) as GameObject;
            newPopup.transform.SetParent(canvasGO.transform);
            newPopup.transform.GetChild(0).GetComponent<Text>().text = "Name: " + currentItem.itemName;

            if (currentItem.type == ItemType.Weapon)
                newPopup.transform.GetChild(1).GetComponent<Text>().text = "Damage: " + currentItem.damageMin + " - " + currentItem.damageMax + " (x" + currentItem.critModifier + ")";
            else if (currentItem.type == ItemType.Armor)
                newPopup.transform.GetChild(1).GetComponent<Text>().text = "Armor Rating: " + currentItem.armor;
            else
                Destroy(newPopup.transform.GetChild(1).gameObject);

            newPopup.transform.GetChild(2).GetComponent<Text>().text = "Type: " + currentItem.type;
            newPopup.transform.GetChild(3).GetComponent<Text>().text = "Value: " + currentItem.value + " Gold";
            newPopup.transform.GetChild(4).GetComponent<Text>().text = "Rarity: " + currentItem.rarity;


            newPopup.transform.localScale = new Vector3(1, 1, 1);
        } 
    }

    public void ClearHoveredItem()
    {
        if (!clicking)
        {
            inventoryPanel.GetComponent<InventoryController>().selectedItem = null;
            Destroy(GameObject.Find("InfoBox(Clone)"));
        }
    }
}
