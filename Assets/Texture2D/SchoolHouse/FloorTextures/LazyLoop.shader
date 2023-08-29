Shader "Custom/ParallaxWindowDepthPerspective"
{
    Properties
    {
        _MainTex ("Window Texture (with transparency)", 2D) = "white" {}
        _SecondTex ("Background Texture", 2D) = "white" {}
        _Parallax ("Parallax Amount", Range(0.01, 5)) = 0.1
        _DepthFactor ("Depth Factor", Range(1, 10)) = 3.0
        _MainColor ("Main Texture Color", Color) = (1, 1, 1, 1)
        _SecondColor ("Second Texture Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
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
                float3 worldPos : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uvFar : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            sampler2D _SecondTex;

            float _Parallax;
            float _DepthFactor;

            float4 _MainTex_ST;
            float4 _SecondTex_ST;

            float4 _MainColor;
            float4 _SecondColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw; // Applying tiling and offset for the main texture
                o.worldPos = mul(unity_WorldToObject, v.vertex).xyz;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // Direct difference in position to get the view direction in object space
                float3 viewDirObjSpace = normalize(i.worldPos - _WorldSpaceCameraPos.xyz);

                // Project the view direction onto a plane to simulate a "sky"
                float2 projection = viewDirObjSpace.xy / (1.0f - viewDirObjSpace.z);

                // Apply the parallax and depth factor
                float depthFactor = max(viewDirObjSpace.z, 0.1); // Ensure we don't divide by near-zero values
                i.uvFar = (i.uv + projection * _Parallax / (_DepthFactor * depthFactor)) * _SecondTex_ST.xy + _SecondTex_ST.zw; // Applying tiling and offset for the second texture

                half4 mainCol = tex2D(_MainTex, i.uv) * _MainColor;  // Applying color effect
                half4 distantCol = tex2D(_SecondTex, frac(i.uvFar)) * _SecondColor;  // Applying color effect

                // Check the transparency of the main texture. If it's transparent, show the distant texture.
                if (mainCol.a < 0.1)
                    return distantCol;

                return mainCol;
            }
            ENDCG
        }
    }
}
