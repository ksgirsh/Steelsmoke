Shader"Unlit/Fuel_Ring"
{
    Properties
    {

        _Color1 ("MinFuel", Color ) = (1, 1, 1, 1)
        _Color2 ("MaxFuel", Color ) = (1, 1, 1, 1)

        //_Color3 ("UltCharged", Color ) = (1, 1, 1, 1)


        _Value ("Fuel", Range(0, 1)) = 0
        _ringThickness ("Thickness", Range(0, 1)) = 0.02
        _radius ("radius", Range(0, 1)) = 0.2
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
            
            
            float4 _Color1;
            float4 _Color2;

          //  float4 _Color3;
            float _Value;
            #include "UnityCG.cginc"    
            #define TAU 6.283185307179586

            float _ringThickness;
            float _radius;

            float threshold = 0.999;


            
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

                //center uv
                o.uv = (v.uv0 * 2) - 1;
                return o;
            }
            
            float InvLerp(float a, float b, float v)
            {
                    return (v - a) / (b - a);
    
            }

            float4 frag (Interpolator i) : SV_Target
            {
                
    
                //black
                float4 bgColor = float4(0,0 ,0, 0);
    
                //all pixels that are more than the current fuel are returned black
                float4 fuelBarMask = _Value > i.uv.x;


                //polar coordinates
                			float angle = atan2(i.uv.y, i.uv.x);
                            float dist = length(i.uv);
                            float normalizedAngle = (angle + 3.14159265) / TAU;
                
                //all angles that are more than the current fuel are returned black
                float angleMask = _Value > normalizedAngle;

                float ringMask = dist > (_radius - _ringThickness) && dist < (_radius + _ringThickness);

                //return lerp(_Color2, _Color1, angleMask);
                float finalMask = angleMask * ringMask;

                float tFract = InvLerp(0, 1, normalizedAngle);
                float4 gradientColor = lerp(_Color1, _Color2, tFract);
                float4 lerpColor = lerp(_Color1, _Color2, _Value);

                //frad
                float4 fuelColor = lerpColor;

                float4 finalC = lerp(bgColor, fuelColor, finalMask);

                /*
                if (finalC.xyz == float3(0,0,0))
                {
                    finalC.a = 0;
                }*/
                float luminance = (finalC.r + finalC.g + finalC.b) / 3.0;

                // clip() discards pixels where the value is negative
                clip(luminance - 0.3);

                return finalC;
            }

            ENDCG
        }
    }
}
