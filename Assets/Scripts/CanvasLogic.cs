using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CanvasLogic : NetworkBehaviour {

	void Start () {
        if (!isClient)
            Destroy(gameObject);
    }
}
