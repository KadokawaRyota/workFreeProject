//------------------------------------------------------------------------------
//          ファイルインクルード
//------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//------------------------------------------------------------------------------
//          メイン
//------------------------------------------------------------------------------
public class Scr_ControllerManager : MonoBehaviour 
{
    //--------------------------------------------------------------------------
    //          変数定義
    //--------------------------------------------------------------------------
        ////
        ////////////////////////////////////////////////////////////////////////
        InputManager inputManager;                  // インプットマネージャー

        ////        コントローラ
        ////////////////////////////////////////////////////////////////////////
        public Vector3 TouchPositionStart;          // タップ開始位置
        public Vector3 TouchPositionNow;            // 現在のタップ位置
        public Vector3 ControllerVec;               // タップ点の始点から終点へのベクトル
        public float   fControllerVecLength;        // ベクトルの長さ
        public static Touch tTouchInfo;             // タップ情報
        private PunipuniController PuniPuniController;

    //--------------------------------------------------------------------------
    //          初期化処理
    //--------------------------------------------------------------------------
	void Start () 
    {
        ////       インプットマネージャ生成
        ////////////////////////////////////////////////////////////////////////
        inputManager = InputManager.Instance;

        PuniPuniController = GameObject.Find("PuniconCamera/Punicon").GetComponent<PunipuniController>();
    }

    //--------------------------------------------------------------------------
    //          更新処理
    //--------------------------------------------------------------------------
    public void Update()
    {
        ////        エディタでの更新処理
        ////////////////////////////////////////////////////////////////////////
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            if (InputManager.GetTouchTrigger()) TouchTrigger();                                     // タップされた瞬間
            else if (InputManager.GetTouchPress() && !InputManager.GetTouchTrigger()) TouchMove();  // タップをキープしている状態
            else if (InputManager.GetTouchRelease()) TouchRelease();                                // タップを解除した瞬間
        }

        ////        モバイルでの更新処理
        ////////////////////////////////////////////////////////////////////////
        else if (Application.isMobilePlatform)
        {
            tTouchInfo = Input.GetTouch(0);
            if (Input.touchCount >= 1)
            {
                if (tTouchInfo.phase == TouchPhase.Began) TouchTrigger();
                else if (tTouchInfo.phase == TouchPhase.Moved || tTouchInfo.phase == TouchPhase.Stationary) TouchMove();
                else if (tTouchInfo.phase == TouchPhase.Ended) TouchRelease();
            }
        }

        PuniPuniController.PuniPuniUpdate();
    }

    //--------------------------------------------------------------------------
    //          タップされた瞬間(トリガー処理)
    //--------------------------------------------------------------------------    
    public void TouchTrigger()
    {
        ////        エディタでの更新処理
        ////////////////////////////////////////////////////////////////////////
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            TouchPositionStart  = InputManager.GetTouchPosition();
            TouchPositionNow    = InputManager.GetTouchPosition();
        }

        ////        モバイルでの更新処理
        ////////////////////////////////////////////////////////////////////////
        else if (Application.isMobilePlatform)
        {
            ////        始点を設定
            ////////////////////////////////////////////////////////////////////
            TouchPositionStart = tTouchInfo.position;
            TouchPositionNow = tTouchInfo.position;
        }
    }

    //--------------------------------------------------------------------------
    //          タップされている(ムーブ処理)
    //--------------------------------------------------------------------------
    void TouchMove()
    {
        ////        エディタでの更新処理
        ////////////////////////////////////////////////////////////////////////
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            TouchPositionNow = InputManager.GetTouchPosition();
        }

        ////        モバイルでの更新処理
        ////////////////////////////////////////////////////////////////////////
        else if (Application.isMobilePlatform)
        {
            ////        終点を設定
            ////////////////////////////////////////////////////////////////////////
            TouchPositionNow = tTouchInfo.position;
        }

        ////        始点と終点の方向ベクトル生成
        ////////////////////////////////////////////////////////////////////////
        ControllerVec = (TouchPositionNow - TouchPositionStart);
        fControllerVecLength = ControllerVec.magnitude;
    }

    //--------------------------------------------------------------------------
    //          タップが解除された(リリース処理)
    //--------------------------------------------------------------------------
    public void TouchRelease()
    {
        TouchPositionNow = Vector3.zero;
        ControllerVec = Vector3.zero;
        fControllerVecLength = 0.0f;
    }

    //--------------------------------------------------------------------------
    //          コントローラのタップ位置取得
    //--------------------------------------------------------------------------
    public Vector3 GetControllerTouchPos()
    {
        return tTouchInfo.position;
    }

    //--------------------------------------------------------------------------
    //          ぷにコンの長さを取得
    //--------------------------------------------------------------------------
    public Vector3 GetControllerVec()
    {
        return ControllerVec;
    }
}



