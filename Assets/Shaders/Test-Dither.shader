Shader"Unlit/Test-Dither"
{
    Properties
    {

        _Color1 ("Color1", Color ) = (1, 1, 1, 1)
        _Color2 ("Color2", Color ) = (1, 1, 1, 1)
        _MainTex ("Main Texture", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            
            float4 _Color1;
            float4 _Color2;
            #include "UnityCG.cginc"    
            #define TAU 6.283185307179586
            
            const float gamma = 2.2;
            const float4 pixel_order = float4(0,2,3,1);
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normals : NORMAL;
                float2 uv0 : TEXCOORD0;
            };

            struct Interpolator
            {
                //float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD1;
 
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            Interpolator vert (appdata v)
            {
                Interpolator o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal( v.normals );
                o.uv = v.uv0;
                return o;
            }
            
            float InvLerp(float a, float b, float v)
            {
                    return (v - a) / (b - a);
    
            }

            float4 fragColor;

            float4 frag (Interpolator i) : SV_Target
            {
                fragColor = float4(0.75, i.uv.y, 0, 1);
                return fragColor;
                
            }

            ENDCG
        }
    }
}
