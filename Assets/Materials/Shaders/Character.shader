// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Character"  
{  
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _AlphaCutOff ("AlphaCutOff", Range(0,1)) = 0.05
    }

    SubShader
    {
        Pass
        {
            Tags {"LightMode"="UniversalForward"}

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            fixed _AlphaCutOff;

            struct appdata
            {
                half3 normal : NORMAL;
                float4 vertex : POSITION;
                float2 uv : TEXCOORD;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD;
                fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float3 cameraDir = ObjSpaceViewDir(v.vertex);
                fixed3 backDir = normalize(float3(0, -cameraDir.y, -cameraDir.z));
                fixed3 upDir = fixed3(0, 1, 0);
                fixed3 rightDir = normalize(cross(upDir, backDir));
                upDir = normalize(cross(backDir, rightDir));
                v.vertex.xyz = mul(v.vertex.xyz, fixed3x3(rightDir, upDir, backDir));
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                clip(col.a - _AlphaCutOff);
                return col;
            }
            ENDCG
        }
        
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        
    }
}  
