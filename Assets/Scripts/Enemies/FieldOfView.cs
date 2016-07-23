using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public float alertRadius;

    public float patrolRadius;

    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public LayerMask alliedMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    void Start()
    {
        StartCoroutine("FindTargetsWithDelay", .2f);
    }


    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void Update()
    {
        if (GetComponent<EnemyAI>().attackingPlayer == false)
        {
            return;
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        List<Collider> targetsInViewRadius = new List<Collider>();
        targetsInViewRadius.AddRange(Physics.OverlapSphere(transform.position, viewRadius, targetMask));

        for (int i = 0; i < targetsInViewRadius.Count; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            
            if (target.gameObject.GetComponent<CharacterBase>().isDead)
            {
                targetsInViewRadius.RemoveAt(i);
                return;
            }

            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                    //Debug.Log("Found someone!");
                    AlertAllies();
                }
            }
        }
    }

    void AlertAllies()
    {
        GetComponent<NavMeshAgent>().SetDestination(visibleTargets[0].position);

        Collider[] alliesInAlertRadius = Physics.OverlapSphere(transform.position, alertRadius, alliedMask);

        GetComponent<NavMeshAgent>().speed = GetComponent<EnemyAI>().runSpeed;
        GetComponent<NavMeshAgent>().SetDestination(visibleTargets[0].position);
        GetComponent<EnemyAI>().attackingPlayer = true;
        GetComponent<EnemyAI>().target = visibleTargets[0];
        GetComponent<EnemyAI>().range = GetComponent<EnemyAI>().attackRange;

        for (int i = 0; i < alliesInAlertRadius.Length; i++)
        {
            NavMeshAgent allyNavMesh = alliesInAlertRadius[i].GetComponent<NavMeshAgent>();
            EnemyAI ai = alliesInAlertRadius[i].GetComponent<EnemyAI>();

            allyNavMesh.speed = ai.runSpeed;
            allyNavMesh.SetDestination(visibleTargets[0].position);
            ai.attackingPlayer = true;
            ai.target = visibleTargets[0];
            ai.range = ai.attackRange;
        }
    }


    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}