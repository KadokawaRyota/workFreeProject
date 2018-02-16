using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

//スケールのサイズ1を1mとして扱う。
//成人男性の歩幅が75cm、2歩で約1秒なのでspeedの変数は1.5m/sを基準とする。(歩行)走る速度は約4倍になるそうなので4をかける。
//speedを扱う際にfpsで割ること。


public class networkPlayerController : NetworkBehaviour
{

    GameObject Scr_ControllerManager;

    [SerializeField]
    GameObject camera;

    public enum PLAYER_STATE
    {
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

    public Text speedText; //Text用変数


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

    GameObject Timer;
    HostTimer hostTimerScript;

    // Use this for initialization
    public void Start()
    {
        if (!isLocalPlayer)
        {
            GetComponent<BoxCollider>().enabled = false;
            return;
        }

        //ネットワーク上では、プレイヤーが生成されてからステージを作ったりするため。色々設定～。

        //名前の変更
        this.name = ("player");

        //ネットワークマネージャーに自分を教える
        GameObject.Find("NetworkManager").GetComponent<NetworkManagerScript>().SetPlayer(this.gameObject);

        //ネットワーク上で管理するタイマーを取得
        Timer = GameObject.Find("HostTimer");
        hostTimerScript = Timer.GetComponent<HostTimer>();
        hostTimerScript.Start();


        velocity = Vector3.zero;
        time = 0;
        state = PLAYER_STATE.STOP;
        bJump = false;
        bDoubleJump = false;
        bHitWall = false;

        //スピードUI
        speedText = GameObject.Find("Canvas/Speed").GetComponent<Text>();
        speedText.text = "Speed: 0";

        //カメラの更新
        camera = GameObject.Find("Main Camera");
        camera.GetComponent<cameraController>().SetCamera();

        //ぷにコンの長さを取得するため、コントローラーマネージャ取得
        Scr_ControllerManager = GameObject.Find("PuniconCamera/ControllerManager");

        //ステージ生成。
        MapLorder mapLorderScript = GameObject.Find("StageEditor").GetComponent<MapLorder>();
        mapLorderScript.SetNetworkPlayer(this.gameObject);

        //スタート位置に戻す処理などを考えてここで再度配置しておく。
        transform.position = GetComponent<playerSetting>().GetStartPos();

        //スコアの取得
        Score = GameObject.Find("ScoreManager");

        //ボタンに自分自身のジャンプを割り当てる。
        GameObject jumpButton = GameObject.Find("Canvas/JumpButton");
        Button button = jumpButton.gameObject.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(  () => playerJump(true) );
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        switch (state)
        {
            case (PLAYER_STATE.STOP):
                {
                    //スタート時間になったので状態を走る状態にする。
                    if (hostTimerScript.GetTime() >= startWaitTime)
                    {
                        time = 0;
                        state = PLAYER_STATE.RUN;
                        GetComponent<Animator>().SetBool("bRun", true);
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
                    if (bHitWall)
                    {
                        velocity.x = 0.0f;
                        time += Time.deltaTime;
                        if (time > 1)
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
        if (!isLocalPlayer) return;

        switch (state)
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
            default:
                {
                    break;
                }
        }

        //カメラの更新
        camera.GetComponent<cameraController>().SetCamera();
    }

    //バグが出たので処理を分けました。
    public void playerJump(bool jump)
    {
        //操作できるプレイヤーじゃなければジャンプ命令を受け付けない。
        if ( !isLocalPlayer || state != PLAYER_STATE.RUN) return;

        //ジャンプ処理。
        if ( !bJump && jump )
        {
            GetComponent<networkPlayerController>().HitWall(false);
            GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, fJumpPower, 0.0f), ForceMode.Impulse);

            //ジャンプですり抜けられる壁をすり抜けるため。
            gameObject.layer = LayerMask.NameToLayer("JumpPlayer");

            //ジャンプフラグをtrueに。
            bJump = true;

            return;
        }
        //ジャンプ中にジャンプ命令が呼ばれた時は二段ジャンプ
        else if( bJump && jump && !bDoubleJump )
        {
            if( !bDoubleJump )
            {
                //空中に地面を作ってジャンプする感じにするため、一旦初期化
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, fJumpPower, 0.0f), ForceMode.Impulse);
                gameObject.layer = LayerMask.NameToLayer("JumpPlayer");

                //二段ジャンプフラグをtrueに
                bDoubleJump = true;
                return;
            }
            else
            {
                return;
            }
        }
        //着地処理用。
        else if( !jump )
        {
            bJump = false;
            bDoubleJump = false;
            return;
        }
        else
        {
            return;
        }
    }

    public void changeAccel()
    {
        //操作できるプレイヤーじゃなければ速度調整命令を受け付けない
        if (!isLocalPlayer) return;

        Vector3 length = Scr_ControllerManager.GetComponent<Scr_ControllerManager>().GetControllerVec();

        //コントローラーの長さ
        if (length.x > 0 && length.x <= 300)
        {
            //キャラクターのスピードが一定以下
            if (speed < speedMin)
            {
                //処理無し
            }
            else if (speed > speedMin)
            {
                speed -= 0.01f;
                if (speed < speedMin)
                {
                    speed = speedMin;
                }
            }
        }
        //コントローラの長さ
        else if (length.x > 300 && length.x <= 600)
        {
            if (speed < (speedMAX - speedMin))
            {
                speed += 0.01f;
                if (speed > (speedMAX - speedMin))
                {
                    speed = (speedMAX - speedMin);
                }
            }
            else if (speed > 2.0)
            {
                speed -= 0.04f;
                if (speed < speedMin)
                {
                    speed = speedMin;
                }
            }
        }
        else if (length.x > 600)
        {
            if (speed < speedMAX)
            {
                speed += 0.04f;
                if (speed > speedMAX)
                {
                    speed = speedMAX;
                }
            }
        }

        //小数点第3位以下切り捨て
        speed *= 100;
        Mathf.Round(speed);
        speed /= 100;

        //テキストの変更
        speedText.text = "Speed: " + speed.ToString();

    }

    public void HitWall(bool hitWall)
    {
        bHitWall = hitWall;
    }

    public void HitItem()
    {
        Score.GetComponent<Score>().ScoreAdd(100);
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Coin")
        {
            col.gameObject.GetComponent<SphereCollider>().enabled = false;
            Destroy(col.gameObject);
            HitItem();
        }
    }
}
