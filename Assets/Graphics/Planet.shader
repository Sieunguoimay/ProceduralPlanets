Shader "Custom/Planet"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _ElevationMinMax ("ElevationMinMax", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _ElevationMinMax;

        float inverseLerp(float a, float b, float t) {
            return (t - a) / (b - a);
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float elevation = IN.uv_MainTex.y;
            float belowZero = min(0, elevation);
            float aboveZero = max(0, elevation);
            float min = _ElevationMinMax.x;
            float max = _ElevationMinMax.y;

            belowZero = inverseLerp(min, 0, belowZero);
            aboveZero = inverseLerp(0, max, aboveZero);

            float floored = floor(belowZero);
            float oneMinus = 1 - floored;
            belowZero = belowZero * 0.5f * oneMinus;

            aboveZero = (aboveZero * 0.5f + 0.5f) * floored;

            float uv_x = belowZero + aboveZero;
            float uv_y = IN.uv_MainTex.x;

            fixed4 c = tex2D(_MainTex, fixed2(uv_x, uv_y));

            // Albedo comes from a texture tinted by color
            //fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
