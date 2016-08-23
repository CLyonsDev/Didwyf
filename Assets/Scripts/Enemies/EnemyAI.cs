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

    [SyncVar] public bool aggressive;

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

        if (GetComponent<EnemyBase>().isDead)
            return;

        if(target != null)
            RotateTowards(target);

        if (patrol && !attackingPlayer)
        {
            MoveTowards(target, range);
        }

        if (!attackingPlayer && IsInRange(target, patrolRange) && waypoints.Length > 0 && waypoints[0] != null)
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

        if(attackingPlayer && IsInRange(target, attackRange) && !GetComponent<EnemyBase>().isDead)
        {
            if(target.GetComponent<MeshRenderer>().enabled != true)
            {
                agent.speed = patrolSpeed;
                if(waypoints.Length > 0 && waypoints[0] != null)
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
        int modRoll;

        roll = (Random.Range(1, 20));

        if (!unitIsRanged)
            modRoll = roll + (GetComponent<EnemyBase>().strength / 2);
        else
        {
            modRoll = roll + (GetComponent<EnemyBase>().dexterity / 2);
        }

        Debug.Log(transform.name + " is attacking " + target.transform.name + " (" + roll + " (" + modRoll + "))!");


        //Debug.Log("Rolled a " + roll + " against an AC of " + ac + ".");

        //Debug.Log("The attack hits!");

        if (unitIsRanged)
        {
            RangedHit(modRoll >= ac || roll == 20, roll == 20, modRoll);
        }
        else
        {
            MeleeHit(roll == 20, modRoll);
        }
    }

    private void MeleeHit(bool isCrit, int modRoll)
    {
        float damage = 0f;

        GameObject gameManager = GameObject.Find("GameManager");
        if (isCrit)
        {
            damage = Mathf.Round(Random.Range(eb.totalDamageMin, eb.totalDamageMax)) * GetComponent<EnemyBase>().weaponCritModifier;
            //target.GetComponent<CharacterBase>().CmdReportDamage(target.GetComponent<NetworkIdentity>().netId, damage, enemyName);
        }   
        else
        {
            damage = Mathf.Round(Random.Range(eb.totalDamageMin, eb.totalDamageMax));
            //target.GetComponent<CharacterBase>().CmdReportDamage(target.GetComponent<NetworkIdentity>().netId, damage, enemyName);
        }

        target.GetComponent<CharacterBase>().CmdReportAttack(GetComponent<NetworkIdentity>().netId, target.GetComponent<NetworkIdentity>().netId, gameManager.GetComponent<NetworkIdentity>().netId, modRoll, damage, transform.name);
    }

    private void RangedHit(bool hits, bool isCrit, float modRoll)
    {
        weapon = transform.GetChild(0);
        GameObject projectileSpawner = weapon.transform.GetChild(0).gameObject;
        GameObject proj = Instantiate(projectile, projectileSpawner.transform.position, transform.rotation) as GameObject;
        proj.GetComponent<ArrowLogic>().target = target;
        proj.GetComponent<ArrowLogic>().modRoll = modRoll;
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
