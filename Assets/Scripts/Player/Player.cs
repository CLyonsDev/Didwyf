using UnityEngine;
using System.Collections;

public class Player : CharacterBase {

	// Use this for initialization
	void Start () {
        RpcCalcStats();
        Debug.Log("RpcCalcStats");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            RpcTakeDamage(5, "Environment");
        }

        if(Input.GetKeyDown(KeyCode.N))
        {
            RpcRespawn();
        }
    }
}
