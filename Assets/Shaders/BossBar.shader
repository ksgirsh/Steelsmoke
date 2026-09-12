Shader"Unlit/Boss_Bar"
{
    Properties
    {

        _Color1 ("Color1", Color ) = (1, 1, 1, 1)
        _Color2 ("Color2", Color ) = (1, 1, 1, 1)

        _Color3("DecayColor", Color ) = (1, 1, 1, 1)

        _Health ("Health", Range(0, 1)) = 0
        _WhiteDecay ("Decay Effect", Range(0, 1)) = 0
        
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
            float4 _Color3;
            float _Health;

            float _WhiteDecay;
            #include "UnityCG.cginc"    
            #define TAU 6.283185307179586
            
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

            float4 frag (Interpolator i) : SV_Target
            {
                //lawl this is complex but im proud of it. i have no idea what i actually did tho im just following my gut
                /*
                float sWhite = lerp(0, _Health, _WhiteDecay);
                float tWhite = InvLerp(0, _Health, _WhiteDecay);
                float white = lerp(0, 1, (tWhite * (1-_Health)));
                */



                //points
                float tFract = InvLerp(0.2, 0.8, i.uv.x);
                float tHealth = InvLerp(0.2, 0.8, _Health);
                //frad
                float4 healthGradient = lerp(_Color1, _Color2, tFract);
                float4 healthLerp = lerp(_Color1, _Color2, tHealth);

                float4 healthColor = healthLerp * healthGradient;
                //black
                float4 bgColor = float4(1,1 ,1, 1);


                //translate uv coords to minus expression. shoulkd be easy but im dumb.
                float4 bgMask = (i.uv.x - (_WhiteDecay - _Health)) < _Health;

                bgColor *= (bgMask * _Color3);

                //all pixels that are more than the current fuel are returned black
                float4 healthBarMask = _Health > i.uv.x;
                
                float4 finalC = lerp(bgColor, healthColor, healthBarMask);
                //_prevHealth = _Health;
                return finalC;
                
                
            }

            ENDCG
        }
    }
}
