using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ArrowLogic : NetworkBehaviour {

    [HideInInspector] public EnemyBase eb;
    [HideInInspector] public EnemyAI ai;

    [SyncVar] public Transform target;

    Rigidbody rb;

    public float speed = 25f;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        if (!NetworkServer.active)
            return;
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (!NetworkServer.active)
            return;
        //GetComponent<Rigidbody>().AddForce(-transform.right * speed / 50, ForceMode.Impulse);

        //rb.AddForce((target.position - transform.position) * (speed / 75), ForceMode.VelocityChange);
        if(!rb.isKinematic)
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (target.GetComponent<Renderer>().enabled == false && GetComponent<Renderer>().enabled == true)
            return;
        transform.LookAt(target);
        transform.Rotate(0, 90, 0);  
        //GetComponent<Rigidbody>().AddForce(-transform.right * speed, ForceMode.VelocityChange);
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Collided with " + col.transform.name);
        rb.isKinematic = true;
        GetComponent<MeshCollider>().enabled = false;
        transform.Rotate(new Vector3(Random.Range(-45, 45), Random.Range(-15, 15), Random.Range(-45, 45)));
        if(col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            transform.SetParent(target, true);
            
            if (NetworkServer.active == true)
                target.gameObject.GetComponent<CharacterBase>().CmdReportDamage(target.GetComponent<NetworkIdentity>().netId, Mathf.Round(Random.Range(eb.totalDamageMin, eb.totalDamageMax)), ai.enemyName);
        }      
    }
}
