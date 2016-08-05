using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HealFountain : NetworkBehaviour {

    GameObject gm;

    public float healMin;
    public float healMax;
    public float duration;
    public float interval;

    void Start()
    {
        gm = GameObject.Find("GameManager");
    }

	void OnTriggerEnter(Collider col)
    {
        Debug.Log("Hit a thingey.");
        if (col.gameObject.layer == LayerMask.NameToLayer("Player") && !col.GetComponent<CharacterBase>().healingOverTime)
        {
            Debug.Log(col.transform.name + " entered Healy Fountain");
            col.GetComponent<CharacterBase>().StartHoT(gm.GetComponent<NetworkIdentity>().netId, col.GetComponent<NetworkIdentity>().netId, healMin, healMax, duration, interval, "Magic Fountain");
        }  
    }
}
