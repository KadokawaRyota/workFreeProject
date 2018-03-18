using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WallCheck : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay(Collider col)
    {
        if (transform.parent.name != "player") return;

        if( col.tag == "Block" )
        {
            if (SceneManager.GetActiveScene().name == "Online")
            {
                transform.parent.GetComponent<networkPlayerController>().HitWall(true);
            }
            else if (SceneManager.GetActiveScene().name == "Offline")
            {
                transform.parent.GetComponent<playerController>().HitWall(true);
            }

            transform.parent.GetComponent<Rigidbody>().AddForce(-300.0f, 0.0f, 0.0f, ForceMode.Impulse);
        }
        if (col.tag == "Toge")
        {
            if (SceneManager.GetActiveScene().name == "Online")
            {
                transform.parent.GetComponent<networkPlayerController>().HitToge();
            }
            else if (SceneManager.GetActiveScene().name == "Offline")
            {
                transform.parent.GetComponent<playerController>().HitToge();
            }
        }

    }
    void OnTriggerExit(Collider col)
    {
    }
}
