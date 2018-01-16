//------------------------------------------------------------------------------
//          ファイルインクルード
//------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//------------------------------------------------------------------------------
//          メイン
//------------------------------------------------------------------------------
public class InputManager : MonoBehaviour
{
    //--------------------------------------------------------------------------
    //          変数定義
    //--------------------------------------------------------------------------
    private static InputManager instance;
    private static Vector3 TouchOldPosition;

    //--------------------------------------------------------------------------
    //          デバッグ表示
    //--------------------------------------------------------------------------
    private InputManager()
    {
        // 生成表示
        //Debug.Log("Create SoundManager instance");
    }

    //--------------------------------------------------------------------------
    //          自己生成関数
    //--------------------------------------------------------------------------
    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("InputManager");
                //DontDestroyOnLoad(obj);
                instance = obj.AddComponent<InputManager>();
            }

            return instance;
        }
    }

    //--------------------------------------------------------------------------
    //          入力判定処理
    //--------------------------------------------------------------------------
    //  Android,iphoneの場合はタッチを検出する
    //  エディタの場合は左クリックとして判定する
    //--------------------------------------------------------------------------
    ////    各状態フラグ
    ///////////////////////////////////////////////////////////////////////////
    private static bool isTouch;            // タップされているか
    private static bool isTouchTrigger;     // トリガー状態か
    private static bool isTouchRelease;     // リリース状態か
    private static bool isTouchMove;        // タップしたまま移動したか
    private static Touch touch;

    //--------------------------------------------------------------------------
    //          初期化処理
    //--------------------------------------------------------------------------
    void Start()
    {
        //////        タップ情報の初期化
        //////////////////////////////////////////////////////////////////////////
        isTouch = false;
        isTouchTrigger = false;
        isTouchRelease = false;
        isTouchMove = false;

        ////        マルチタップ無効
        ////////////////////////////////////////////////////////////////////////
        Input.multiTouchEnabled = false;
    }

    //--------------------------------------------------------------------------
    //          更新処理
    //--------------------------------------------------------------------------
    void Update()
    {
        UpdateTouch();
    }

    //--------------------------------------------------------------------------
    //          タップ状態のアップデート
    //--------------------------------------------------------------------------
    private void UpdateTouch()
    {
        ////        タップ状況初期化
        ////////////////////////////////////////////////////////////////////////
        isTouch = false;
        isTouchTrigger = false;
        isTouchRelease = false;
        isTouchMove = false;

        ////        エディタでの更新処理
        ////////////////////////////////////////////////////////////////////////
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            if(Input.GetMouseButton(0))
            {
                isTouch = true;
            }
            if (Input.GetMouseButtonDown(0))
            {
                isTouchTrigger = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isTouchRelease = true;
            }
        }

        ////        モバイルでの更新処理
        ////////////////////////////////////////////////////////////////////////
        else if (Application.isMobilePlatform)
        {
            //タップ
            if (touch.phase == TouchPhase.Began) isTouchTrigger = true;

            //ドラッグ
            else if (touch.phase == TouchPhase.Moved) isTouchMove = true;

            //タッチ終了
            else if (touch.phase == TouchPhase.Ended) isTouchRelease = true;
        }
    }

    //--------------------------------------------------------------------------
    //          各種判定処理
    //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        //          クリック判定処理
        //--------------------------------------------------------------------------
        public static bool GetTouchPress() { return isTouch; }
        
        //--------------------------------------------------------------------------
        //          トリガー判定処理
        //--------------------------------------------------------------------------
        public static bool GetTouchTrigger() { return isTouchTrigger; }

        //--------------------------------------------------------------------------
        //          リリース判定処理
        //--------------------------------------------------------------------------
        public static bool GetTouchRelease() { return isTouchRelease; }
    
    //--------------------------------------------------------------------------
    //          タップ位置取得
    //--------------------------------------------------------------------------
    public static Vector3 GetTouchPosition()
    {
        Vector3 screenPos = Vector3.zero;

        ////        タップ位置を代入(エディタ)
        ////////////////////////////////////////////////////////////////////////
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
		{
            screenPos = Input.mousePosition;
        }

        ////        タップ位置を代入(モバイル)
        ////////////////////////////////////////////////////////////////////////
        else if (Application.isMobilePlatform)
        {
            screenPos = Input.GetTouch(0).position;
        }

        return screenPos;
    }
}
