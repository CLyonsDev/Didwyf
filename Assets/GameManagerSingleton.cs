using UnityEngine;
using System.Collections;

public class GameManagerSingleton : MonoBehaviour {

    private static GameManagerSingleton _instance;

	// Use this for initialization
	void Awake () {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
	}
}
