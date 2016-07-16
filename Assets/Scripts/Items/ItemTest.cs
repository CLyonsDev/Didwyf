using UnityEngine;
using System.Collections;

public class ItemTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    foreach (ItemEntry item in XMLManager.ins.itemDB.list)
        {
            //Debug.Log(item.itemName + "(" + item.itemID.ToString() + ")");
        }
	}
	
	// Update is called once per frame
	void Update () {

	}
}
