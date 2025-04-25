Shader "Custom/RoadShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _OffsetFactor ("Offset Factor", Float) = 10
        _OffsetUnits ("Offset Units", Float) = -100
        _DepthBias ("Depth Bias", Range(0,1)) = 0.001
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        // Add this line to apply the polygon offset
        Offset [_OffsetFactor], [_OffsetUnits]
        
        CGPROGRAM
        // Rest of your shader code remains the same
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}