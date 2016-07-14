using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CharacterBase : NetworkBehaviour {
    /*attributes*/
    [SyncVar] public int strength;
    [SyncVar] public int dexterity;
    [SyncVar] public int intelligence;
    [SyncVar] public int vitality;

    /*natural stats*/
    [SyncVar] public int maxHealth;
    [SyncVar] public int currentHealth;
    [SyncVar] public int evadeChance;

    /*artificial stats*/
    [SyncVar] public int armorRating;

    [SyncVar] bool isDead = false;

    [ClientRpc]
    public void RpcCalcStats()
    {
        maxHealth = vitality + (strength / 2);
        evadeChance = dexterity + (intelligence / 2);
        armorRating = evadeChance;

        currentHealth = maxHealth;

        Debug.Log(currentHealth + " / " + maxHealth);
        Debug.Log(evadeChance);
        Debug.Log(armorRating);
    }

    [ClientRpc]
    public void RpcTakeDamage(int damage, string source)
    {
        if(isDead)
        {
            Debug.LogError(source + " is trying to deal damage to " + gameObject.transform.name + ", but it's already dead!");
            return;
        }

        Debug.Log("Took " + damage + " damage from \"" + source + "\"!");
        currentHealth -= damage;
        Debug.Log(currentHealth + " / " + maxHealth);
        if(currentHealth <= 0)
        {
            Debug.Log("DEAD");
            isDead = true;
            MeshRenderer[] mr = GetComponentsInChildren<MeshRenderer>();
            {
                foreach (MeshRenderer m in mr)
                {
                    m.enabled = false;
                }
            }
        }
    }
    [ClientRpc]
    public void RpcRespawn()
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
    }
}
