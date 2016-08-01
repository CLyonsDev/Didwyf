using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ArrowLogic : NetworkBehaviour {

    [HideInInspector] public EnemyBase eb;
    [HideInInspector] public EnemyAI ai;

    [SyncVar] public Transform target;

    Rigidbody rb;

    public float speed = 25f;

    [SyncVar] public bool shouldHit;

    [SyncVar] public bool isCrit = false;

    [SyncVar] public float modRoll = 0;

    [SyncVar] bool moving = true;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        if (!NetworkServer.active)
            return;

        if (!shouldHit)
        {
            transform.LookAt(target);
            transform.Rotate(0, 90, 0);
            GetComponent<BoxCollider>().isTrigger = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (!NetworkServer.active)
            return;

        if (!moving)
            return;

        if(shouldHit)
            TravelToPlayer();
        else
        {
            TravelStraight();
        }
    }

    void TravelToPlayer()
    {
        if (!rb.isKinematic)
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (target.GetComponent<Renderer>().enabled == false && GetComponent<Renderer>().enabled == true)
            return;

        transform.LookAt(target);
        transform.Rotate(0, 90, 0);
    }

    void TravelStraight()
    {
        //if (!rb.isKinematic)
            transform.position += -transform.right * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider col)
    {
        moving = false;

        //Debug.Log("Hit, ShouldHit = " + shouldHit);

        if (shouldHit)
        {
            float damage;
            rb.isKinematic = true;
            GetComponent<MeshCollider>().enabled = false;
            transform.Rotate(new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5)));
            if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                transform.SetParent(target, true);

                if (NetworkServer.active == true)
                {
                    if (isCrit)
                        damage = Mathf.Round(Random.Range(eb.totalDamageMin, eb.totalDamageMax));
                    else
                        damage = Mathf.Round(Random.Range(eb.totalDamageMin, eb.totalDamageMax));

                    target.gameObject.GetComponent<CharacterBase>().CmdReportAttack(GetComponent<NetworkIdentity>().netId, target.gameObject.GetComponent<NetworkIdentity>().netId, GameObject.Find("GameManager").GetComponent<NetworkIdentity>().netId, modRoll, damage, "an arrow");
                }
            }
        }

        DisableCollision();
    }

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log("Missed and collided with " + col.transform.name);

        moving = false;
        rb.useGravity = true;
        StartCoroutine(DisableCollision());
    }

    IEnumerator DisableCollision()
    {
        yield return new WaitForSeconds(1.5f);

        rb.isKinematic = true;
        gameObject.layer = LayerMask.NameToLayer("Ignore-Player-And-Enemy-Collision");
    }
}
