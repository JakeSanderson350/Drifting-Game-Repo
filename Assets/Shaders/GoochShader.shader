Shader "Unlit/GoochShader"
{
    Properties
    {
        _DarkColor ("Dark Color (Cool)", Color) = (0, 0, 1, 1) // Default Blue
        _LightColor ("Light Color (Warm)", Color) = (1, 1, 0, 1) // Default Yellow
        _BlendFactor ("Color Blend Subtlety", Range(0.1, 3.0)) = 1.0 // Controls color transition subtlety
        _EdgeColor ("Edge Color", Color) = (0, 0, 0, 1) // Black edge color
        _EdgeThreshold ("Edge Threshold", Range(0.0, 1.0)) = 0.1 // Controls edge strength
        _EdgeWidth ("Edge Width", Range(0.0, 2.0)) = 1.0 // Controls width of silhouette edges
        
        // Specular and rim highlight properties
        _SpecColor ("Specular Color", Color) = (1, 1, 1, 1) // Specular highlight color
        _SpecPower ("Specular Power", Range(1, 128)) = 64 // Specular power/shininess
        _RimColor ("Rim Light Color", Color) = (1, 1, 1, 1) // Rim lighting color
        _RimPower ("Rim Power", Range(0.1, 10.0)) = 3.0 // Controls rim light falloff
        
        // Fresnel effect properties
        _FresnelColor ("Fresnel Color", Color) = (0.5, 0.5, 1.0, 1) // Color for fresnel effect
        _FresnelPower ("Fresnel Power", Range(0.1, 10.0)) = 2.0 // Controls fresnel falloff
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        // First pass - Edge detection
        Pass
        {
            Cull Front // Render back faces first
            ZWrite On
            ZTest LEqual
            Offset 0, -1 // Fix for z-fighting
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            float _EdgeWidth;
            fixed4 _EdgeColor;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                float3 normal = normalize(UnityObjectToWorldNormal(v.normal));
                // Convert normal back to object space for consistent scaling
                normal = normalize(mul((float3x3)unity_WorldToObject, normal));
                
                // Scale the extrusion based on screen-space considerations
                float3 extrudedVertex = v.vertex.xyz + normal * _EdgeWidth * 0.005;
                o.pos = UnityObjectToClipPos(float4(extrudedVertex, 1.0));
                
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                return _EdgeColor;
            }
            ENDCG
        }
        
        // Main Pass - Gooch shading with enhancements
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"
            
            sampler2D _CameraDepthTexture;
            float _EdgeThreshold;
            fixed4 _EdgeColor;
            fixed4 _DarkColor;
            fixed4 _LightColor;
            float _BlendFactor;
            
            // Specular properties
            float4 _SpecColor;
            float _SpecPower;
            
            // Rim light properties
            float4 _RimColor;
            float _RimPower;
            
            // Fresnel properties
            float4 _FresnelColor;
            float _FresnelPower;
            
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
                float4 screenPos : TEXCOORD3;
                float3 viewDir : TEXCOORD4;
            };
            
            // Vertex shader
            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.pos);
                
                // Calculate view direction for rim lighting and fresnel
                float3 worldViewDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
                o.viewDir = worldViewDir;
                
                return o;
            }
            
            // Fragment shader
            fixed4 frag(v2f i) : SV_Target
            {
                // Calculate Gooch shading
                float3 normalDir = normalize(i.normal);
                float3 viewDir = normalize(i.viewDir);
                float3 lightDir;
                
                if (_WorldSpaceLightPos0.w == 0) // Directional Light
                {
                    lightDir = normalize(_WorldSpaceLightPos0.xyz);
                }
                else // Point Light
                {
                    lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                }
                
                // Base Gooch shading
                float baseLightIntensity = dot(normalDir, lightDir);
                float blendedIntensity = pow((baseLightIntensity * 0.5 + 0.5), _BlendFactor);
                blendedIntensity = smoothstep(0.2, 0.8, blendedIntensity);
                
                float3 goochColor = lerp(_DarkColor.rgb, _LightColor.rgb, blendedIntensity);
                
                // Specular highlight
                float3 halfVector = normalize(lightDir + viewDir);
                float specularIntensity = pow(max(0, dot(normalDir, halfVector)), _SpecPower);
                float3 specular = _SpecColor.rgb * specularIntensity;
                
                // Rim lighting (edge highlighting based on view angle)
                float rimFactor = 1.0 - max(0, dot(normalDir, viewDir));
                rimFactor = pow(rimFactor, _RimPower);
                float3 rim = _RimColor.rgb * rimFactor;
                
                // Fresnel effect for depth perception
                float fresnel = pow(1.0 - max(0, dot(normalDir, viewDir)), _FresnelPower);
                float3 fresnelColor = _FresnelColor.rgb * fresnel;
                
                // Combine all lighting effects
                float3 finalColor = goochColor + specular + rim + fresnelColor;
                
                return float4(finalColor, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}