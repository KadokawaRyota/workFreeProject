using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//スケールのサイズ1を1mとして扱う。
//成人男性の歩幅が75cm、2歩で約1秒なのでspeedの変数は1.5m/sを基準とする。(歩行)走る速度は約4倍になるそうなので4をかける。
//speedを扱う際にfpsで割ること。


public class playerController : MonoBehaviour {

    [SerializeField]
    GameObject punipuniCon;

    [SerializeField]
    GameObject camera;

    public enum PLAYER_STATE{
        STOP = 0,
        RUN,
        MAX,
    };

    [SerializeField]
    float startWaitTime;    //スタートするまでの時間
    [SerializeField]
    float speed = 1.5f;        //(秒速)
    [SerializeField]
    float speedMin = 1.0f;
    float speedMAX = 3.0f;

    [SerializeField]
    float fJumpPower;

    Collider groundChecker;

    float time;             //時間カウント用
    bool bJump = false;

    bool bDoubleJump = false;

    Vector3 velocity;       //プレイヤーの移動量
    PLAYER_STATE state;     //プレイヤーの状態遷移

    // Use this for initialization
    void Start () {
        velocity = Vector3.zero;
        time = 0;
        state = PLAYER_STATE.STOP;
        bJump = false;
        bDoubleJump = false;

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
                //横の移動量決定
                velocity = new Vector3(speed / 60 * 4, velocity.y, 0);
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
                transform.position += velocity;
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

    public void playerJump( bool jump )
    {
        //ジャンプしてる時にジャンプ呼ばれたり、ジャンプしてない時にジャンプしてない状態に変えても意味ないので、制御用
        if (bJump != jump)
        {
            bJump = jump;
            //着地処理の時だけ二段ジャンプをfalseに。
            if( !jump )
            bDoubleJump = false;
        }
        //二段ジャンプ（ジャンプ中にジャンプ処理が呼ばれた。かつまだ二段ジャンプしていない）
        else if (bJump && jump && bDoubleJump == false)
        {
            bDoubleJump = true;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, fJumpPower, 0.0f), ForceMode.Impulse);
            return;
        }
        else
        {
            return;
        }

        //ジャンプ処理。
        if ( bJump )
        {
            if( state == PLAYER_STATE.RUN )
            GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, fJumpPower, 0.0f), ForceMode.Impulse);
        }
    }
}
