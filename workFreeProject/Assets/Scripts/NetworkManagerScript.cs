using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class NetworkManagerScript : NetworkManager
{

    NetworkManager manager;

    //ローカル切り替えフラグ	ture時はOnline,false時はOffline
    [SerializeField]
    bool isOnlinePlay = false;

    //サーバー切り替えフラグ
    public bool isStartAsServer = true;

    [SerializeField]
    string serverIPAdress = "192.168.13.3";

    public GameObject punioconCamera;       //ぷにコンカメラの取得

    GameObject player = null;

    void Awake()
    {
        serverIPAdress = GameObject.Find("NetworkSetter").GetComponent<NetworkSetter>().GetIpSetting();

        if (GameObject.Find("NetworkSetter").GetComponent<NetworkSetter>().GetState() == "host")
        {
            isStartAsServer = true;
        }
        else if (GameObject.Find("NetworkSetter").GetComponent<NetworkSetter>().GetState() == "client")
        {
            isStartAsServer = false;
        }
    }

    public void Start()
    {
        player = null;

        Debug.Log("Start");
        Debug.Log(SceneManager.GetActiveScene().name);
        if (SceneManager.GetActiveScene().name == "Online")
        {
            isOnlinePlay = true;
        }
        else if (SceneManager.GetActiveScene().name == "Offline")
        {
            isOnlinePlay = false;
        }

        //NetworkManagerの取得
        manager = GetComponent<NetworkManager>();
        //punioconCamera = GameObject.Find("PuniconCamera");

        if (isOnlinePlay)
        {
            OnlineSetup();  //オンライン時の設定
        }
        else
        {
            isStartAsServer = true; //オフライン時はホストになる
            OfflineSetup(); //オフライン時の設定
        }
    }

    public bool GetOnline()
    {
        return isOnlinePlay;
    }

    //オンラインセットアップ関数
    void OnlineSetup()
    {
        //PCアプリケーション起動時処理
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            if (isStartAsServer)
            {
                manager.networkAddress = "localhost";       //ホストの時はlocalhost
                manager.StartHost();                        //ホスト処理開始
                Debug.Log("Start as Server");
                punioconCamera.SetActive(true);

            }

            else
            {
                //仮想コントローラーの実装
                punioconCamera.SetActive(true);

                manager.networkAddress = serverIPAdress;    //クライアントの時は設定したIPアドレスを代入
                manager.StartClient();                      //クライアント処理開始
                Debug.Log("Start as Client");
            }

        }

        //アンドロイドアプリケーション起動時処理
        else if (Application.platform == RuntimePlatform.Android)
        {
            //仮想コントローラーの実装
            punioconCamera.SetActive(true);

            if (isStartAsServer)
            {
                manager.networkAddress = serverIPAdress;       //ホストの時はlocalhost
                manager.StartHost();                        //ホスト処理開始
                Debug.Log("Start as Server");
                punioconCamera.SetActive(true);

            }
            else
            {               
                manager.networkAddress = GameObject.Find("NetworkSetter").GetComponent<NetworkSetter>().GetIpSetting();
                manager.StartClient();
                Debug.Log("Start as Client");
                punioconCamera.SetActive(true);
            }
        }
    }

    //オフラインセットアップ関数
    void OfflineSetup()
    {
        //PCアプリケーション起動時処理
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            //仮想コントローラーの実装
            punioconCamera.SetActive(true);

            manager.networkAddress = "localhost";       //ホストの時はlocalhost
                                                        //manager.networkAddress = serverIPAdress;
            manager.StartHost();                        //ホスト処理開始
            Debug.Log("Start as Host");
        }

        //アンドロイドアプリケーション起動時処理
        else if (Application.platform == RuntimePlatform.Android)
        {
            //仮想コントローラーの実装
            punioconCamera.SetActive(true);

            //オフライン時はホストになる
            serverIPAdress = "localhost";
            //manager.networkAddress = serverIPAdress;
            manager.StartHost();
            Debug.Log("Start as Host");
        }
    }

    public void SetPlayer( GameObject sPlayer )
    {
        player = sPlayer;
    }

    //ネットワーク終了処理
    public void NetDisconnect()
    {
        //manager.StopClient ();
        Shutdown();
        //Destroy (this.gameObject);
    }

    //////オーバーライド

    //クライアントが入ってきたら、サーバー側で呼び出される。
    public override void OnServerConnect(NetworkConnection conn)
    {
        if (player == null) return;

        //元のを呼ばないとオーバーライド出来ない。
        base.OnServerConnect(conn);

        player.GetComponent<networkPlayerController>().Start();
    }

    //サーバーが落ちた時
    //サーバーが切断されたときにクライアント上で呼び出されます。
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        //元のを呼ばないとオーバーライド出来ない。
        base.OnClientDisconnect(conn);

        SceneManager.LoadScene("Result");
    }
}