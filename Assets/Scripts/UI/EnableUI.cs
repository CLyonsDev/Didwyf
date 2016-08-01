using UnityEngine;
using System.Collections;

public class EnableUI : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        GameObject.Find("Canvas").SetActive(true);
	}
}
