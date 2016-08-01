using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Attack : NetworkBehaviour {

    public GameObject damageNumbers, gameManagerGO;

    public float attackDelay = 0f;
    public float attackTimer = 0;

    public float minDamage = 0;
    public float maxDamage = 0;
    public float critMod = 0;

    public LayerMask lm;

    CharacterBase cb;
    Player playerScript;

	// Use this for initialization
	void Start () {
        cb = GetComponent<CharacterBase>();
        playerScript = GetComponent<Player>();

        gameManagerGO = GameObject.Find("GameManager");

        attackDelay = cb.weaponAttackDelay;
        attackTimer = attackDelay;
	}
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
            return;


        if (attackTimer < attackDelay)
            attackTimer += Time.deltaTime;
        else if (attackTimer > attackDelay)
            attackTimer = attackDelay;

        if(Input.GetMouseButtonDown(0) && attackTimer == attackDelay)
        {
            Debug.Log("Swinging Weapon...");
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity) && hit.transform.tag == "Enemy")
            {
                if(Vector3.Distance(transform.position, hit.transform.position) <= cb.weaponRange)
                {
                    Debug.Log("Ray has hit!");
                    CalculateAttack(hit.transform.gameObject);
                }
                else
                {
                    Debug.Log("Enemy " + hit.transform.name + " is out of range!");
                }
                
            }
        }
	}

    void CalculateAttack(GameObject target)
    {
        EnemyBase eb = target.GetComponent<EnemyBase>();

        int roll = Random.Range(1, 20);
        int modRoll = roll + (cb.strength / 2);

        bool didHit = modRoll > eb.armorRating;

        float damageRoll = 0;

        Debug.Log(modRoll + "(" + roll + ")" + " against an AC of " + eb.armorRating);

        if(didHit)
        {
            damageRoll = Mathf.Round(Random.Range(cb.totalDamageMin, cb.totalDamageMax));
            eb.TakeDamage(GetComponent<NetworkIdentity>().netId, damageRoll, cb.playerName);
        }


        CmdSpawnDamageBox(didHit, damageRoll, 1, target.transform.position, gameManagerGO.GetComponent<NetworkIdentity>().netId);

        attackTimer = 0;
    }

    [Command]
    void CmdSpawnDamageBox(bool hit, float damageAmt, int characterType, Vector3 spawnPos, NetworkInstanceId gameManagerID)
    {
        RpcSpawnDamageBox(hit, damageAmt, characterType, spawnPos, gameManagerID);
    }

    [ClientRpc]
    void RpcSpawnDamageBox(bool hit, float damageAmt, int characterType, Vector3 spawnPos, NetworkInstanceId gameManagerID)
    {
        GameObject gameManager = ClientScene.FindLocalObject(gameManagerID);
        gameManager.GetComponent<CombatPopup>().DisplayDamage(hit, damageAmt, characterType, spawnPos);
    }
}
