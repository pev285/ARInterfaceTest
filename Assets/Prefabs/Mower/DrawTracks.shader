Shader "Unlit/DrawTracks"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Coordinate("Coordinate", Vector) = (0,0,0,0)
		_Color("Draw Color", Color) = (1,0,0,0)
		_Size("Size", Range(1,500)) = 50
		_Strength("Strength", Range(0, 1)) = 0.5
		//_Speed("Speed", float) = 0.005
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Coordinate;
			fixed4 _Color;
			half _Size, _Strength;
			//float _Speed;
			
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				/*if (col.x > 0)
				{
					col.x = col.x - saturate(_Time.x) * _Speed * 0.01f;
				}*/
				float draw = saturate(1 - distance(i.uv, _Coordinate.xy));
				draw = pow(draw, 500/_Size);
				fixed4 drawcol = _Color * (draw * _Strength);
				fixed4 result = saturate(col + drawcol);
				if (result.r < 0.025)
				{
					return 0;
				}
				return result;
			}
			ENDCG
		}
	}
}
