using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CharacterBase : NetworkBehaviour {
    /*attributes*/
    [SyncVar(hook = "UpdateStr")] public int strength;
    [SyncVar(hook = "UpdateDex")] public int dexterity;
    [SyncVar(hook = "UpdateInt")] public int intelligence;
    [SyncVar(hook = "UpdateVit")] public int vitality;

    int statMin = 5;
    int statMax = 18;

    /*natural stats*/
    [SyncVar] public float maxHealth;
    [SyncVar] public float currentHealth;
    [SyncVar] public int evadeChance;

    /*artificial stats*/
    [SyncVar] public string playerName;

    [SyncVar] public int armorRating;

    [SyncVar] public float weaponDamageMin;
    [SyncVar] public float weaponDamageMax;
    [SyncVar] public float weaponCritModifier;


    [SyncVar] public float totalDamageMin;
    [SyncVar] public float totalDamageMax;

    [SyncVar] public bool isDead = false;

    public MeshRenderer[] meshRenderers;

    void Start()
    {
        if (!isLocalPlayer)
            return;

        playerName = ("Player " + Random.Range(0, 10000).ToString());
        transform.name = playerName;
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        //Debug.LogWarning("Trying to randomize our stats. Our player's NetworkID is " + GetComponent<NetworkIdentity>().netId);

        if(strength == 0)
            CmdRandomizeStats(GetComponent<NetworkIdentity>().netId);

        //RandomizeStats();
        //GenerateStats();
        //CmdCalcStats(GetComponent<NetworkIdentity>().netId);
    }

    void UpdateStr(int str)
    {
        //Debug.Log("Updating Stats");
        strength = str;
        GenerateStats();
    }

    void UpdateDex(int dex)
    {
        //Debug.Log("Updating Stats");
        dexterity = dex;
        GenerateStats();
    }

    void UpdateInt(int ints)
    {
        //Debug.Log("Updating Stats");
        intelligence = ints;
        GenerateStats();
    }

    void UpdateVit(int vit)
    {
        //Debug.Log("Updating Stats");
        vitality = vit;
        GenerateStats();
    }

    [Command]
    public void CmdRandomizeStats(NetworkInstanceId playerID)
    {
        RpcRandomizeStats(playerID);
    }

    [ClientRpc]
    public void RpcRandomizeStats(NetworkInstanceId playerID)
    {

        GameObject player = ClientScene.FindLocalObject(playerID);
        CharacterBase cb = player.GetComponent<CharacterBase>();
        Debug.LogWarning("Cound not find a player with the NetworkInstanceID of " + playerID);

        cb.strength = Random.Range(statMin, statMax);
        cb.dexterity = Random.Range(statMin, statMax);
        cb.intelligence = Random.Range(statMin, statMax);
        cb.vitality = Random.Range(statMin, statMax);
    }

    public void RandomizeStats()
    {
        if (!isLocalPlayer)
        {
            Debug.Log("!islocalplayer");
                return;
        }
            

        strength = Random.Range(statMin, statMax);
        dexterity = Random.Range(statMin, statMax);
        intelligence = Random.Range(statMin, statMax);
        vitality = Random.Range(statMin, statMax);
    }

    [Command]
    public void CmdGenerateStats(NetworkInstanceId playerID)
    {

        RpcGenerateStats(playerID);
    }

    [ClientRpc]
    public void RpcGenerateStats(NetworkInstanceId playerID)
    {
        GameObject player = ClientScene.FindLocalObject(playerID);
        CharacterBase cb = player.GetComponent<CharacterBase>();

        cb.maxHealth = cb.vitality + (cb.strength / 2);
        cb.evadeChance = cb.dexterity + (cb.intelligence / 2);
        cb.armorRating = cb.evadeChance;

        cb.currentHealth = cb.maxHealth;

        cb.totalDamageMin = cb.weaponDamageMin + cb.strength;
        cb.totalDamageMax = cb.weaponDamageMax + cb.strength;

        //Debug.Log(cb.currentHealth + " / " + cb.maxHealth);
        //Debug.Log(cb.evadeChance);
        //Debug.Log(cb.armorRating);
    }

    private void GenerateStatsNoNetworking()
    {
        maxHealth = 5 + vitality + (strength / 2);
        evadeChance = dexterity + (intelligence / 2);
        armorRating = evadeChance;

        currentHealth = maxHealth;

        totalDamageMin = weaponDamageMin + strength;
        totalDamageMax = weaponDamageMax + strength;
    }

    public void GenerateStats()
    {
        Debug.Log("GenerateStats()");
        //CmdGenerateStats(GetComponent<NetworkIdentity>().netId);
        GenerateStatsNoNetworking();
    }


    [Command]
    public void CmdReportDamage(NetworkInstanceId playerID, float damage, string source)
    {
        Debug.Log("[COMMAND] NetID " + playerID + " has been dealt " + damage + " damage by " + source + ".");
        GameObject targetPlayer = NetworkServer.FindLocalObject(playerID);
        targetPlayer.GetComponent<CharacterBase>().TakeDamage(playerID, damage, source);
    }

    [Command]
    public void CmdReportHeal(NetworkInstanceId playerID, float amount, string source)
    {
        GameObject targetPlayer = NetworkServer.FindLocalObject(playerID);
        targetPlayer.GetComponent<CharacterBase>().HealPlayer(playerID, amount, source);
    }

    [Command]
    public void CmdRequestRespawn(NetworkInstanceId playerID)
    {
        GameObject targetPlayer = NetworkServer.FindLocalObject(playerID);
        targetPlayer.GetComponent<CharacterBase>().RpcRespawnPlayer(playerID);
    }

    [Command]
    public void CmdAddStats(NetworkInstanceId playerID, int str, int dex, int ints, int vit)
    {
        GameObject targetPlayer = NetworkServer.FindLocalObject(playerID);
        targetPlayer.GetComponent<CharacterBase>().AddStats(str, dex, ints, vit);
    }

    public void AddStats(int str, int dex, int ints, int vit)
    {
        strength += str;
        dexterity += dex;
        intelligence += ints;
        vitality += vit;

        Debug.Log("Updated Stats.");
    }

    public void TakeDamage(NetworkInstanceId playerID, float damage, string source)
    {
        if(isDead)
        {
            Debug.LogError(source + " is trying to deal damage to " + gameObject.transform.name + ", but it's already dead!");
            return;
        }

        GameObject targetPlayer = NetworkServer.FindLocalObject(playerID);

        Debug.Log("Took " + damage + " damage from \"" + source + "\"!");
        currentHealth -= damage;
        Debug.Log(currentHealth + " / " + maxHealth);
        if(currentHealth <= 0)
        {
            RpcDie(GetComponent<NetworkIdentity>().netId);
        }
    }

    private void HealPlayer(NetworkInstanceId playerID, float amount, string source)
    {
        if (isDead)
            return;

        GameObject targetPlayer = NetworkServer.FindLocalObject(playerID);

        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        Debug.Log(currentHealth + " / " + maxHealth);
    }

    [ClientRpc]
    private void RpcRespawnPlayer(NetworkInstanceId playerID)
    {
        if (!isDead)
            return;

        Debug.Log("RespawnPlayer has been called.");

        GameObject targetPlayer = ClientScene.FindLocalObject(playerID);
        //MeshRenderer[] mr = targetPlayer.GetComponentsInChildren<MeshRenderer>();

        foreach(Renderer meshRenderer in targetPlayer.GetComponentsInChildren<MeshRenderer>())
        {
            Debug.Log("Enabling MeshRenderers.");
            meshRenderer.enabled = true;
        }

        targetPlayer.gameObject.layer = LayerMask.NameToLayer("Player");

        if(isLocalPlayer)
        {
            GetComponent<Movement>().enabled = true;
            GetComponent<Rotation>().enabled = true;
            Debug.Log("Re-enabled scripts.");
        }

        currentHealth = maxHealth;
        isDead = false;
    }

    [ClientRpc]
    public void RpcDie(NetworkInstanceId playerID)
    {

        GameObject targetPlayer = ClientScene.FindLocalObject(playerID);

        currentHealth = 0;
        Debug.Log("DEAD");
        isDead = true;

        /*foreach (MeshRenderer m in meshRenderers)
        {
            m.enabled = false;
        }*/

        foreach(MeshRenderer mr in targetPlayer.GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = false;
        }

        //targetPlayer.GetComponent<CapsuleCollider>().enabled = false;
        targetPlayer.gameObject.layer = LayerMask.NameToLayer("Default");

        if (isLocalPlayer)
        {
            targetPlayer.GetComponent<Movement>().enabled = false;
            targetPlayer.GetComponent<Rotation>().enabled = false;
        } 
    }

    [ClientRpc]
    public void RpcRespawn(NetworkInstanceId playerID)
    {
        GameObject player = ClientScene.FindLocalObject(playerID);

        if (!isDead)
        {
            Debug.LogError("Character " + gameObject.transform.name + " is trying to respawn but isn't dead!");
            return;
        }

        Debug.Log("Respawned!");
        isDead = false;

        foreach (MeshRenderer mr in meshRenderers)
        {
            mr.enabled = true;
        }

        player.GetComponent<CapsuleCollider>().enabled = false;

        player.GetComponent<Movement>().enabled = true;
        player.GetComponent<Rotation>().enabled = true;

        currentHealth = maxHealth;
    }

    [Command]
    public void CmdRespawn()
    {
        if (!isDead)
        {
            Debug.LogError("Character " + gameObject.transform.name + " is trying to respawn but isn't dead!");
            return;
        }

        Debug.Log("Respawned!");
        isDead = false;
        MeshRenderer[] mr = GetComponentsInChildren<MeshRenderer>();
        {
            foreach (MeshRenderer m in mr)
            {
                m.enabled = true;
            }
        }

        GetComponent<Movement>().enabled = true;
        GetComponent<Rotation>().enabled = true;

        currentHealth = maxHealth;
    }

    [Command]
    public void CmdHeal(float amount, string source)
    {
        if(isDead)
        {
            Debug.LogError(source + " is trying to heal " + transform.name + " for " + amount + ", but he is dead!");
            return;
        }

        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        Debug.Log(gameObject.transform.name + " has been healed for " + amount + " health by " + source + "!");
        Debug.Log(currentHealth + " / " + maxHealth);
    }
}
