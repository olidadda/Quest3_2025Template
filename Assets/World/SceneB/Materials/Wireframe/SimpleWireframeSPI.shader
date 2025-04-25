Shader "Custom/SimpleWireframeSPI"
{
    Properties
    {
        _WireColor ("Wireframe Color", Color) = (0, 1, 0, 1) // Default to green
    }
    SubShader
    {
        Tags { "Queue"="Overlay+1" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geo
            #pragma fragment frag
            #pragma target 4.0
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #include "UnityInstancing.cginc"

            fixed4 _WireColor;

            // Input to Vertex Shader
            struct appdata
            {
                float4 vertex : POSITION;
                // Use explicit definition for robustness
                #if defined(UNITY_INSTANCING_ENABLED) || defined(UNITY_STEREO_INSTANCING_ENABLED)
                    uint instanceID : SV_InstanceID;
                #endif
            };

            // Output from Vertex Shader / Input to Geometry Shader
            struct v2g
            {
                float4 worldPos : TEXCOORD0;
                // *** ADD THIS BACK IN (Defines member needed by macros) ***
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // Output from Geometry Shader / Input to Fragment Shader
            struct g2f
            {
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
                 // Note: Removed instance ID here as frag doesn't need it
            };

            // -- Vertex Shader --
            v2g vert (appdata v)
            {
                v2g o;
                UNITY_SETUP_INSTANCE_ID(v);             // Setup here
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); // Initialize stereo output
                // UNITY_TRANSFER_INSTANCE_ID(v, o);    // Keep this removed

                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            // -- Geometry Shader --
            [maxvertexcount(6)]
            void geo(triangle v2g input[3], inout LineStream<g2f> lineStream)
            {
                g2f p1, p2; // Output vertices for the line segment

                // --- REMOVE INSTANCE SETUP FROM GEO ---
                // UNITY_SETUP_INSTANCE_ID(input[0]); // Let's assume vert's setup is sufficient

                // Initialize stereo output for the vertices we will create
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(p1);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(p2);
                // Note: We don't need to transfer instance ID to p1/p2 if frag doesn't use it

                // Edge 1: input[0] to input[1]
                p1.vertex = UnityWorldToClipPos(input[0].worldPos);
                UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(input[0], p1); // Copy stereo state FOR p1 from input[0]
                lineStream.Append(p1);

                p2.vertex = UnityWorldToClipPos(input[1].worldPos);
                UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(input[1], p2); // Copy stereo state FOR p2 from input[1]
                lineStream.Append(p2);
                lineStream.RestartStrip();

                // Edge 2: input[1] to input[2]
                p1.vertex = UnityWorldToClipPos(input[1].worldPos);
                UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(input[1], p1);
                lineStream.Append(p1);

                p2.vertex = UnityWorldToClipPos(input[2].worldPos);
                UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(input[2], p2);
                lineStream.Append(p2);
                lineStream.RestartStrip();

                // Edge 3: input[2] to input[0]
                p1.vertex = UnityWorldToClipPos(input[2].worldPos);
                UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(input[2], p1);
                lineStream.Append(p1);

                p2.vertex = UnityWorldToClipPos(input[0].worldPos);
                UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(input[0], p2);
                lineStream.Append(p2);
                lineStream.RestartStrip();
            }

            // -- Fragment Shader --
            fixed4 frag (g2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); // Required setup
                return _WireColor;
            }
            ENDCG
        }
    }
    Fallback Off
}