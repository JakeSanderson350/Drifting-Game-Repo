Shader "Unlit/GoochShaderWithDepthEdgeDetection"
{
    Properties
    {
        _DarkColor ("Dark Color (Cool)", Color) = (0, 0, 1, 1) // Default Blue
        _LightColor ("Light Color (Warm)", Color) = (1, 1, 0, 1) // Default Yellow
        _BlendFactor ("Color Blend Subtlety", Range(0.1, 3.0)) = 1.0 // Controls color transition subtlety
        _EdgeColor ("Edge Color", Color) = (0, 0, 0, 1) // Black edge color
        _EdgeThreshold ("Edge Threshold", Range(0.0, 1.0)) = 0.1 // Controls edge strength
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
            #pragma target 3.0
            #include "UnityCG.cginc"
            
            // Declare the depth texture correctly using lowercase 'sampler2D'
            sampler2D _CameraDepthTexture;
            float _EdgeThreshold;
            fixed4 _EdgeColor;
            fixed4 _DarkColor;
            fixed4 _LightColor;
            float _BlendFactor;
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };
            
            // Vertex shader
            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.uv;
                return o;
            }
            
            // Fragment shader with depth-based edge detection
            fixed4 frag(v2f i) : SV_Target
            {
                // Calculate Gooch shading
                float3 normalDir = normalize(i.normal);
                float3 lightDir;
                
                if (_WorldSpaceLightPos0.w == 0) // Directional Light
                {
                    lightDir = normalize(_WorldSpaceLightPos0.xyz);
                }
                else // Point Light
                {
                    lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                }
                
                float baseLightIntensity = dot(normalDir, lightDir);
                float blendedIntensity = pow((baseLightIntensity * 0.5 + 0.5), _BlendFactor);
                blendedIntensity = smoothstep(0.2, 0.8, blendedIntensity);
                
                float3 goochColor = lerp(_DarkColor.rgb, _LightColor.rgb, blendedIntensity);
                fixed4 col = float4(goochColor, 1.0);
                
                // Depth-based edge detection
                // Sample the depth texture using the correct function
                // float depth = Linear01Depth(tex2D(_CameraDepthTexture, i.uv));
                
                // // Sample surrounding depths to detect edges (larger filter size for better edge detection)
                // float depthLeft = Linear01Depth(tex2D(_CameraDepthTexture, i.uv + float2(-0.02, 0.0)));
                // float depthRight = Linear01Depth(tex2D(_CameraDepthTexture, i.uv + float2(0.02, 0.0)));
                // float depthUp = Linear01Depth(tex2D(_CameraDepthTexture, i.uv + float2(0.0, 0.02)));
                // float depthDown = Linear01Depth(tex2D(_CameraDepthTexture, i.uv + float2(0.0, -0.02)));
                
                // // Calculate the difference in depth (Sobel-like approach with larger filter)
                // float depthDifference = abs(depth - depthLeft) + abs(depth - depthRight) + abs(depth - depthUp) + abs(depth - depthDown);
                
                // // Apply threshold to detect edges
                // float edgeMask = step(_EdgeThreshold, depthDifference);
                
                // // If depth-based edge detection is not strong enough, use normals to enhance edge detection
                // if (edgeMask == 0)
                // {
                //     float3 normalLeft = normalize(tex2D(_CameraDepthTexture, i.uv + float2(-0.02, 0.0)).xyz);
                //     float3 normalRight = normalize(tex2D(_CameraDepthTexture, i.uv + float2(0.02, 0.0)).xyz);
                //     float3 normalUp = normalize(tex2D(_CameraDepthTexture, i.uv + float2(0.0, 0.02)).xyz);
                //     float3 normalDown = normalize(tex2D(_CameraDepthTexture, i.uv + float2(0.0, -0.02)).xyz);
                    
                //     float normalDifference = abs(dot(i.normal, normalLeft)) + abs(dot(i.normal, normalRight)) + abs(dot(i.normal, normalUp)) + abs(dot(i.normal, normalDown));
                    
                //     // If normal difference is large, this might indicate an edge
                //     edgeMask = step(_EdgeThreshold, normalDifference);
                // }
                
                // Blend edge color with Gooch shading
    
                
                return col;
            }
            ENDCG
        }
    }
}
