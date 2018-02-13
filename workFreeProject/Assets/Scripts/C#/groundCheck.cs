using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundCheck : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter( Collider col )
    {
        if( col.gameObject.name == "normalBlock(Clone)" || col.gameObject.tag == "AirBlock")

        //下降してる時のみ
        if(transform.parent.gameObject.GetComponent<Rigidbody>().velocity.y < 0 || col.gameObject.name == "normalBlock(Clone)" )
        {
            transform.parent.GetComponent<playerController>().playerJump(false);
            transform.parent.gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }
}
