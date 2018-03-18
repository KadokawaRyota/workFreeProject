//****************************************************
// アウトライントゥーンシェーダ
//****************************************************
Shader "Custom/OutlineToon" {
	Properties{
		//使用宣言
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_RampTex("Ramp", 2D) = "white"{}
	_OutlineColor("Outline Color", Color) = (0,0,0,0)
		_Outline("Outline",Range(0.001,0.005)) = 0.001
	}
		SubShader{
		Tags{ "RenderType" = "Transparent"
		"Queue" = "Transparent" }
		LOD 200

		//アウトラインシェーダ
		Pass{
		Cull Front	//裏面描画

		Stencil{
		Ref 2			//ステンシル値
		Comp always		//常にステンシルテストを成功
		Pass Replace	//常にステンシルに書き込み
	}
		CGPROGRAM	//開始

#include "UnityCG.cginc"	//インクルード

#pragma vertex vert			//頂点シェーダ宣言
#pragma fragment frag		//フラグメントシェーダ宣言

		float _Outline;				//アウトライン
	fixed4 _OutlineColor;		//アウトラインカラー

								//取得情報
	struct appdata {
		float4 position : POSITION;		//頂点座標
		float3 normal : NORMAL;			//法線
	};

	//出力情報
	struct v2f {
		float4 position : SV_POSITION;	//頂点座標
		fixed4 color : COLOR0;			//カラー
	};

	//頂点シェーダ
	v2f vert(appdata i)
	{
		float distance = -UnityObjectToViewPos(i.position).z;	//カメラクリップ空間へ変換（モデルと被った部分は見えなくさせるための変換）
		i.position.xyz += i.normal * distance * _Outline;		//モデルを法線の方向へ拡大する

		v2f o;
		o.position = UnityObjectToClipPos(i.position);	//座標変換
		o.color = 0;

		return o;
	}

	//フラグメントシェーダ
	fixed4 frag(v2f o) : SV_Target
	{
		o.color = _OutlineColor;	//カラー代入

	return o.color;
	}

		ENDCG	//終了
	}


		//トゥーンシェーダ
		Stencil{
		Ref 3			//ステンシル値
		Comp Always		//常にステンシルテストを成功
		Pass Replace	//常にステンシルに書き込み
	}

		CGPROGRAM	//開始

#pragma surface surf ToonRamp		//ライト情報が必要なため、サーフェースシェーダを使用
#pragma target 3.0

		sampler2D _MainTex;		//メインテクスチャ
	sampler2D _RampTex;		//トゥーンテクスチャ

							//出力
	struct Input
	{
		float2 uv_MainTex;	//テクスチャ座標	
	};

	fixed4 _Color;	//カラー

					//ライティング用メソッド
	fixed4 LightingToonRamp(SurfaceOutput s, fixed3 lightDir, fixed atten)
	{
		//法線とライトの方向ベクトルの内積値(0.0f～1.0f)
		half d = dot(s.Normal, lightDir) * 0.5f + 0.5f;

		//値に応じた明度のUV座標に設定
		fixed3 ramp = tex2D(_RampTex, fixed2(d,0.5f)).rgb;

		//色の計算
		fixed4 c;
		c.rgb = s.Albedo * _LightColor0.rgb * ramp;
		c.a = 0;
		return c;
	}

	//サーフェースシェーダ
	void surf(Input IN, inout SurfaceOutput o) {

		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;		//テクスチャとカラーを乗算合成
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG	//終了

	}
}
