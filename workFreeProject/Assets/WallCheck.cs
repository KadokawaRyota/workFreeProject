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
        }
    }
    void OnTriggerExit(Collider col)
    {
    }
}
