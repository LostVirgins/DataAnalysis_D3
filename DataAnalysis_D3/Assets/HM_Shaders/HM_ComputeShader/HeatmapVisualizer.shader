Shader "Custom/HeatmapVisualizer"
{
    Properties
    {
        _VolumeTex("Volume Texture", 3D) = "white" { }
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler3D _VolumeTex;
            float4 _VolumeTex_TexelSize;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float3 texCoord : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = v.vertex;
                o.texCoord = (v.vertex.xyz + 1.0) * 0.5; // Map to [0,1] range for texture coordinates
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Sample the 3D texture at the given coordinates
                float3 texCoord = i.texCoord;
                half4 color = tex3D(_VolumeTex, texCoord);

                return color; // Returning the heatmap color
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
