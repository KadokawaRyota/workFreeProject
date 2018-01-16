//------------------------------------------------------------------------------
//          ファイルインクルード
//------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//------------------------------------------------------------------------------
//          定義
//------------------------------------------------------------------------------
public enum EFFECT_TYPE
{
    NONE = 0,   // 出現していない
    TAP,        // タップエフェクト
    HOLD        // ホールドエフェクト
}; 

//------------------------------------------------------------------------------
//          メイン
//------------------------------------------------------------------------------
public class TapEffect : MonoBehaviour
{
    [SerializeField]
    ParticleSystem tapEffect;   // タップエフェクト

    [SerializeField]
    ParticleSystem HoldEffect;  // ホールドエフェクト

    [SerializeField]
    GameObject HoldEffect2D;    // ホールドエフェクト(2Dオブジェクト部分)

    [SerializeField]
    Camera TargetCamera;        // カメラの座標

    //--------------------------------------------------------------------------
    //          変数定義
    //--------------------------------------------------------------------------
    public bool EffectFlug;             // エフェクト発生フラグ 
    public Vector3 EffectPos;           // エフェクト発生位置
    public EFFECT_TYPE Effecttype;      // エフェクトタイプ
    public int HoldEffectSpawnFrame;    // ホールドエフェクト発生間隔
    int HoldEffectSpawnCounter;         // ホールドエフェクト発生カウンター    

    //--------------------------------------------------------------------------
    //          初期化処理
    //--------------------------------------------------------------------------
    void Start()
    {
        EffectStatusReset();    //    エフェクト情報初期化
    }

    //--------------------------------------------------------------------------
    //          更新処理
    //--------------------------------------------------------------------------
    void Update()
    {
        if (EffectFlug)
        {
            Vector3 pos = Vector3.zero;

            switch (Effecttype)
            {
                case EFFECT_TYPE.NONE:
                    break;

                ////    タップエフェクト
                /////////////////////////////////////////////////////////////////////////////////////
                case EFFECT_TYPE.TAP:
                    
                    // ポジション設定
                    pos = TargetCamera.ScreenToWorldPoint(new Vector3(EffectPos.x, EffectPos.y, 1.0f));
                    tapEffect.transform.position = pos;
                    
                    // スケール値変更
                    tapEffect.transform.localScale = new Vector3(8, 8, 8);
                    
                    // 発生
                    tapEffect.Emit(1);
                    
                    // 情報リセット
                    EffectFlug = false;
                    
                    // デバック表示
                    //Debug.Log("タップエフェクト発生");
                    
                    break;

                ////    ホールドエフェクト
                /////////////////////////////////////////////////////////////////////////////////////
                case EFFECT_TYPE.HOLD:

                    ////        ホールドエフェクト(2D部分)の位置設定
                    /////////////////////////////////////////////////////////////////////////////////
                    pos = TargetCamera.ScreenToWorldPoint(new Vector3(EffectPos.x, EffectPos.y, 1.0f));
                    HoldEffect2D.transform.position = pos;

                    ////        ホールドエフェクト(パーティクル部分)の位置設定
                    /////////////////////////////////////////////////////////////////////////////////
                    pos = TargetCamera.ScreenToWorldPoint(new Vector3(EffectPos.x, EffectPos.y, 1.0f));
                    HoldEffect.transform.position = pos;

                    ////        発生処理
                    /////////////////////////////////////////////////////////////////////////////////
                    if (HoldEffectSpawnFrame <= HoldEffectSpawnCounter)
                    {
                        #region [ 後で戻すかもしれないホールドエフェクトパーティクル設定 ]
                        //pos = TargetCamera.ScreenToWorldPoint(new Vector3(EffectPos.x, EffectPos.y, 1.0f));
                        //tapEffect.transform.position = pos;
                        // ポジション設定
                        //pos = TargetCamera.ScreenToWorldPoint(new Vector3(EffectPos.x, EffectPos.y, 1.0f));
                        //HoldEffect.transform.position = pos;
                        // スケール値変更
                        //tapEffect.transform.localScale = new Vector3(6, 6, 6);

                        // 発生
                        //tapEffect.Emit(1);
                        #endregion
                        
                        // カウンタ初期化
                        HoldEffectSpawnCounter = 0;

                        // スケール値変更
                        HoldEffect.transform.localScale = new Vector3(10, 10, 10);
                        
                        // 発生
                        HoldEffect.Emit(1);
                    }

                    HoldEffectSpawnCounter++;
                    break;

                default:
                    break;
            }
        }
    }

    //--------------------------------------------------------------------------
    //      エフェクトタイプセット関数
    //      (0-NONE 1-TAP 2-HOLD)
    //--------------------------------------------------------------------------
    public void SetEffectType(EFFECT_TYPE type)
    {
        Effecttype = type;
    }

    //--------------------------------------------------------------------------
    //      エフェクト状態初期化
    //--------------------------------------------------------------------------
    public void EffectStatusReset()
    {
        EffectFlug = false;                 // 発生フラグ
        EffectPos = Vector3.zero;           // 発生位置
        if (Effecttype == EFFECT_TYPE.HOLD) // エフェクト消去
        tapEffect.Clear();
        HoldEffect.Clear();
        Effecttype = EFFECT_TYPE.NONE;      // エフェクトタイプ
        HoldEffectSpawnCounter = 0;         // ホールドエフェクト発生カウンター
        HoldEffect2D.transform.position = new Vector3(100.0f, 0.0f, 0.0f);  // 2Dエフェクトの位置リセット
    }
}