using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameClear : MonoBehaviour {

    GameObject networkManager;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            if (SceneManager.GetActiveScene().name == "Online")
            {
                networkManager = GameObject.Find("NetworkManager");
                networkManager.GetComponent<NetworkManagerScript>().NetDisconnect();

                //WIN表示あり
                if (col.name == "player")
                {
                    SceneManager.LoadScene("Result");
                }
                else//LOSE表示あり
                {
                    SceneManager.LoadScene("Result");
                }
            }
            else if (SceneManager.GetActiveScene().name == "Offline")
            {
                //WINもLOSEも表示しない。
                SceneManager.LoadScene("Result");
            }
        }
    }
}
