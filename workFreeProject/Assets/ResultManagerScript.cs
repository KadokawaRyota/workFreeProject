using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//接続プレイヤーのゴール判定とリザルト画面への遷移
//順位やポイントの勝ち負けの判定を行う。

public class ResultManagerScript : MonoBehaviour {

    GameObject myPlayer;    //自分
    bool myPlayerhost;      //自分がホストかどうか？


    int myPlayerRunk;
    int myPlayerScore;

    GameObject vsPlayer;    //相手
    bool bVsPlayerGoal;

    bool bTimer = false;
    float timer;

    static public ResultManagerScript instance;

    // Use this for initialization
    void Start () {

        ////初期化処理。
        myPlayer = null;
        vsPlayer = null;

        bTimer = false;
        timer = 0;

        //ResultManagerインスタンスが存在したら
        if (instance != null)
        {
            //今回インスタンス化したNetworkSetterを破棄
            Destroy(this.gameObject);
            return;
        }
        //ResultManagerインスタンスがなかったら
        else if (instance == null)
        {
            //このResultManagerをインスタンスとする
            instance = this;
        }
        //シーンを跨いでもNetworkSetterインスタンスを破棄しない
        DontDestroyOnLoad(gameObject);

    }

    // Update is called once per frame
    void Update () {
        if (myPlayer != null && !bTimer)
        {
            if( vsPlayer != null )
            {
                //対戦相手がゴールしているか調べる。
                bVsPlayerGoal = vsPlayer.GetComponent<networkPlayerController>().GetSyncGoal();

                if( bVsPlayerGoal )
                {
                    bTimer = true;
                }
            }
            //対戦相手がいない
            else
            {
                bTimer = true;
            }
        }

        //リザルト遷移までの時間を測って遷移させる。
        if( bTimer )
        {
            timer += Time.deltaTime;


            if( timer > 3 && myPlayerhost )
            {
                bTimer = false;
                timer = 0;
                //ネットワーク終了処理。
                GameObject.Find("NetworkManager").GetComponent<NetworkManagerScript>().NetDisconnect();
                SceneManager.LoadScene("Result");
            }
        }

	}


    //プレイヤーがゴールしたら呼び出される。
    public void PlayerGoal( GameObject thisPlayer , bool isServer )
    {
        //呼び出し元の自分のプレイヤーを保持。
        myPlayer = thisPlayer;
        myPlayerhost = isServer;

        //呼び出し元の自分のプレイヤーの順位を格納。
        myPlayerRunk = myPlayer.GetComponent<networkPlayerController>().GetRunk();
        //スコアを格納。
        myPlayerScore = myPlayer.GetComponent<networkPlayerController>().GetScore();



        //対戦相手を検索して格納する。
        vsPlayer = GameObject.Find("vsPlayer");
    }

    //プレイヤーがゴールしたら呼び出される。
    public void ServerDisconnectPlayerGoal( GameObject thisPlayer )
    {
        //呼び出し元の自分のプレイヤーを保持。
        myPlayer = thisPlayer;

        //呼び出し元の自分のプレイヤーの順位を1位とする。
        myPlayerRunk = 1;

        //スコアを格納。
        myPlayerScore = myPlayer.GetComponent<networkPlayerController>().GetScore();
    }


    public GameObject GetMyPlayerGoalInfo()
    {
        return myPlayer;
    }

    public int GetRunk()
    {
        return myPlayerRunk;
    }

    public int GetScore()
    {
        return myPlayerScore;
    }
}
