using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay(Collider col)
    {
        if( col.tag == "Block" )
        {
            transform.parent.GetComponent<playerController>().HitWall(true);
            transform.parent.GetComponent<Rigidbody>().AddForce(-300.0f, 0.0f, 0.0f, ForceMode.Impulse);
        }
    }
    void OnTriggerExit(Collider col)
    {
    }
}
