using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class CombatPopup : NetworkBehaviour {

    public GameObject damageTextGO;
    private Transform damageCanvas;

	// Use this for initialization
	void Start () {
        damageCanvas = GameObject.FindGameObjectWithTag("CombatCanvas").transform;      
	}
	
	public void DisplayDamage(bool hit, float amount, int dealerType, Vector3 spawnPos)
    {

        Vector3 newPos = Camera.main.WorldToScreenPoint(spawnPos);
        newPos.z = 0.0f;

        Vector3 newPosFinal = new Vector3(Random.Range(newPos.x - 15, newPos.x + 15), newPos.y, newPos.z);

        GameObject instance = Instantiate(damageTextGO, newPos, damageTextGO.transform.rotation) as GameObject;

        if (hit)
        {
            instance.GetComponent<Text>().fontSize = 50;
            instance.GetComponent<Text>().text = ((int)amount).ToString();
        }
        else
        {
            //instance.GetComponent<RectTransform>().sizeDelta = new Vector2(65, 30);
            instance.GetComponent<Text>().fontSize = 25;
            instance.GetComponent<Text>().text = "Missed!";
        }

        if (dealerType == 1) //Player
        {
           // if (isLocalPlayer) Actually check if the target is localplayer tagged.
                instance.GetComponent<Text>().color = new Color32(103, 155, 153, 255);
            //else
                //instance.GetComponent<Text>().color = new Color32(13, 78, 73, 255);

        }
        else if (dealerType == 2) //Enemy
        {
            instance.GetComponent<Text>().color = new Color32(211, 105, 108, 255);
        }
        else if (dealerType == 3) //Neutral NPC
        {

        }

        instance.transform.SetParent(damageCanvas);
        //instance.transform.localScale = new Vector3(1, 1, 1);

        if(NetworkServer.active)
            NetworkServer.Spawn(instance);
    }
}
