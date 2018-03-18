using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//スケールのサイズ1を1mとして扱う。
//成人男性の歩幅が75cm、2歩で約1秒なのでspeedの変数は1.5m/sを基準とする。(歩行)走る速度は約4倍になるそうなので4をかける。
//speedを扱う際にfpsで割ること。


public class playerController : MonoBehaviour {

    GameObject Scr_ControllerManager;

    [SerializeField]
    GameObject camera;

    public enum PLAYER_STATE{
        STOP = 0,
        RUN,
        RECOVERY,
        GOAL,
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

    Vector3 oldPosition;

    bool bHitWall = false;

    GameObject Score;

    public Text speedText; //Text用変数

    GameObject oldBlock;    //前回のブロック位置
    GameObject oldBlock2;   //前々回のブロック位置

    float recoveryTime = 0;
    float RECOVERYTIMERIMIT = 2;

    float resultTimer = 0;

    [SerializeField]
    float marginSpaceTAP;


    //エフェクト
    [SerializeField]
    ParticleSystem smog;        //走る時の煙

    // Use this for initialization
    void Start () {
        velocity = Vector3.zero;
        time = 0;
        state = PLAYER_STATE.STOP;
        bJump = false;
        bDoubleJump = false;
        bHitWall = false;

        resultTimer = 0;

        camera = GameObject.Find("Main Camera");
        //カメラの更新
        camera.GetComponent<cameraController>().SetCamera();

        //ぷにコンの長さを取得するため、コントローラーマネージャ取得
        Scr_ControllerManager = GameObject.Find("PuniconCamera/ControllerManager");

        Score = GameObject.Find("ScoreManager");

        //テキストの変更
        speedText.text = "Speed: " + speed.ToString();


        //エフェクト
        smog.Stop();
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
                    smog.Play();    //走る時の煙エフェクトON
                }
                break;
            }
            case (PLAYER_STATE.RUN):
            {
                //スピードの変更
                changeAccel();
                //横の移動量決定
                velocity = new Vector3(speed / 60 * 4, velocity.y, 0);

                //壁に当たってる間は止まる。
                if( bHitWall )
                {
                    velocity.x = 0.0f;
                    time += Time.deltaTime;
                    if( time > 1 )
                    {
                        time = 0;
                        speed = speedMin;
                        bHitWall = false;
                    }
                }
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
                oldPosition = transform.position;
                break;
            }
            case (PLAYER_STATE.RECOVERY):
            {
                recoveryTime += Time.deltaTime;
                if(recoveryTime > RECOVERYTIMERIMIT)
                {
                    recoveryTime = 0;
                    state = PLAYER_STATE.RUN;
                }
                break;
            }
            case (PLAYER_STATE.GOAL):
            {
                GetComponent<Animator>().SetBool("bRun", false);
                resultTimer += Time.deltaTime;
                
                //スピードを落とすまで。
                if( 0.2 > resultTimer )
                {
                    transform.position += velocity;
                    oldPosition = transform.position;
                }
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
        if (bJump != jump && !bHitWall && state == PLAYER_STATE.RUN && !bHitWall )
        {
            bJump = jump;
            //着地処理の時だけ二段ジャンプをfalseに。
            if ( !jump )
            bDoubleJump = false;
            smog.Play();    //走る時の煙エフェクトOFF
        }
        //二段ジャンプ（ジャンプ中にジャンプ処理が呼ばれた。かつまだ二段ジャンプしていない）
        else if (bJump && jump && bDoubleJump == false && !bHitWall )
        {
            bDoubleJump = true;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, fJumpPower, 0.0f), ForceMode.Impulse);
            gameObject.layer = LayerMask.NameToLayer("JumpPlayer");
            return;
        }
        else
        {
            return;
        }

        //ジャンプ処理。
        if ( bJump )
        {
            GetComponent<playerController>().HitWall(false);
            if ( state == PLAYER_STATE.RUN )
            GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, fJumpPower, 0.0f), ForceMode.Impulse);
            gameObject.layer = LayerMask.NameToLayer("JumpPlayer");

            smog.Stop();    //走る時の煙エフェクトOFF
        }
    }

    public void changeAccel()
    {
        Vector3 length = Scr_ControllerManager.GetComponent<Scr_ControllerManager>().GetControllerVec();

        if ( length.x > marginSpaceTAP)
        {
            speed += 0.04f;
        }
        else if ( length.x < -marginSpaceTAP)
        {
            speed -= 0.04f;
        }

        ////速度の制御はコントローラの長さ
        //コントローラーの長さ(増)
        if (length.x > marginSpaceTAP && length.x < 300 )
        {
            //最大速度が2.0
            if (speed > 2.0 )
            {
                speed = 2.0f;
            }
        }
        else if (length.x >= 300 && length.x < 600)
        {
            //最大速度が3.0
            if (speed > speedMAX )
            {
                speed = speedMAX;
            }
        }
        //コントローラの長さ
        else if ( length.x >= 600 )
        {
            //急加速
            speed += 0.06f;
            //最大速度が3.0
            if (speed > speedMAX)
            {
                speed = speedMAX;
            }
        }
        //////コントローラーの長さ(減)
        if (length.x < -marginSpaceTAP && length.x > -600)
        {
            //最低速度が１1.0
            if (speed < 1.0)
            {
                speed = 1.0f;
            }
        }
        //コントローラの長さ
        else if (length.x <= -600)
        {
            //急減速
            speed -= 0.06f;
            //最低速度が1.0
            if (speed < 1.0)
            {
                speed = 1.0f;
            }
        }

        //小数点第3位以下切り捨て
        speed *= 100;
        Mathf.Round(speed);
        speed /= 100;

        //テキストの変更
        speedText.text = "Speed: " + speed.ToString();
    }

    public void HitWall( bool hitWall )
    {
        bHitWall = hitWall;
    }

    public void HitItem()
    {
        Score.GetComponent<Score>().ScoreAdd(100);
    }


    void OnTriggerEnter(Collider col)
    {
        if( col.gameObject.tag == "Coin" )
        {
            col.gameObject.GetComponent<SphereCollider>().enabled = false;
            Destroy(col.gameObject);
            HitItem();
        }
        if( col.gameObject.tag == "Block" )
        {
            if( col.gameObject.name == "startBlock(Clone)")
            {
                oldBlock = col.gameObject;
            }

            oldBlock2 = oldBlock;
            oldBlock = col.gameObject;
        }
        if( col.gameObject.name == "GameOverLine(Clone)")
        {
            state = PLAYER_STATE.RECOVERY;
            transform.position = new Vector3 ( oldBlock2.transform.position.x , oldBlock2.transform.position.y + 1.0f , oldBlock2.transform.position.z);
        }
    }

    public void GoalFlugSwitch()
    {
        state = PLAYER_STATE.GOAL;
        //リザルト遷移の準備
        GameObject.Find("resultManager").GetComponent<ResultManagerScript>().OfflinePlayerGoal(this.gameObject);
    }

    public int GetScore()
    {
        return Score.GetComponent<Score>().GetScore();
    }

}
