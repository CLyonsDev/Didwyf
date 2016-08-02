using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SpinnyPuzzle : NetworkBehaviour {

    public GameObject fireTrapParticleEmitter;

    GameObject[] SpinnyThings, pillars;

    [SerializeField] int[] passCode;

    [SerializeField] [SyncVar] int correctEntries;

    float damage = 3;

	// Use this for initialization
	void Start () {
        SpinnyThings = GameObject.FindGameObjectsWithTag("SpinnyThing");
        pillars = GameObject.FindGameObjectsWithTag("TrapPillar");
    }
	
	// Update is called once per frame
	void Update () { 
	}

    public void CheckPassCode()
    {
        correctEntries = 0;
        for(int i = 0; i < SpinnyThings.Length; i++)
        {
            if(SpinnyThings[i].GetComponent<SpinnyThing>().currentSymbol == passCode[i])
            {
                Debug.Log("Passcode incorrect!");
                correctEntries++;
            }
        }

        if(correctEntries >= passCode.Length)
        {
            Debug.LogWarning("PASSCODE CORRECT!");
            if (NetworkServer.active == false)
                return;
            for (int i = 0; i < pillars.Length; i++)
            {
                GameObject trap = Instantiate(fireTrapParticleEmitter, new Vector3(pillars[i].transform.position.x, pillars[i].transform.position.y + 2.5f, pillars[i].transform.position.z), Quaternion.identity) as GameObject;
                trap.transform.Rotate(90, 0, 0);
                NetworkServer.Spawn(trap);
            }
            GameObject localPlayer = GameObject.FindGameObjectWithTag("LocalPlayer");
            GameObject[] otherPlayers = GameObject.FindGameObjectsWithTag("Player");

            localPlayer.GetComponent<CharacterBase>().StartDoT(localPlayer.GetComponent<NetworkIdentity>().netId, localPlayer.GetComponent<NetworkIdentity>().netId, Mathf.Round(damage), 100, 1f, "Fire Trap");
            localPlayer.GetComponent<CharacterBase>().ClearInventory();
            foreach (GameObject target in otherPlayers)
            {
                target.GetComponent<CharacterBase>().StartDoT(localPlayer.GetComponent<NetworkIdentity>().netId, localPlayer.GetComponent<NetworkIdentity>().netId, Mathf.Round(damage), 100, 1f, "Fire Trap");
                target.GetComponent<CharacterBase>().ClearInventory();
            }
        }
    }
}
