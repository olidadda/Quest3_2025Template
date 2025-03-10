Shader "Custom/Wireframe"
{
    Properties
    {
        _WireColor ("Wire Color", Color) = (0, 1, 0, 1)
        _WireWidth ("Wire Width", Range(0, 0.1)) = 0.01
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
            #pragma geometry geom
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2g
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct g2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 dist : TEXCOORD1;
            };
            
            float4 _WireColor;
            float _WireWidth;
            
            v2g vert (appdata v)
            {
                v2g o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            [maxvertexcount(3)]
            void geom(triangle v2g input[3], inout TriangleStream<g2f> triStream)
            {
                // Compute the distances
                float2 p0 = input[0].vertex.xy / input[0].vertex.w;
                float2 p1 = input[1].vertex.xy / input[1].vertex.w;
                float2 p2 = input[2].vertex.xy / input[2].vertex.w;
                
                // Calculate edge distances
                float2 v0 = p2 - p1;
                float2 v1 = p0 - p2;
                float2 v2 = p1 - p0;
                
                // Calculate the area and distances
                float area = abs(v1.x * v2.y - v1.y * v2.x);
                
                float dist0 = area / length(v0);
                float dist1 = area / length(v1);
                float dist2 = area / length(v2);
                
                g2f o;
                
                // First vertex
                o.vertex = input[0].vertex;
                o.uv = input[0].uv;
                o.dist = float3(dist0, 0, 0);
                triStream.Append(o);
                
                // Second vertex
                o.vertex = input[1].vertex;
                o.uv = input[1].uv;
                o.dist = float3(0, dist1, 0);
                triStream.Append(o);
                
                // Third vertex
                o.vertex = input[2].vertex;
                o.uv = input[2].uv;
                o.dist = float3(0, 0, dist2);
                triStream.Append(o);
            }
            
            fixed4 frag (g2f i) : SV_Target
            {
                // Calculate the minimum distance to any edge
                float minDist = min(min(i.dist.x, i.dist.y), i.dist.z);
                
                // Apply the width factor
                float thickness = _WireWidth * 0.01;
                
                // Calculate the opacity based on distance from the edge
                float opacity = 1.0 - smoothstep(0, thickness, minDist);
                
                // Return the wire color with adjusted opacity
                return float4(_WireColor.rgb, opacity);
            }
            ENDCG
        }
    }
}