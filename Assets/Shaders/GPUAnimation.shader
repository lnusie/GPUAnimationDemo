Shader "Custom/GPUAnimation"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}

		_PosTex("Position Tex", 2D) = "white" {}
		_FrameIndex0("frame index 0", Float) = 0
		_FrameIndex1("frame index 1", Float) = 0
		_BoundMin0("bound min 0", Float) = 0
		_BoundMax0("bound max 0", Float) = 0
		_BoundMin1("bound min 1", Float) = 0
		_BoundMax1("bound max 1", Float) = 0

		_BlendFactor("blend factor", Float) = 0

	}
		SubShader
		{
			Pass
			{
				Name "GPU Animation"

				Tags { "RenderType" = "Opaque" }

				CGPROGRAM
				#pragma enable_d3d11_debug_symbols
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#pragma multi_compile_instancing

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float4 texcoord : TEXCOORD0;
					float2 uv3 : TEXCOORD2;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _PosTex;
				float4 _PosTex_TexelSize;
				float4 _Color;

				UNITY_INSTANCING_BUFFER_START(Props)
					UNITY_DEFINE_INSTANCED_PROP(float, _FrameIndex0)
					UNITY_DEFINE_INSTANCED_PROP(float, _FrameIndex1)
					UNITY_DEFINE_INSTANCED_PROP(float, _BoundMin0)
					UNITY_DEFINE_INSTANCED_PROP(float, _BoundMax0)
					UNITY_DEFINE_INSTANCED_PROP(float, _BoundMin1)
					UNITY_DEFINE_INSTANCED_PROP(float, _BoundMax1)
					UNITY_DEFINE_INSTANCED_PROP(float, _BlendFactor)
				UNITY_INSTANCING_BUFFER_END(Props)

				v2f vert(appdata v)
				{
					v2f o;

					UNITY_SETUP_INSTANCE_ID(v);
					float x = (v.uv3.x + 0.5) * _PosTex_TexelSize.x; // _TexelSize.x : 1/texture_width
					float y0 = UNITY_ACCESS_INSTANCED_PROP(Props, _FrameIndex0);
					float y1 = UNITY_ACCESS_INSTANCED_PROP(Props, _FrameIndex1);

					float4 pos0 = tex2Dlod(_PosTex, float4(x, y0, 0, 0));
					float boundMin = UNITY_ACCESS_INSTANCED_PROP(Props, _BoundMin0);
					float expand = UNITY_ACCESS_INSTANCED_PROP(Props, _BoundMax0) - boundMin;
					pos0.xyz *= expand;
					pos0.xyz += boundMin;

					float4 pos1 = tex2Dlod(_PosTex, float4(x, y1, 0, 0));
				 	boundMin = UNITY_ACCESS_INSTANCED_PROP(Props, _BoundMin1);
					expand = UNITY_ACCESS_INSTANCED_PROP(Props, _BoundMax1) - boundMin;
					pos1.xyz *= expand;
					pos1.xyz += boundMin;
 
					float blendFactor = UNITY_ACCESS_INSTANCED_PROP(Props, _BlendFactor);

					v.vertex = lerp(pos0, pos1, blendFactor); 

					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);

					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					return tex2D(_MainTex, i.uv.xy) * _Color;
				}
				ENDCG
			}
		}
			FallBack "Diffuse"
}
