using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class CamMovement : NetworkBehaviour {

    [SerializeField]
    GameObject player;

    Vector3 startPos;
    Vector3 pos;
    Vector3 PlayerPos;

	// Use this for initialization
	void Start () {

        if(!isClient)
            Destroy(gameObject);

        StartCoroutine(LookForPlayer());
	}
	
	// Update is called once per frame
	void Update ()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (player == null)
            return;
        PlayerPos = player.transform.position;
        pos = transform.position;

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //transform.position = Vector3.MoveTowards(pos, PlayerPos, 0.75f);
            transform.position = Vector3.Lerp(pos, PlayerPos, 4.5f * Time.deltaTime);
        }else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            transform.position = Vector3.Lerp(pos, startPos, 8.5f * Time.deltaTime);
        }

        /*if(Input.GetMouseButtonDown(2))
        {
            Debug.Log("Pan");
        }*/
    }

    void LateUpdate()
    {
        if (player == null)
            return;
        transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z - 9f);
        transform.LookAt(player.transform.position);
    }

    private IEnumerator LookForPlayer()
    {
        yield return new WaitForSeconds(0.1f);

        startPos = transform.position;
        player = GameObject.FindGameObjectWithTag("LocalPlayer");
        if (GameObject.FindGameObjectWithTag("LocalPlayer") != null)
        {
        }
        else
        {
            Debug.LogError("Found nothing! (Looked for LocalPlayer)");
        }

    }
}
