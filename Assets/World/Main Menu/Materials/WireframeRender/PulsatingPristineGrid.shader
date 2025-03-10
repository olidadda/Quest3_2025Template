Shader "Pulsating Pristine Grid"
{


    Properties
    {
        [KeywordEnum(MeshUV, WorldX, WorldY, WorldZ)] _UVMode ("UV Mode", Float) = 2.0
        _GridScale ("Grid Scale", Float) = 1.0
        
        _LineWidthX ("Line Width X", Range(0,1.0)) = 0.01
        _LineWidthY ("Line Width Y", Range(0,1.0)) = 0.01

        _LineColor ("Line Color", Color) = (1,1,1,1)
        _BaseColor ("Base Color", Color) = (0,0,0,1)

        // // New properties for pulsation
        _PulseSpeed ("Pulse Speed", Range(0,10)) = 2.0
        _PulseStrength ("Pulse Strength", Range(0,2)) = 1.0

        [HDR]_EmissionColor("Emission Color", Color) = (1,1,1,1)
        _EmissionIntensity("Emission Intensity", Range(0,10)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma shader_feature _ _UVMODE_MESHUV _UVMODE_WORLDX _UVMODE_WORLDZ // _UVMODE_WORLDY is default

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing
            #include "UnityInstancing.cginc"

            // float _PulseSpeed;
            // float _PulseStrength;

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID //Insert
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_OUTPUT_STEREO //Insert
            };

            float _GridScale;

            v2f vert (appdata v)
            {
                //UNITY_SETUP_INSTANCE_ID(v);

                v2f o;

                UNITY_SETUP_INSTANCE_ID(v); //Insert
                UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

                o.pos = UnityObjectToClipPos(v.vertex);

            #if defined(_UVMODE_MESHUV)
                o.uv = v.uv * _GridScale;

                //UNITY_TRANSFER_INSTANCE_ID(v, o);
            #else
                // trick to reduce visual artifacts when far from the world origin
                // keeps world position relative to a grid snapped camera position
                float3 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)).xyz;
                float3 cameraCenteringOffset = floor(_WorldSpaceCameraPos * _GridScale);
                float3 cameraSnappedWorldPos = worldPos * _GridScale - cameraCenteringOffset;

                // float3 cameraSnappedWorldPos = worldPos * _GridScale;
                // o.uv = cameraSnappedWorldPos.xz; // or whichever plane you prefer


            #if defined(_UVMODE_WORLDX)
                o.uv = cameraSnappedWorldPos.xy;
            #elif defined(_UVMODE_WORLDZ)
                o.uv = cameraSnappedWorldPos.yz;
            #else // defined(_UVMODE_WORLDY)
                o.uv = cameraSnappedWorldPos.xz;
            #endif
            #endif

                return o;
            }

            // grid function from Best Darn Grid article
            float PristineGrid(float2 uv, float2 lineWidth)
            {
                lineWidth = saturate(lineWidth);
                float4 uvDDXY = float4(ddx(uv), ddy(uv));
                float2 uvDeriv = float2(length(uvDDXY.xz), length(uvDDXY.yw));
                bool2 invertLine = lineWidth > 0.5;
                float2 targetWidth = invertLine ? 1.0 - lineWidth : lineWidth;
                float2 drawWidth = clamp(targetWidth, uvDeriv, 0.5);
                float2 lineAA = max(uvDeriv, 0.000001) * 1.5;
                float2 gridUV = abs(frac(uv) * 2.0 - 1.0);
                gridUV = invertLine ? gridUV : 1.0 - gridUV;
                float2 grid2 = smoothstep(drawWidth + lineAA, drawWidth - lineAA, gridUV);
                grid2 *= saturate(targetWidth / drawWidth);
                grid2 = lerp(grid2, targetWidth, saturate(uvDeriv * 2.0 - 1.0));
                grid2 = invertLine ? 1.0 - grid2 : grid2;
                return lerp(grid2.x, 1.0, grid2.y);
            }

            float _LineWidthX, _LineWidthY;
            half4 _LineColor, _BaseColor;

                // --- New Emission & Pulsation Props ---
            half4 _EmissionColor;
            float _EmissionIntensity;
            float _PulseSpeed;
            float _PulseStrength;

            fixed4 frag (v2f i) : SV_Target
           {
            // Compute grid mask (1 = line, 0 = base)
            float grid = PristineGrid(i.uv, float2(_LineWidthX, _LineWidthY));

            // Our base color is simply a lerp
            half4 col = lerp(_BaseColor, _LineColor, grid * _LineColor.a);

            // Pulsation factor: a sine wave that goes from 0..1
            float wave = (sin(_Time.y * _PulseSpeed) + 1.0) * 0.5; 
            // Scale wave by PulseStrength
            float pulseFactor = 1.0 + wave * _PulseStrength;

            // Emission = EmissionColor * EmissionIntensity * pulseFactor
            // For bloom to pick it up, use HDR values (like 1..10 or more)
            half3 emissive = _EmissionColor.rgb * _EmissionIntensity * pulseFactor;

            // Add emission into the color only where the grid lines are
            // (If you want glowing lines but no emission in the base area.)
            float lineMask = grid * _LineColor.a; 
            col.rgb += emissive * lineMask;

            // If you're in gamma color space, handle gamma->linear conversions
           #if defined(UNITY_COLORSPACE_GAMMA)
            col.rgb = LinearToGammaSpace(col.rgb);
           #endif

            return col;
           }
            ENDCG
           }
    }
}