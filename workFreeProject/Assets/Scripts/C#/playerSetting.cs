using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSetting : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //右に向かせる
        transform.LookAt(new Vector3(90, 0, 0));
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
