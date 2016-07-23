using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemyAI : NetworkBehaviour {

    public string enemyName;

    public bool patrol;
    public bool attackingPlayer = false;

    float remainingDistance;
    public float runSpeed;
    public float patrolSpeed;
    public float rotationSpeed = 10f;

    public float attackRange;
    public float patrolRange;
    [HideInInspector] public float range;

    public float rateOfAttack = 1;
    public float timer = 0;

    public Transform[] waypoints;
    public Transform target;

    NavMeshAgent agent;

    EnemyBase eb;

    // Use this for initialization
    void Start () {
        if (!patrol)
            return;

        range = patrolRange;
        agent = GetComponent<NavMeshAgent>();
        eb = GetComponent<EnemyBase>();
        target = waypoints[Random.Range(0, waypoints.Length)];
        //agent.updateRotation = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (timer < rateOfAttack)
            timer += Time.deltaTime;
        else if (timer > rateOfAttack)
            timer = rateOfAttack;

        if (!patrol)
            return;

        MoveTowards(target, range);

        if (!attackingPlayer && IsInRange(target, patrolRange))
        {
            Transform tempTarg = target;
            while(target == tempTarg)
            {
                target = waypoints[Random.Range(0, waypoints.Length)];
            }
        }

        if(attackingPlayer && IsInRange(target, attackRange))
        {
            if(target.GetComponent<MeshRenderer>().enabled != true)
            {
                target = waypoints[Random.Range(0, waypoints.Length)];
                agent.speed = patrolSpeed;
                return;
            }

            if(timer >= rateOfAttack)
            {
                Debug.Log("Attacking");
                target.GetComponent<CharacterBase>().CmdReportDamage(target.GetComponent<NetworkIdentity>().netId, Mathf.Round(Random.Range(eb.totalDamageMin, eb.totalDamageMax)), enemyName);
                timer = 0;
            }
        }

        

        RotateTowards(target);

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
