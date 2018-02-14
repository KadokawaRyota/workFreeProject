/****************************************************************************
 * ファイル名:MapLoader.cs
 * タイトル:マップローディング
 * 作成日：2018/1/08
 * 作成者：
 * 説明：CSVファイルを解析して、マップを生成する
 * 更新履歴：1/8:+新規作成
 * **************************************************************************/
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MapLorder : MonoBehaviour
{
    private const string MAP_FILE_NAME = "CSV/Stage001";    //ファイル名


    private TextAsset CsvFile = null;  //空のCSVファイルを入れる箱

    //ブロックの幅
    [SerializeField]
    private int Width;  //幅行の幅
    [SerializeField]
    private int Height; //地面からの床の高さ

    public Vector3 startPosition;   //プレイヤーのスタートポジション
    public Vector3 goalPosition;    //プレイヤーのゴールポジション

    [SerializeField]
    GameObject normalBlock; // 通常床のプレハブ.
    [SerializeField]
    GameObject goalBlock;       // ゴール位置のプレハブ.
    [SerializeField]
    GameObject startBlock;      // スタート位置のプレハブ
    [SerializeField]
    GameObject gameOverLine;
    [SerializeField]
    GameObject AirBlock;
    [SerializeField]
    GameObject Coin;

    GameObject Player = null;
    bool SetPlayer = false;

    // Use this for initialization
    void Start()
    {
        //最後にプレイヤーにスタート地点を送るため、プレイヤーを取得(オンライン時はここはエラーを吐く。)
        if (Player == null)
        {
            Player = GameObject.Find("player");
        }

        //ネットワーク上ではプレイヤーが生成されてなければ取得出来ないので、処理を抜ける。若しくは、スタートがSetPlayer上で呼び出されている。場合
        if (Player == null || SetPlayer ) return;

        // CSVの中身を入れる as TextAssetはキャストのようなもの
        CsvFile = (Resources.Load(MAP_FILE_NAME)) as TextAsset;

        //新しいインスタンスを初期化、stringReaderクラスを指定した文字列から読み取る。
        StringReader Mapreader = new StringReader(CsvFile.text);

        //ステージを作る親を指定
        GameObject StageBlocks = GameObject.Find("StageEditor/StageBlocks");

        //CSVファイル読み溜め
        List<String> lines = new List<string>();
        while (Mapreader.Peek() > -1)
        {
            lines.Add(Mapreader.ReadLine()); //1行読み取って、1行分の文字列を読み込む
        }

        //行数が確定したので、マップの高さが確定する.
        Height = lines.Count;

        //内容の解析.
        int lineCount = 0; // Excelでいう行用インデックス
        foreach (var line in lines)
        {
            string[] values = line.Split(',');  //指定された文字列をカンマ区切りで分割して返す

            //列ぶん繰りかえす valuesの長さ分colを分回す
            for (int col = 0; col < values.Length; col++)
            {
                Vector3 Pos = new Vector3(col * 1.0f, (Height - (lineCount + 1)) * 1.0f, 0.0f); // 生成位置.
                Vector3 scale = Vector3.zero;

                switch (values[col])
                {
                    case "0": // 空
                        {
                            break;
                        }

                    case "N": // 普通の床
                        {
                            GameObject.Instantiate(normalBlock, Pos, Quaternion.identity , StageBlocks.transform); //使用する床プレファブ,ポジション,Rotationを決定
                            break;
                        }
                    case "S": //スタート地点
                        {
                            //スタート地点ブロックの上を求める.
                            GameObject.Instantiate(startBlock, Pos, Quaternion.identity , StageBlocks.transform);   //スタート地点の生成
                            Player.GetComponent<playerSetting>().SetPlayer(Pos + new Vector3(0.0f , 1.0f ,0.0f ));
                            break;
                        }

                    case "G":  //ゴール地点
                        {
                            GameObject.Instantiate(goalBlock, Pos, Quaternion.identity , StageBlocks.transform);  //ゴール地点の生成
                            break;
                        }
                    case "O":   //ゲームオーバーの位置
                        {
                            GameObject.Instantiate(gameOverLine, Pos, Quaternion.identity, StageBlocks.transform);  //ゴール地点の生成
                            break;
                        }
                    case "A":   //空中ブロック
                        {
                            Pos.y += 0.4f;
                            GameObject.Instantiate(AirBlock, Pos, Quaternion.identity, StageBlocks.transform);  //空中ブロックの生成
                            break;
                        }
                    case "C":   //コイン
                        {
                            GameObject.Instantiate(Coin, Pos, Quaternion.identity, StageBlocks.transform);  //コインの生成
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            lineCount++;//行加算
        }
    }
    /***************
    *ネットワーク用*
    ***************/

    //呼び出し元、networkPlayer;
    public void SetNetworkPlayer(GameObject sPlayer)
    {
        //プレイヤーをセット
        Player = sPlayer;
        //スタート呼び出し
        Start();
        //スタート二重で入るのを制限。
        SetPlayer = true;
    }
    
}