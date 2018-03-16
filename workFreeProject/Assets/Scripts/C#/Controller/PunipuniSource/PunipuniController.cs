//------------------------------------------------------------------------------
//          ファイルインクルード
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

//http://kikikiroku.session.jp/unity5-circle-controller/

//------------------------------------------------------------------------------
//          メイン
//------------------------------------------------------------------------------
public class PunipuniController : MonoBehaviour
{
    //------------------------------------------------------------------------------
    //          変数定義
    //------------------------------------------------------------------------------
    public enum TOUCH_STATE
    {
        NONE,   // 判別してない
        TAP,    // タップ
        HOLD,   // ホールド
    };

    #region [ パブリック ]
    public Scr_ControllerManager ControllerManager; // タップ情報
    public TapEffect tapeffect;                     // タップエフェクト情報
    public SpriteRenderer SpriteRenderHE2D;         // ホールドエフェクトの情報
    public Camera TargetCamera;                     // 描画対象のカメラ
    public Material Material;                       // 描画対象のマテリアル

    public int TapDiscriminationFrame;                  // タップorホールド判別用変数タップorホールド判別時間(〇フレーム以内ならタップ、それ以上ならホールド)
    public int nStateCheckCounter;                      // タップorホールド判別用カウンター
    public TOUCH_STATE TouchState = TOUCH_STATE.NONE;   // タップの状況
    //public static Touch tTouchInfo;                   // タップ情報
    public Vector3 BeginMousePosition;                  // タップ位置
    private bool bStateCountFlug;

    
    #endregion

    #region [ プライベート ]

    ////    半径
    ////////////////////////////////////////////////////////////////////////
    public float RadiusPixel;
    float Radius;

    ////    描画オブジェクト情報
    ////////////////////////////////////////////////////////////////////////
    PunipuniMesh PuniMesh;

    ////    描画設定
    ////////////////////////////////////////////////////////////////////////
    MeshRenderer Renderer;

    ////    ベジェ曲線パラメータ(中央、左、右)
    ////////////////////////////////////////////////////////////////////////
    Bezier BezierC = new Bezier();
    Bezier BezierL = new Bezier();
    Bezier BezierR = new Bezier();
    #endregion

    //--------------------------------------------------------------------------
    //          ぷにこん表示設定(表示のON/OFF)
    //--------------------------------------------------------------------------
    #region プロパティ
    private bool VisiblePunipuniController
        {
            get
            {
                //  コントローラ生成済み
                if( this.Renderer != null ) 
                {
                    // 表示をONに変更
                    return this.Renderer.enabled;
                }
            
                return false;
            }

            set
            {
                //  コントローラ生成済み
                if( this.Renderer != null ) 
                {
                    // 状態変更
                    this.Renderer.enabled = value;
                }
            }
        }
        #endregion

    //--------------------------------------------------------------------------
    //          初期化処理
    //--------------------------------------------------------------------------
        void Start()
        {
            ////    コントローラの半径定義
            ////////////////////////////////////////////////////////////////////
            var p1 = TargetCamera.ScreenToWorldPoint(new Vector3(this.RadiusPixel, this.RadiusPixel, transform.position.z));
            var p2 = TargetCamera.ScreenToWorldPoint(new Vector3(-this.RadiusPixel, -this.RadiusPixel, transform.position.z));
        
            ////    円作成
            ////////////////////////////////////////////////////////////////////
            this.Radius = System.Math.Abs(p1.x - p2.x);

            ////    メッシュを作成しMeshRendererを追加
            ////////////////////////////////////////////////////////////////////
            PuniMesh = new PunipuniMesh(64, this.Radius);
            AddMeshRenderer(gameObject, this.Material);

            ////    MeshRendererを保持しておく
            ////////////////////////////////////////////////////////////////////
            this.Renderer = GetComponent<MeshRenderer>();

            ////    ぷにこんオブジェクトを非表示にしておく
            ////////////////////////////////////////////////////////////////////
            VisiblePunipuniController = false;

            ////    タップ状況判別カウンタを初期化&カウントフラグfalse
            ////////////////////////////////////////////////////////////////////
            nStateCheckCounter = 0;
            bStateCountFlug = false;

            //先端のエフェクト取得
            SpriteRenderHE2D = GameObject.Find("PuniconCamera/TapEffect/HoldEffect2D").GetComponent<SpriteRenderer>();
            
        }

    //--------------------------------------------------------------------------
    //          更新処理
    //--------------------------------------------------------------------------
        public void PuniPuniUpdate()
        {
            ////    タップ状況の更新
            ////////////////////////////////////////////////////////////////////
            UpdateTapState();

            ////        エディタでの更新処理
            ////////////////////////////////////////////////////////////////////////
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (InputManager.GetTouchTrigger()) BeginPunipuni();                                  // タップされた瞬間
                else if (InputManager.GetTouchPress() && !InputManager.GetTouchTrigger()) TrackingPunipuni();   // タップをキープしている状態
                else if (InputManager.GetTouchRelease()) EndPunipuni();                                   // タップを解除した瞬間

            }
            ////        モバイルでの更新処理
            ////////////////////////////////////////////////////////////////////////
            else if (Application.isMobilePlatform)
            {
                if (Scr_ControllerManager.tTouchInfo.phase == TouchPhase.Began) BeginPunipuni();      // タップされた瞬間
                if (Scr_ControllerManager.tTouchInfo.phase == TouchPhase.Moved ||
                    Scr_ControllerManager.tTouchInfo.phase == TouchPhase.Stationary) TrackingPunipuni();   // タップをキープしている状態 
                if (Scr_ControllerManager.tTouchInfo.phase == TouchPhase.Ended)
                {
                    EndPunipuni();        // タップを解除した瞬間
                }
            }
        }
    
    //--------------------------------------------------------------------------
    //          指定GameObjectにMeshRendererを追加
    //--------------------------------------------------------------------------
        void AddMeshRenderer( GameObject target, Material material )
        {
            ////    メッシュ設定
            ////////////////////////////////////////////////////////////////////
            var meshFilter = target.AddComponent<MeshFilter>();
            meshFilter.mesh = PuniMesh.Mesh;

            ////    マテリアル設定
            ////////////////////////////////////////////////////////////////////
            {
                var renderer = target.AddComponent<MeshRenderer>();
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
                renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                renderer.material = material;
            }
        }

    //--------------------------------------------------------------------------
    //          ぷにぷにコントローラーの開始
    //--------------------------------------------------------------------------
        void BeginPunipuni()
        {

            ////    ベジェ曲線とぷニコンメッシュのリセット
            ////////////////////////////////////////////////////////////////////
            ResetPuniMeshAndBezier();

			////    カウントフラグをtrueに
            ////////////////////////////////////////////////////////////////////
			bStateCountFlug = true;
			
            ////    タップ位置を取得
            ////////////////////////////////////////////////////////////////////   
            Vector3 pos = Vector3.zero; 
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                pos = InputManager.GetTouchPosition();
            }

            else if(Application.isMobilePlatform)
            {
                pos = Scr_ControllerManager.tTouchInfo.position;
            }

            ////    コントローラに位置情報を設定
            ////////////////////////////////////////////////////////////////////   
            BeginMousePosition = TargetCamera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 1.0f));
            transform.position = TargetCamera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 1.0f));
            
            //CheckUIObjectRay();
        }

    //--------------------------------------------------------------------------
    //          ぷにぷにコントローラーの終了
    //--------------------------------------------------------------------------
        public void EndPunipuni()
        {
            ////   タップされていた時間が一定以下だった時の処理
            ////////////////////////////////////////////////////////////////////           
            if (TouchState == TOUCH_STATE.NONE && nStateCheckCounter < TapDiscriminationFrame && Input.touchCount > 0)
            {
                ////    タップ状況を「TAP」に変更
                ////////////////////////////////////////////////////////////////
                TouchState = TOUCH_STATE.TAP;           

                ////    タップエフェクト発生
                ////////////////////////////////////////////////////////////////
                tapeffect.SetEffectType(EFFECT_TYPE.TAP);                     // エフェクトタイプセット
                tapeffect.EffectFlug = true;                                  // エフェクト更新フラグON
            }
        ////    ベジェ曲線とぷニコンメッシュのリセット
            ////////////////////////////////////////////////////////////////////
            ResetPuniMeshAndBezier();

            ////    エフェクトOFF
            ////////////////////////////////////////////////////////////////
            if (tapeffect.Effecttype == EFFECT_TYPE.HOLD)
            {
                SpriteRenderHE2D.enabled = false;   // ホールドエフェクトOFF
                tapeffect.EffectStatusReset();      // ステータスリセット
            }

        ////   タップされていた時間が一定以上だった時の処理
        ////////////////////////////////////////////////////////////////////     
        /*            if (Input.touchCount == 0)
                    {
                        ////    ベジェ曲線とぷニコンメッシュのリセット
                        ////////////////////////////////////////////////////////////////////
                        ResetPuniMeshAndBezier();

                        ////    エフェクトOFF
                        ////////////////////////////////////////////////////////////////
                        if (tapeffect.Effecttype == EFFECT_TYPE.HOLD)
                        {
                            SpriteRenderHE2D.enabled = false;   // ホールドエフェクトOFF
                            tapeffect.EffectStatusReset();      // ステータスリセット
                        }       
                    }*/
    }

    //--------------------------------------------------------------------------
    //          ぷにぷにコントローラーの追跡処理
    //--------------------------------------------------------------------------
    void TrackingPunipuni()
    {
        Vector3 Inputpos = Vector3.zero;

        ////    ベジェ曲線パラメータの更新
        ////////////////////////////////////////////////////////////////////
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Inputpos = InputManager.GetTouchPosition();
            tapeffect.EffectPos = InputManager.GetTouchPosition();
        }

        else if (Application.isMobilePlatform)
        {
            Inputpos = Scr_ControllerManager.tTouchInfo.position;
            tapeffect.EffectPos = Scr_ControllerManager.tTouchInfo.position;
        }

        Vector3 pos = TargetCamera.ScreenToWorldPoint(new Vector3(Inputpos.x, Inputpos.y, 1.0f));

        ////    ベジェ曲線位置の更新
        ////////////////////////////////////////////////////////////////////////
        var x = this.BeginMousePosition.x - pos.x;
        var y = this.BeginMousePosition.y - pos.y;

        ////    ベジェ曲線の情報を更新
        ////////////////////////////////////////////////////////////////////////
        UpdateBezierParameter(-x, -y);

        ////    メッシュ情報の更新
        ////////////////////////////////////////////////////////////////////////
        PuniMesh.Vertexes = TransformFromBezier(new Vector3());

        ////    
        ////////////////////////////////////////////////////////////////////////
        var centerPos = BezierC.GetPosition(0.8f);
        PuniMesh.CenterPoint = centerPos;
}

    //--------------------------------------------------------------------------
    //          タップ状況の更新
    //--------------------------------------------------------------------------
    private void UpdateTapState()
    {
        //----------------------------------------------------------------------
        //          カウンタが一定数を超えた場合の処理
        //----------------------------------------------------------------------
        if (TouchState == TOUCH_STATE.NONE && nStateCheckCounter >= TapDiscriminationFrame)
        {
            ////    タップ状況を「ホールド」に変更
            ////////////////////////////////////////////////////////////////////
            TouchState = TOUCH_STATE.HOLD;

            ////    コントローラ表示フラグをTRUE、エフェクト生成
            ////////////////////////////////////////////////////////////////////
            VisiblePunipuniController = true;
            tapeffect.EffectFlug = true;                                  // エフェクト更新フラグON
            tapeffect.SetEffectType(EFFECT_TYPE.HOLD);                    // エフェクトタイプセット
            SpriteRenderHE2D.enabled = true;
        }

        //----------------------------------------------------------------------
        //          タップ状態が(HOLD)の時の位置情報送信処理
        //----------------------------------------------------------------------
        if (tapeffect.EffectFlug == true && tapeffect.Effecttype == EFFECT_TYPE.HOLD && VisiblePunipuniController == true)
        {
            tapeffect.EffectPos = TargetCamera.ScreenToWorldPoint(new Vector3(Scr_ControllerManager.tTouchInfo.position.x,
                                                                              Scr_ControllerManager.tTouchInfo.position.y,
                                                                              1.0f));
        }

        //----------------------------------------------------------------------
        //          カウントフラグがTRUEの場合の処理
        //----------------------------------------------------------------------
        if (TouchState == TOUCH_STATE.NONE && bStateCountFlug) 
        {
            ////    カウンタを加算
            ////////////////////////////////////////////////////////////////////
            nStateCheckCounter++; 
        }
    }

    //--------------------------------------------------------------------------
    //          ベジェ曲線のパラメータの更新処理
    //--------------------------------------------------------------------------
    private void UpdateBezierParameter( float x, float y )
    {
        AnimateBezierParameter(this.BezierC, this.BezierC, x, y);

        { 
            // 他の2個のベジェの開始位置を更新
            var dir = this.BezierC.P2 - this.BezierC.P1;
            var dirL = new Vector2(dir.y, -dir.x);
            var dirR = new Vector2(-dir.y, dir.x);
            dirL = dirL.normalized;
            dirR = dirR.normalized;
            dirL.x = dirL.x * this.Radius + this.BezierC.P1.x;
            dirL.y = dirL.y * this.Radius + this.BezierC.P1.y;
            dirR.x = dirR.x * this.Radius + this.BezierC.P1.x;
            dirR.y = dirR.y * this.Radius + this.BezierC.P1.y;
            this.BezierL.P1 = dirL;
            this.BezierR.P1 = dirR;
        }

        AnimateBezierParameter(this.BezierL, this.BezierC, x, y);
        AnimateBezierParameter(this.BezierR, this.BezierC, x, y);
    }
    
    float ArrivalTime = 10; // frame count(本来はtimeがいい)

    //--------------------------------------------------------------------------
    //          ベジェ曲線のパラメータの更新処理(軌道)
    //--------------------------------------------------------------------------
    private void AnimateBezierParameter(Bezier bez, Bezier baseBez, float x, float y)
    {
        ////    先端点
        ////////////////////////////////////////////////////////////////////////
        bez.P4 = new Vector2(x, y);

        ////    先端点制御点情報
        ////////////////////////////////////////////////////////////////////////
        {
            if (bez.P3 != null)
            {
                var vec = baseBez.P1 - bez.P1;
                vec = vec.normalized * (this.Radius / 4);

                var pos = bez.P3 - bez.P4;
                pos += vec;
                pos /= this.ArrivalTime;
                bez.P3 -= pos;
            }

            else
            {
                bez.P3 = new Vector2(x, y);
            }
        }

        ////    中心制御点
        ////////////////////////////////////////////////////////////////////////
        {
            ////    最終的な位置
            ////////////////////////////////////////////////////////////////////
            var ev = baseBez.P4 - baseBez.P1;
            var len = ev.magnitude;
            ev = ev.normalized;
            ev *= (len / 4);
            ev += bez.P1;

            if (bez.P2 != null)
            {
                var v = ev - bez.P2;
                v /= 3;
                bez.P2 += v;
            }

            else
            {
                bez.P2 = new Vector2(bez.P1.x, bez.P1.y);
            }
        }
    }

    //--------------------------------------------------------------------------
    //          操作対象の頂点インデックスを取得
    //--------------------------------------------------------------------------
    void GetMoveFixedVertexIndex( Vector3 center, out int startIndex, out int endIndex )
    {
        ////    ローカル変数
        ////////////////////////////////////////////////////////////////////////
        Vector3[] points = PuniMesh.OriginalVertexes;   // 頂点情報
        int sidx = -1;                                  // ID
        int eidx = -1;                                  // ID   
        int idx = 0;                                    // インデックスデータ
        bool recheckStart = true;
        bool recheckEnd = true;

        ////    インデックス取得
        ////////////////////////////////////////////////////////////////////////
        for( int n = 0; n < points.Length; n++ ) 
        {
            var point = points[n];
            
            if( BezierL.IsValid ) 
            {
                var PT = point - center;
                var AB = BezierC.P1 - BezierL.P1;
                var c1 = AB.x * PT.y - AB.y * PT.x;

                if (c1 < 0)
                {
                    // move
                    if (recheckStart)
                    {
                        sidx = idx;
                        recheckStart = false;
                        recheckEnd = true;
                    }
                }
                else
                {
                    // fixed
                    if (recheckEnd)
                    {
                        eidx = idx - 1;
                        recheckStart = true;
                        recheckEnd = false;
                    }
                }
            }
            ++idx;
        }
        startIndex = sidx;
        endIndex = eidx;
    }
    
    //--------------------------------------------------------------------------
    //          ベジェ曲線パラメータに応じてメッシュを変形
    //--------------------------------------------------------------------------
    Vector3[] TransformFromBezier(Vector3 center)
    {
        Vector3[] points = PuniMesh.Vertexes;
        Vector3[] org_points = PuniMesh.OriginalVertexes;

        ////    捜査対象の頂点インデックス取得
        ////////////////////////////////////////////////////////////////////////
        int si;
        int ei;
        GetMoveFixedVertexIndex(center, out si, out ei);
        if (ei == -1) ei = points.Length - 1;
        if (si == -1 || ei == -1)
        {
            return org_points;
        }

        if (si > ei) ei += points.Length;
        var useCount = ei - si;
        if (useCount <= 0)
        {
            return org_points;
        }

        int centerIdx = (int)(useCount / 2) + si;
        int count1 = centerIdx - si;
        int count2 = ei - centerIdx;

        for (int n = 0; n < points.Length; n++)
        {
            points[n] = org_points[n];
        }

        for (int n = 0; n < count1; n++)
        {
            float t = (float)(n + 1) / (float)(count1 + 1);
            var point = BezierL.GetPosition(t);

            // 半径内にある場合はオリジナルを使用する
            var dist = new Vector3(point.x, point.y, center.z) - center;
            var idx = (n + si) % points.Length;
            if (dist.magnitude > this.Radius)
            {
                points[idx].x = point.x;
                points[idx].y = point.y;
            }
        }

        for (int n = 0; n < count2; n++)
        {
            float t = (float)(n + 1) / (float)(count2 + 1);
            var point = BezierR.GetPosition(t);

            //  半径内にある場合はオリジナルを使用する
            var dist = new Vector3(point.x, point.y, center.z) - center;
            var idx = (ei - n) % points.Length;
            if (dist.magnitude > this.Radius)
            {
                points[idx].x = point.x;
                points[idx].y = point.y;
            }
        }

        {
            //  半径内にある場合はオリジナルを使用する
            var dist = new Vector3(BezierC.P4.x, BezierC.P4.y, center.z) - center;
            var idx = (centerIdx) % points.Length;
            if (dist.magnitude > this.Radius)
            {
                //  中心
                points[idx].x = BezierC.P4.x;
                points[idx].y = BezierC.P4.y;
            }
        }
        return points;
    }

    //--------------------------------------------------------------------------
    //          ベジェ曲線とぷニコンメッシュのリセット
    //--------------------------------------------------------------------------
    void ResetPuniMeshAndBezier()
    {
        ////    PC or スマホ タッチ判定分岐処理
        ////////////////////////////////////////////////////////////////////
        Vector3 screenPos = Vector3.zero;                        // タップ座標
        screenPos = Input.mousePosition;                         // 座標取得

        ////    初期位置(始点)&現在位置(終点)設定
        ////////////////////////////////////////////////////////////////////
        this.BeginMousePosition = TargetCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 1.0f));
        transform.position = TargetCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 1.0f));

        ////    ベジェ曲線パラメータ設定
        ////////////////////////////////////////////////////////////////////
        var x = 0;
        var y = 0;
        BezierC.P1 = new Vector2(x, y);     // 中心
        BezierC.P2 = new Vector2(x, y);     // 制御点1
        BezierC.P3 = new Vector2(x, y);     // 制御点2
        BezierC.P4 = new Vector2(x, y);     // 終点
        BezierL.P1 = new Vector2(x, y);     // 中心
        BezierL.P2 = new Vector2(x, y);     // 制御点1
        BezierL.P3 = new Vector2(x, y);     // 制御点2
        BezierL.P4 = new Vector2(x, y);     // 終点
        BezierR.P1 = new Vector2(x, y);     // 中心
        BezierR.P2 = new Vector2(x, y);     // 制御点1
        BezierR.P3 = new Vector2(x, y);     // 制御点2
        BezierR.P4 = new Vector2(x, y);     // 終点

        ////    ベジェ曲線の情報を更新
        ////////////////////////////////////////////////////////////////////////
        UpdateBezierParameter(-x, -y);

        ////    メッシュ情報の更新
        ////////////////////////////////////////////////////////////////////////
        PuniMesh.Vertexes = TransformFromBezier(new Vector3());

        ////   メッシュ情報確定
        ////////////////////////////////////////////////////////////////////////
        var centerPos = BezierC.GetPosition(0.8f);
        PuniMesh.CenterPoint = centerPos;

        ////    タップ状況を"NONE"にする。
        ////////////////////////////////////////////////////////////////////////
        TouchState = TOUCH_STATE.NONE;

        ////    カウンタリセット
        ////////////////////////////////////////////////////////////////
        nStateCheckCounter = 0;

        ////    タップ状況判別カウントフラグをfalseに
        ////////////////////////////////////////////////////////////////////
        bStateCountFlug = false;

        ////    コントローラ表示フラグfalse
        ////////////////////////////////////////////////////////////////////
        VisiblePunipuniController = false;
    }
}
