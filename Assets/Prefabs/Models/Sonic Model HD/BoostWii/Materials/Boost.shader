Shader "Custom/DoubleSidedBoostAuraShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.5
        _DistortionSpeed ("Distortion Speed", Range(0, 10)) = 1.0
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Alpha ("Alpha", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Cull Off // Disable back face culling
        
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        
        struct Input
        {
            float2 uv_MainTex;
        };
        
        sampler2D _MainTex;
        sampler2D _NoiseTex;
        float _NoiseStrength;
        float _DistortionSpeed;
        fixed4 _Color;
        float _Alpha;
        
        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Sample main texture
            fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
            
            // Calculate distortion using noise texture
            float2 distortion = tex2D(_NoiseTex, IN.uv_MainTex * _DistortionSpeed).rg * 2.0 - 1.0;
            distortion *= _NoiseStrength;
            
            // Apply distortion to UVs
            float2 distortedUV = IN.uv_MainTex + distortion;
            
            // Sample distorted texture
            fixed4 finalColor = tex2D(_MainTex, distortedUV) * _Color;
            
            // Apply alpha
            finalColor.a *= _Alpha;
            
            // Output final color
            o.Albedo = finalColor.rgb;
            o.Alpha = finalColor.a;
        }
        ENDCG
        
        // Second pass for back faces
        Blend SrcAlpha OneMinusSrcAlpha // Blend mode for back faces
        Pass
        {
            Cull Front // Render only back faces
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float _NoiseStrength;
            float _DistortionSpeed;
            fixed4 _Color;
            float _Alpha;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                // Sample main texture
                fixed4 mainTex = tex2D(_MainTex, i.uv);
                
                // Calculate distortion using noise texture
                float2 distortion = tex2D(_NoiseTex, i.uv * _DistortionSpeed).rg * 2.0 - 1.0;
                distortion *= _NoiseStrength;
                
                // Apply distortion to UVs
                float2 distortedUV = i.uv + distortion;
                
                // Sample distorted texture
                fixed4 finalColor = tex2D(_MainTex, distortedUV) * _Color;
                
                // Apply alpha
                finalColor.a *= _Alpha;
                
                // Output final color
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}