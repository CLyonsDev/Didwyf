using UnityEngine;
using System.Collections;

public class CanvasLogic : MonoBehaviour {

	void Start () {
            transform.localPosition = new Vector3(0, 0, 0);
            transform.FindChild("InventoryPanel").localPosition = new Vector3(249.35f, 166, 0);
    }
}
