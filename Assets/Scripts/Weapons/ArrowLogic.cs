using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ArrowLogic : NetworkBehaviour {

    [HideInInspector] public EnemyBase eb;
    [HideInInspector] public EnemyAI ai;

    public Transform target;

    public float speed = 50;

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().AddForce(-transform.right * speed, ForceMode.Impulse);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!NetworkServer.active)
            return;

        transform.LookAt(target);
        transform.Rotate(0, 90, 0);
	}

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Collided with " + col.transform.name);
        transform.SetParent(target, true);
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<MeshCollider>().enabled = false;
        target.GetComponent<CharacterBase>().CmdReportDamage(target.GetComponent<NetworkIdentity>().netId, Mathf.Round(Random.Range(eb.totalDamageMin, eb.totalDamageMax)), ai.enemyName);
    }
}
