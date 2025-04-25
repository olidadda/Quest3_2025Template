// Make sure standard Unity helper functions and variables are included
#include "UnityCG.cginc"
// *** ADD THIS: Include file necessary for Instancing and Stereo macros ***
#include "UnityInstancing.cginc"

// Input properties from the ShaderLab block
fixed4 _LineColor;
float _LineSize;

// Input structure from the mesh vertex data
struct appdata
{
    float4 vertex : POSITION;
    float3 normal : NORMAL; // Note: normal isn't used in vert/frag, could potentially be removed if not needed elsewhere
    float2 uv : TEXCOORD0;

    // *** ADD THIS: Macro to enable passing instance ID to the vertex shader ***
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

// Structure passed from vertex shader to fragment shader
struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;

    // *** ADD THIS: Macro to handle passing stereo information (like eye index) ***
    UNITY_VERTEX_OUTPUT_STEREO
};

// Vertex Shader
v2f vert(appdata v) // Input struct 'v'
{
    v2f o; // Output struct 'o'

    // *** ADD THIS: Setup instance ID (required for multi_compile_instancing) ***
    UNITY_SETUP_INSTANCE_ID(v);
    // *** ADD THIS: Initialize stereo output struct members ***
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    // *** ADD THIS: Transfer instance ID from input to output (required for instancing) ***
    UNITY_TRANSFER_INSTANCE_ID(v, o); // Must happen AFTER SETUP_INSTANCE_ID

    // Calculate clip space position - UnityObjectToClipPos handles stereo projection automatically
    o.vertex = UnityObjectToClipPos(v.vertex);

    // Z-offset trick (should be okay with SPI)
    o.vertex.z -= 0.001;

    // Pass UVs
    o.uv = v.uv;

    return o;
}

// Helper function (remains unchanged)
float distanceSq(float2 pt1, float2 pt2)
{
    float2 v = pt2 - pt1;
    return dot(v, v);
}

// Helper function (remains unchanged)
float minimum_distance(float2 v, float2 w, float2 p)
{
    float l2 = distanceSq(v, w);
  // Handle zero-length segment case to avoid division by zero
    if (l2 < 0.00001)
        return distance(p, v);
    float t = max(0, min(1, dot(p - v, w - v) / l2));
    float2 projection = v + t * (w - v);
    return distance(p, projection);
}

// Fragment Shader
fixed4 frag(v2f i) : SV_Target // Input struct 'i'
{
    // *** ADD THIS: Setup stereo eye index (required even if not directly used) ***
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

    // --- Rest of the fragment shader logic remains unchanged ---
    float lineWidthInPixels = _LineSize;
    float lineAntiaAliasWidthInPixels = 1;

    // Use DDX/DDY for screen-space derivative calculations
    float2 uVector = float2(ddx(i.uv.x), ddy(i.uv.x));
    float2 vVector = float2(ddx(i.uv.y), ddy(i.uv.y));

    float vLength = length(uVector);
    float uLength = length(vVector);
    // Avoid potential division by zero if lengths are extremely small
    vLength = max(vLength, 0.00001);
    uLength = max(uLength, 0.00001);
    
    float uvDiagonalLength = length(uVector + vVector);
    uvDiagonalLength = max(uvDiagonalLength, 0.00001);

    float maximumUDistance = lineWidthInPixels * vLength;
    float maximumVDistance = lineWidthInPixels * uLength;
    float maximumUVDiagonalDistance = lineWidthInPixels * uvDiagonalLength;

    float leftEdgeUDistance = i.uv.x;
    float rightEdgeUDistance = (1.0 - leftEdgeUDistance);

    float bottomEdgeVDistance = i.uv.y;
    float topEdgeVDistance = 1.0 - bottomEdgeVDistance;

    float minimumUDistance = min(leftEdgeUDistance, rightEdgeUDistance);
    float minimumVDistance = min(bottomEdgeVDistance, topEdgeVDistance);
    float uvDiagonalDistance = minimum_distance(float2(0.0, 1.0), float2(1.0, 0.0), i.uv);

    float normalizedUDistance = minimumUDistance / maximumUDistance;
    float normalizedVDistance = minimumVDistance / maximumVDistance;
    float normalizedUVDiagonalDistance = uvDiagonalDistance / maximumUVDiagonalDistance;


    float closestNormalizedDistance = min(normalizedUDistance, normalizedVDistance);
    closestNormalizedDistance = min(closestNormalizedDistance, normalizedUVDiagonalDistance);


    float lineAlpha = 1.0 - smoothstep(1.0, 1.0 + (lineAntiaAliasWidthInPixels / lineWidthInPixels), closestNormalizedDistance);

    lineAlpha *= _LineColor.a;

    return fixed4(_LineColor.rgb, lineAlpha);
}