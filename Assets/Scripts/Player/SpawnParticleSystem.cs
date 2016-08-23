using UnityEngine;
using System.Collections;

public class SpawnParticleSystem : MonoBehaviour {

    [SerializeField] GameObject[] particleSystems;

    public void SpawnParticleSystemFromIndex(int particleIndex)
    {
        Debug.Log("Spawning Particle System Script Called");
        GameObject ps = Instantiate(particleSystems[particleIndex], new Vector3(transform.position.x, transform.position.y + GetComponent<CapsuleCollider>().height / 2, transform.position.z), new Quaternion(-90, 0, 0, 1)) as GameObject;
        ps.transform.SetParent(this.transform, true);
    }
}
