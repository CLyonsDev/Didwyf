using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemyAI : NetworkBehaviour {

    public string enemyName;

    public bool patrol = true;
    public bool attackingPlayer = false;
    public bool unitIsRanged = false;

    float remainingDistance;
    public float runSpeed;
    public float patrolSpeed;
    public float rotationSpeed = 10f;

    public float attackRange;
    public float patrolRange;
    public float range;

    public float rateOfAttack = 1;
    public float timer = 0;

    public Transform[] waypoints;
    [SyncVar] public Transform target;

    public Transform weapon;
    public GameObject projectile;

    NavMeshAgent agent;

    EnemyBase eb;

    // Use this for initialization
    void Start () {

        agent = GetComponent<NavMeshAgent>();
        eb = GetComponent<EnemyBase>();

        if (!patrol)
            return;

        range = patrolRange;
        target = waypoints[Random.Range(0, waypoints.Length)];
        //agent.updateRotation = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (timer < rateOfAttack)
            timer += Time.deltaTime;
        else if (timer > rateOfAttack)
            timer = rateOfAttack;

        if (target == null)
            return;

        RotateTowards(target);

        if (patrol && !attackingPlayer)
        {
            MoveTowards(target, range);
        }

        if (!attackingPlayer && IsInRange(target, patrolRange))
        {
            Transform tempTarg = target;
            while(target == tempTarg)
            {
                target = waypoints[Random.Range(0, waypoints.Length)];
            }
        }

        if (attackingPlayer)
        {
            MoveTowards(target, range);
        }

        if(attackingPlayer && IsInRange(target, attackRange))
        {
            if(target.GetComponent<MeshRenderer>().enabled != true)
            {
                agent.speed = patrolSpeed;
                if(waypoints.Length > 0)
                    target = waypoints[Random.Range(0, waypoints.Length)];
                attackingPlayer = false;
                return;
            }

            if(timer >= rateOfAttack)
            {
                Attack();
                timer = 0;
            }
        }
    }

    private void Attack()
    {
        int ac = target.GetComponent<CharacterBase>().armorRating;

        int roll;

        if (!unitIsRanged)
            roll = (Random.Range(1, 20) + GetComponent<EnemyBase>().strength);
        else
        {
            roll = (Random.Range(1, 20) + GetComponent<EnemyBase>().dexterity);
        }  

        Debug.Log("Rolled a " + roll + " against an AC of " + ac + ".");

        Debug.Log("The attack hits!");

        if (unitIsRanged)
        {
            RangedHit(roll < ac);
        }
        else
        {
            if (roll < ac)
                return;
            MeleeHit();
        }
    }

    private void MeleeHit()
    {
        target.GetComponent<CharacterBase>().CmdReportDamage(target.GetComponent<NetworkIdentity>().netId, Mathf.Round(Random.Range(eb.totalDamageMin, eb.totalDamageMax)), enemyName); 
    }

    private void RangedHit(bool hits)
    {
        //target.GetComponent<CharacterBase>().CmdReportDamage(target.GetComponent<NetworkIdentity>().netId, Mathf.Round(Random.Range(eb.totalDamageMin, eb.totalDamageMax)), enemyName);
        weapon = transform.GetChild(0);
        GameObject projectileSpawner = weapon.transform.GetChild(0).gameObject;
        GameObject proj = Instantiate(projectile, projectileSpawner.transform.position, transform.rotation) as GameObject;
        proj.GetComponent<ArrowLogic>().target = target;
        proj.GetComponent<ArrowLogic>().eb = GetComponent<EnemyBase>();
        proj.GetComponent<ArrowLogic>().ai = GetComponent<EnemyAI>();
        NetworkServer.Spawn(proj);

        proj.GetComponent<ArrowLogic>().shouldHit = hits;
    }

    private void MoveTowards(Transform target, float range)
    {
        agent.SetDestination(target.position);
        agent.stoppingDistance = range;
    }

    private bool IsInRange(Transform target, float distance)
    {
        float dist = Vector3.Distance(transform.position, target.position);
        return dist <= distance;
    }

    private void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
    }
}
