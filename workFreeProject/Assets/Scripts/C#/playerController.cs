using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//スケールのサイズ1を1mとして扱う。
//成人男性の歩幅が75cm、2歩で約1秒なのでspeedの変数は1.5m/sを基準とする。(歩行)走る速度は約4倍になるそうなので4をかける。
//speedを扱う際にfpsで割ること。


public class playerController : MonoBehaviour {

    [SerializeField]
    GameObject camera;

    public enum PLAYER_STATE{
        STOP = 0,
        RUN,
        MAX,
    };

    [SerializeField]
    float startWaitTime;    //スタートするまでの時間
    float time;             //時間カウント用

    float speed = 1.5f;        //(秒速)

    PLAYER_STATE state;     //プレイヤーの状態遷移

    // Use this for initialization
    void Start () {
        time = 0;
        state = PLAYER_STATE.STOP;

        //カメラの更新
        camera.GetComponent<cameraController>().SetCamera();
    }
	
	// Update is called once per frame
	void Update () {
		switch(state)
        {
            case (PLAYER_STATE.STOP):
            {
                //スタートするまでの待機
                time += Time.deltaTime;
                //スタート時間になったので状態を走る状態にする。
                if( time >= startWaitTime )
                {
                    time = 0;
                    state = PLAYER_STATE.RUN;
                        GetComponent<Animator>().SetBool("bRun",true);
                }
                break;
            }
            case (PLAYER_STATE.RUN):
            {
                break;
            }
            default:
            {
                break;
            }
        }
	}
    void FixedUpdate()
    {
        		switch(state)
        {
            case (PLAYER_STATE.STOP):
            {
                break;
            }
            case (PLAYER_STATE.RUN):
            {
                transform.position += new Vector3(speed / 60 * 4 , 0, 0);
                break;
            }
            default:
            {
                break;
            }
        }

        //カメラの更新
        camera.GetComponent<cameraController>().SetCamera();
    }
}
