using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour {

    [SerializeField]
    Vector3 Distance;


    GameObject posRobject;    //注視点となるオブジェクト

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
    }

    public void SetCamera()
    {
        posRobject = GameObject.Find("player/cameraR");
        //注視点を入れる
        transform.LookAt(posRobject.transform.position);
        //カメラのポジションを入れる。
        transform.position = posRobject.transform.position + Distance;
    }
}
