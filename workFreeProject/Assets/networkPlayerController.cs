using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    public Text speedText; //Text用変数

    public Text rankText; //Text用変数


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

    GameObject oldBlock;    //前回のブロック位置
    GameObject oldBlock2;   //前々回のブロック位置

    float recoveryTime = 0;
    float RECOVERYTIMERIMIT = 2;

    GameObject Player2;

    [SerializeField]
    float Distance = 0.0f;

    int rank = 1;

    float resultTimer = 0;

    [SyncVar]
    bool syncGoal = false;

    [SerializeField]
    float marginSpaceTAP;

    [SerializeField]
    ParticleSystem smog;

    // Use this for initialization
    public void Start()
    {
        if (!isLocalPlayer)
        {
            //自分以外のプレイヤーの当たり判定をoff
            GetComponent<BoxCollider>().enabled = false;
            //Skyテクスチャオフ
            foreach (Transform child in this.transform)
            {
                if( child.name == "Sky" )
                {
                    child.gameObject.SetActive(false);
                }

            }

            //名前の変更
            this.name = ("vsPlayer");
            return;
        }

        resultTimer = 0;
        Distance = 0.0f;
        Player2 = null;
        rank = 1;

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

        //ランクUI
        rankText = GameObject.Find("Canvas/Rank").GetComponent<Text>();

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

        //ボタンに直接割り当てる場合。（イベントトリガーに設定したらいらなくなった。）
        /*Button button = jumpButton.gameObject.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(  () => playerJump(true) );*/


        //ボタンのイベントトリガーに割当
        EventTrigger currentTrigger = jumpButton.AddComponent<EventTrigger>();
        currentTrigger.triggers = new List<EventTrigger.Entry>();
        //↑ここでAddComponentしているので一応、初期化しています。

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown; //PointerClickの部分は追加したいEventによって変更してね
        entry.callback.AddListener((x) => playerJump(true));  //ラムダ式の右側は追加するメソッドです。

        currentTrigger.triggers.Add(entry);

        //エファクト
        smog.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        ////順位判定
        if( Player2 == null )
        {
            Player2 = GameObject.Find("networkPlayer(Clone)");
        }
        else
        {
            Distance = transform.position.x - Player2.transform.position.x;
        }



        rankText.text = rank.ToString() + "位";


        switch (state)
        {
            case (PLAYER_STATE.STOP):
                {
                    //スタート時間になったので状態を走る状態にする。
                    if (hostTimerScript.GetTime() >= startWaitTime)
                    {
                        smog.Play();
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

                    //順位判定
                    if (Distance >= 0)
                    {
                        rank = 1;
                    }
                    else
                    {
                        rank = 2;
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
            case (PLAYER_STATE.RECOVERY):
                {
                    recoveryTime += Time.deltaTime;
                    if (recoveryTime > RECOVERYTIMERIMIT)
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

                    //順位によってゴールした後に立つ位置を変える。
                    if (rank == 1)
                    {
                        speed = 3.0f;
                    }
                    else
                    {
                        speed = 1.0f;
                    }

                    //ゴールした後も少しだけ右へ進む。
                    //スピードを落とすまで。
                    if (0.2 > resultTimer)
                    {
                        //横の移動量決定
                        velocity = new Vector3(speed / 60 * 4, velocity.y, 0);
                        transform.position += velocity;
                        oldPosition = transform.position;
                        smog.Stop();
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
            smog.Stop();

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
            smog.Stop();
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

        if (length.x > marginSpaceTAP)
        {
            speed += 0.04f;
        }
        else if (length.x < -marginSpaceTAP)
        {
            speed -= 0.04f;
        }

        ////速度の制御はコントローラの長さ
        //コントローラーの長さ(増)
        if (length.x > marginSpaceTAP && length.x < 300)
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
            if (speed > speedMAX)
            {
                speed = speedMAX;
            }
        }
        //コントローラの長さ
        else if (length.x >= 600)
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

    public void HitWall(bool hitWall)
    {
        bHitWall = hitWall;
    }

    public void HitToge()
    {
        speed = 1.0f;
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
        if (col.gameObject.tag == "Block")
        {
            if (col.gameObject.name == "startBlock(Clone)")
            {
                oldBlock = col.gameObject;
            }

            oldBlock2 = oldBlock;
            oldBlock = col.gameObject;
        }
        if (col.gameObject.name == "GameOverLine(Clone)")
        {
            state = PLAYER_STATE.RECOVERY;
            transform.position = new Vector3(oldBlock2.transform.position.x, oldBlock2.transform.position.y + 1.0f, oldBlock2.transform.position.z);
            speed = 1.0f;
        }
    }

    
    public void GoalFlugSwitch()
    {
        //自分の状態をゴールに。
        state = PLAYER_STATE.GOAL;

        //リザルト遷移の準備
        GameObject.Find("resultManager").GetComponent<ResultManagerScript>().PlayerGoal( this.gameObject , isServer);

        ////ネットワークでゴールした事を送信(ホストとクライアントで分けているのは仕様上仕方ない。)
        //クライアントだけでもいいが、途中でホストがネットワークを切った時の事を考えて互いに送信し合うことにした。
        //クライアント
        if ( !isServer )
        {
            CmdProvideGoalToServer();
        }
        //ホスト
        else
        {
            RpcProvidePositionToServer();
        }
    }

    //クライアントからホストへ自分がゴールした事を伝える。
    [Command]
    void CmdProvideGoalToServer()
    {
        syncGoal = true;
    }

    //ホストからクライアントへ自分がゴールした事を伝える。
    [ClientRpc]
    void RpcProvidePositionToServer()
    {
        syncGoal = true;
    }

    public bool GetSyncGoal()
    {
        return syncGoal;
    }

    public int GetRunk()
    {
        return rank;
    }

    public int GetScore()
    {
        return Score.GetComponent<Score>().GetScore();
    }

    public void hostDisconnect()
    {
        GameObject.Find("resultManager").GetComponent<ResultManagerScript>().ServerDisconnectPlayerGoal(this.gameObject);
    }
}
