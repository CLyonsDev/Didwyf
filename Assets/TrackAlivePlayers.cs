using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TrackAlivePlayers : NetworkBehaviour {

    public List<GameObject> playerList = new List<GameObject>();

    public GameObject gameTextCanvasGO;
    public Text deadTimerText;

    public bool allPlayersDead = false;

    public float restartTimer = 0f;
    public float restartDelay;

    [SyncVar] public int alivePlayers = 0;
    [SyncVar] public bool introLevel = false;

    // Use this for initialization
    void Start () {
        gameTextCanvasGO = GameObject.Find("GameTextCanvas");
        deadTimerText = gameTextCanvasGO.transform.GetChild(1).gameObject.GetComponent<Text>();

        for (int i = 0; i < gameTextCanvasGO.transform.childCount; i++)
        {
            gameTextCanvasGO.transform.GetChild(i).gameObject.SetActive(false);
        }

        InvokeRepeating("CheckPlayersAtInterval", 0.1f, 0.3f);
	}
	
	// Update is called once per frame
	void Update () {

        CheckSceneName();

        if (!IsInvoking("CheckPlayersAtInterval"))
        {
            Debug.LogError("Not invoking!");
            InvokeRepeating("CheckPlayersAtInterval", 0.1f, 0.3f);
        }

        if (gameTextCanvasGO == null || deadTimerText == null)
        {
            gameTextCanvasGO = GameObject.Find("GameTextCanvas");
            deadTimerText = gameTextCanvasGO.transform.GetChild(1).gameObject.GetComponent<Text>();

            if (gameTextCanvasGO == null || deadTimerText == null)
            {
                for (int i = 0; i < gameTextCanvasGO.transform.childCount; i++)
                {
                    gameTextCanvasGO.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

	    if(allPlayersDead && !introLevel)
        {
            restartTimer += Time.deltaTime;
            deadTimerText.text = ("Restarting Floor In " + Mathf.Round(restartDelay - restartTimer) + " Seconds...");
            if(restartTimer >= restartDelay)
            {
                foreach(GameObject go in playerList)
                {
                    allPlayersDead = false;
                    go.GetComponent<CharacterBase>().ClearInventory();
                    go.GetComponent<Player>().CmdReloadLevel(); 
                }
            }
        }else if(allPlayersDead && introLevel)
        {
            gameTextCanvasGO.transform.GetChild(0).gameObject.GetComponent<Text>().text = ("You have been apprehended!");
            deadTimerText.text = ("As punishment for your crimes, you will be sent to clear out some nearby dungeons!");

            restartTimer += Time.deltaTime;
            if (restartTimer >= restartDelay)
            {
                foreach (GameObject go in playerList)
                {
                    allPlayersDead = false;
                    go.GetComponent<Player>().CmdNextLevel();
                }
            }
        }
	}

    /*IEnumerator CheckPlayersAtInterval(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            CheckPlayersAlive();
        } 
    }*/

    void CheckPlayersAtInterval()
    {
        CheckPlayersAlive();
    }

    public void CheckPlayersAlive()
    {
        playerList.Clear();
        alivePlayers = 0;

        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        GameObject localPlayer = GameObject.FindGameObjectWithTag("LocalPlayer");


        playerList.Add(localPlayer);

        for (int i = 0; i < allPlayers.Length; i++)
        {
            playerList.Add(allPlayers[i]);
            if (!allPlayers[i].GetComponent<CharacterBase>().isDead)
                alivePlayers++;
        }

        if (!localPlayer.GetComponent<CharacterBase>().isDead)
        {
            alivePlayers++;
        }



        if(alivePlayers == 0 && !allPlayersDead)
        {
            Debug.LogWarning("All players have died!");
            restartTimer = 0.0f;
            allPlayersDead = true;
            for (int i = 0; i < gameTextCanvasGO.transform.childCount; i++)
            {
                gameTextCanvasGO.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else if(alivePlayers > 0)
        {
            restartTimer = 0.0f;
            allPlayersDead = false;
            if(gameTextCanvasGO != null)
            {
                for (int i = 0; i < gameTextCanvasGO.transform.childCount; i++)
                {
                    gameTextCanvasGO.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    void CheckSceneName()
    {
        if (SceneManager.GetActiveScene().name == "Tavern" && !introLevel)
        {
            Debug.Log("Intro scene!");
            restartDelay = 6f;
            introLevel = true;
            GetComponent<IntroSceneLogic>().enabled = true;
        }
        else if (introLevel && !SceneManager.GetActiveScene().name.Equals("Tavern"))
        {
            introLevel = false;
            restartDelay = 4f;
            GetComponent<IntroSceneLogic>().enabled = false;
        }
    }
}
