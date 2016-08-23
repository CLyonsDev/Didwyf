using UnityEngine;
using System.Collections;

public class CombatCanvasSingleton : MonoBehaviour {

    private static CombatCanvasSingleton _instance;

	// Use this for initialization
	void Awake () {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
	}
}
