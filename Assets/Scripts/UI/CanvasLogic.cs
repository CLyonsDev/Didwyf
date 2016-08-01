using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CanvasLogic : NetworkBehaviour {

	void Start () {
       //if (!isClient)
            //Destroy(transform.parent.gameObject);

            transform.localPosition = new Vector3(0, 0, 0);
            transform.FindChild("InventoryPanel").localPosition = new Vector3(249.35f, 166, 0);

        
    }
}
