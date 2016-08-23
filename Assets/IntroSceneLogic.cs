using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class IntroSceneLogic : NetworkBehaviour {

    public GameObject[] enemies, guardSpawns;

    public GameObject guardPrefab;

    public float guardSpawnDelay = 3f;

    [SyncVar] public bool guardsSpawned = false;


	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        enemies = GameObject.FindGameObjectsWithTag("NPC");

        if (enemies.Length == 0 && !guardsSpawned)
        {
            guardsSpawned = true;
            Invoke("SpawnGuards", guardSpawnDelay);
        }
	}

    public void SpawnGuards()
    {
        foreach(GameObject go in guardSpawns)
        {
            GameObject guard = Instantiate(guardPrefab, go.transform.position, go.transform.rotation) as GameObject;

            if (NetworkServer.active)
                NetworkServer.Spawn(guard);
        }
    }
}
