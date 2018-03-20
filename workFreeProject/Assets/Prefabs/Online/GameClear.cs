using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameClear : MonoBehaviour {

    GameObject networkManager;

    // Use this for initialization
    void Start () {
 //       networkManager = GameObject.Find("NetworkManager");
 //       networkManager.GetComponent<NetworkManagerScript>().NetDisconnect();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            if( SceneManager.GetActiveScene().name == "Offline" )
            {
               col.gameObject.GetComponent<playerController>().GoalFlugSwitch();
            }
            else if (SceneManager.GetActiveScene().name == "Online")
            {
                col.gameObject.GetComponent<networkPlayerController>().GoalFlugSwitch();
            }
        }
    }
}
