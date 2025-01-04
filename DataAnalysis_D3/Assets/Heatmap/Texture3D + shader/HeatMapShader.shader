Shader "Custom/Heatmap3DVolumetric"
{
    Properties
    {
        _HeatmapTexture("Heatmap Texture", 3D) = "" {}
        _GradientTexture("Gradient Texture", 2D) = "white" {}
        _Threshold("Intensity Threshold", Range(0, 1)) = 0.1
        _StepSize("Step Size", Range(0.001, 0.1)) = 0.01
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler3D _HeatmapTexture;
            sampler2D _GradientTexture;
            float _Threshold;
            float _StepSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // World-space position
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Define ray start and end points in volume space
                float3 rayStart = i.uv; // Texture space (0 to 1)
                float3 rayDir = normalize(float3(0, 0, 1)); // View direction
                float3 rayPos = rayStart;

                // Accumulate color
                fixed4 color = fixed4(0, 0, 0, 0);

                // Raymarch through the volume
                [unroll(128)]
                for (float t = 0; t < 1.0; t += _StepSize)
                {
                    float intensity = tex3D(_HeatmapTexture, rayPos).r;

                    if (intensity > _Threshold)
                    {
                        // Map intensity to a color using the gradient texture
                        fixed4 mappedColor = tex2D(_GradientTexture, float2(intensity, 0));
                        mappedColor.a = intensity;

                        // Accumulate color with alpha blending
                        color.rgb += (1.0 - color.a) * mappedColor.rgb * mappedColor.a;
                        color.a += mappedColor.a;

                        // Stop accumulating if fully opaque
                        if (color.a >= 1.0) break;
                    }

                    // Advance the ray
                    rayPos += rayDir * _StepSize;
                }

                return color;
            }
            ENDCG
        }
    }

            FallBack "Transparent"
}