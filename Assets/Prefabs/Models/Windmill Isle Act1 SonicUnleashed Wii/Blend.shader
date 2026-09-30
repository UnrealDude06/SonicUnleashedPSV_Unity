Shader "Custom/EdgeBlendShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BlendTex ("Blend (RGB)", 2D) = "white" {}
        _BlendStrength ("Blend Strength", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        sampler2D _BlendTex;
        float _BlendStrength;

        struct Input
        {
            float2 uv_MainTex;
        };

        half4 LightingStandard(SurfaceOutputStandard s, half3 lightDir, half atten)
        {
            half4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * (atten * 2);
            c.a = s.Alpha;
            return c;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            half4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
            half4 blendTex = tex2D(_BlendTex, IN.uv_MainTex);

            // Calculate blend factor based on proximity to left and right edges
            float edgeDistance = min(IN.uv_MainTex.x, 1.0 - IN.uv_MainTex.x);
            float blendFactor = smoothstep(0.0, 0.1, edgeDistance) * _BlendStrength;

            o.Albedo = lerp(mainTex.rgb, blendTex.rgb, blendFactor);
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
