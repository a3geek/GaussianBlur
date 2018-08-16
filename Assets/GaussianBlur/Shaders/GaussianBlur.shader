Shader "Hidden/GaussianBlur"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct WeightInfo
            {
                float weight;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 offset : TEXCOORD0;  // xy...Vertical, zw...Horizontal.
                float4 coord : TEXCOORD1;   // xy...Vertical, zw...Horizontal.
            };

            sampler2D _MainTex;
            float4 _MainTex_ST, _MainTex_TexelSize;
            uniform int _SamplingCount;
            uniform float _SamplingDistance;
            uniform StructuredBuffer<WeightInfo> _Weights;
            
            v2f vert(appdata_full v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float2 uv = TRANSFORM_TEX(v.texcoord, _MainTex);
            
                // Sampling offset.
                o.offset = float4(
                    _MainTex_TexelSize.xy * half2(0.0, 1.0) * _SamplingDistance,
                    _MainTex_TexelSize.xy * half2(1.0, 0.0) * _SamplingDistance
                );

                // First sampling points uv.
                o.coord = float4(
                    uv - o.offset.xy * ((_SamplingCount - 1) * 0.5),
                    uv - o.offset.zw * ((_SamplingCount - 1) * 0.5)
                );

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 col = 0;

                // Vertical.
                for (int j = 0; j < _SamplingCount; j++) {
                    // Sampling and weighting.
                    // Multiply by 0.5 is that for the later calculation for horizontal direction.
                    col += tex2D(_MainTex, i.coord.xy) * _Weights[j].weight * 0.5;
                    // Shifted sampling point by offset.
                    i.coord.xy += i.offset.xy;
                }

                // Horizontal.
                for (j = 0; j < _SamplingCount; j++) {
                    col += tex2D(_MainTex, i.coord.zw) * _Weights[j].weight * 0.5;
                    i.coord.zw += i.offset.zw;
                }

                return col;
            }
            ENDCG
        }
    }
}
