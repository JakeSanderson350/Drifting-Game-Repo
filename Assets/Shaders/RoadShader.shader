Shader "Custom/RoadShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _OffsetFactor ("Offset Factor", Float) = -1
        _OffsetUnits ("Offset Units", Float) = -1
        _ZBias ("Z Bias", Float) = 0.001
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-100" }
        LOD 200
        
        // Use ZWrite and ZTest for better depth control
        ZWrite On
        ZTest LEqual
        
        // Apply polygon offset
        Offset [_OffsetFactor], [_OffsetUnits]
        
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0
        
        sampler2D _MainTex;
        float _ZBias;
        
        struct Input
        {
            float2 uv_MainTex;
        };
        
        void vert(inout appdata_full v) 
        {
            v.vertex.z -= _ZBias;
        }
        
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