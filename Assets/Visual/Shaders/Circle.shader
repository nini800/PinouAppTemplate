Shader "Custom/Circle"
{
	Properties
	{
		[NoScaleOffset] _MainTex("Fill Texture", 2D) = "white" {}
		[Toggle] _UseFillTexture("Use Fill Texture", Float) = 0
		_Color("Color", Color) = (1,1,1,1)
		_FillColor("Fill Color", Color) = (1,1,1,1)
		_FillAlphaIntensity("Fill Alpha Intensity", Float) = 1
		_FillAlphaPow("Fill Alpha Pow", Float) = 1
		_FillPan("Fill Pan", Vector) = (0,0,0,0)
		_Intensity("Intensity", Float) = 1
		_Radius("Circle Radius", Range(0, 0.25)) = 0.5
		_SmoothDistance("Smooth Distance", Range(0, 0.25)) = 0.01
		_SmoothPower("Smooth Power", Range(0, 10)) = 1
		_Thickness("Circle Thickness", Range(0, 0.25)) = 0.05
		_Fill("Fill", Range(0, 1)) = 1
		[Toggle] _ClockwiseFill("Clockwise Fill", Float) = 0
		_Rotation("Rotation", Range(-10, 10)) = 1
	}
	SubShader
	{
		Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed _Intensity;
			fixed _UseFillTexture;
			fixed4 _Color;
			fixed4 _FillColor;
			fixed _FillAlphaIntensity;
			fixed _FillAlphaPow;
			fixed4 _FillPan;
			fixed _Thickness;
			fixed _Radius;
			fixed _Fill;
			fixed _Rotation;
			fixed _ClockwiseFill;
			fixed _SmoothDistance;
			fixed _SmoothPower;
			static const fixed twoPi =	  fixed(6.28318530717958647);
			static const fixed piInvert = fixed(0.31830988618379067);

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = (1,1,1,1);
				fixed4 fillTex = tex2D(_MainTex, i.uv + _FillPan.xy * _Time);

				fixed2 relPos = i.uv - (0.5, 0.5);
				fixed2 relPosNormalized = normalize(relPos);
				half distanceFromCenter = length(relPos);
				

				col *= _Intensity * _Intensity * _Color;
				fixed InnerSmoothDistance = pow(clamp(1 + (distanceFromCenter - (_Radius - _Thickness))/_SmoothDistance, 0, 1), _SmoothPower);
				fixed OuterSmoothDistance = pow(clamp(1 + (_Radius - distanceFromCenter)/_SmoothDistance, 0, 1), _SmoothPower);

				fixed4 fillCol = (fillTex * _UseFillTexture * _FillColor) + ((1 - _UseFillTexture) * _FillColor);
				fillCol.a = pow(fillCol.a, _FillAlphaPow) * _FillAlphaIntensity;
				fixed fillColFactor = clamp(1 + (_Radius - distanceFromCenter)/0.000000001, 0, 1);
				fillCol *= fillColFactor;
				col.rgb = lerp(col.rgb, fillCol.rgb, (1 - InnerSmoothDistance) * fillCol.a);

				fixed angle = (relPos.y < 0 ? -acos(relPosNormalized.x) + twoPi : acos(relPosNormalized.x)) / twoPi * sign(1-_ClockwiseFill - 0.5);
				_Rotation = _Rotation % 1;

				angle += (-sign(angle)*0.5 + 0.5);
				angle += (1 - _Rotation);
				angle %= 1;
				fixed angleFactorInner = pow(clamp(1 - (angle - _Fill)/(_SmoothDistance*0.7), 0, 1), _SmoothPower);
				fixed angleFactorOuter = pow(clamp((angle-(1-_SmoothDistance*0.7))	/(_SmoothDistance*0.7), 0, 1), _SmoothPower);

				col.a *=InnerSmoothDistance * OuterSmoothDistance;
				col.a = max(fillCol.a, col.a);
				col.a *= angleFactorInner + angleFactorOuter;
				col.a = min(col.a, OuterSmoothDistance);
				col.a = min(col.a, max(fillCol.a, InnerSmoothDistance));

				//Disappear at fill = 0
				col *= clamp(_Fill / 0.0000000001, 0, 1);

				return col;
			}
			ENDCG
		}
	}
}
