using UnityEngine;
using System.Collections;

public class RandomTentacles : MonoBehaviour {

    [SerializeField]
    GameObject[] Tentacles;

    GameObject currentTentacle;

    Animator anim;

    // Use this for initialization
    void Start () {
        Tentacles = GameObject.FindGameObjectsWithTag("Tentacle");

        foreach(GameObject tent in Tentacles)
        {
            tent.GetComponent<Animator>().SetTime(Random.Range(0.0f, 4));
        }
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.T))
        {
            currentTentacle = Tentacles[Random.Range(0, Tentacles.Length - 1)];
            anim = currentTentacle.GetComponent<Animator>();
            anim.SetBool("Slam", true);
            StartCoroutine(StopSlam());
        }
	}

    IEnumerator StopSlam()
    {
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("Slam", false);
    }
}
