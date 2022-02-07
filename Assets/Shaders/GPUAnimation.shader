Shader "Custom/GPUAnimation"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}

        _PosTex("Position Tex", 2D) = "white" {}
        _CurFrameIndex("current frame index", Float) = 0
        _BoundMin("bound min", Float) = 0
        _BoundMax("bound max", Float) = 0
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
                UNITY_DEFINE_INSTANCED_PROP(float, _CurFrameIndex)
                UNITY_DEFINE_INSTANCED_PROP(float, _BoundMin)
                UNITY_DEFINE_INSTANCED_PROP(float, _BoundMax)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                float x = (v.uv3.x + 0.5) * _PosTex_TexelSize.x; // _TexelSize.x : 1/texture_width
                float y = UNITY_ACCESS_INSTANCED_PROP(Props, _CurFrameIndex);
                float4 pos = tex2Dlod(_PosTex, float4(x, y, 0, 0));
                float boundMin = UNITY_ACCESS_INSTANCED_PROP(Props, _BoundMin);
                float expand = UNITY_ACCESS_INSTANCED_PROP(Props, _BoundMax) - boundMin;
                pos.xyz *= expand;
                pos.xyz += boundMin;
                v.vertex = pos;

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
