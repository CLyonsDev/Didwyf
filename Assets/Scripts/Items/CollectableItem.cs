using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CollectableItem : NetworkBehaviour {

    public int itemIndex;

    [SerializeField]
    ItemEntry item;

	// Use this for initialization
	void Start ()
    {
        item = XMLManager.ins.itemDB.list[itemIndex];
    }
}
