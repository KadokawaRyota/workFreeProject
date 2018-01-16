using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSetting : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //プレイヤーの開始地点を決める
    public void SetPlayer( Vector3 pos )
    {
        transform.position = pos;
    }
}
