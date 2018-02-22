using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSetter : MonoBehaviour
{
    //タイトルで呼び出す。
    [SerializeField]
    static string NetworkState;    //ホストかクライアントか
    [SerializeField]
    static string NetworkIp;              //IPアドレス
    [SerializeField]
    static string NetworkPort;            //ポート番号

    static public NetworkSetter instance;

    void Start()
    {
        //NetworkSetterインスタンスが存在したら
        if (instance != null)
        {
            //今回インスタンス化したNetworkSetterを破棄
            Destroy(this.gameObject);
            return;
        }
        //NetworkSetterインスタンスがなかったら
        else if (instance == null)
        {
            //このNetworkSetterをインスタンスとする
            instance = this;
        }
        //シーンを跨いでもNetworkSetterインスタンスを破棄しない
        DontDestroyOnLoad(gameObject);
    }

    //タイトルで呼び出す。
    public void NetSettingData( string State, string Ip , string port )
    {
        NetworkState = State;
        NetworkIp = Ip;
        NetworkPort = port;
    }
    public string GetIpSetting()
    {
        if( NetworkIp == null )
        {
            NetworkIp = "-1";
        }
        return NetworkIp;
    }
    public string GetState()
    {
        return NetworkState;
    }
}