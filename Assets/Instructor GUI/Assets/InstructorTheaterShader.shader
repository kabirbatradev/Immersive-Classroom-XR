Shader "Custom/InstructorTheaterShader" {
    Properties {
        _Color1 ("Top Color", Color) = (1,1,1,1)
        _Color2 ("Bottom Color", Color) = (0,0,0,1)
        _Threshold ("Threshold", Range(0.0, 1.0)) = 0.5
    }
    SubShader {
        Tags { "Queue"="Background" }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color1;
            fixed4 _Color2;
            float _Threshold;

            struct appdata_t {
                float4 vertex : POSITION;
            };

            struct v2f {
                float3 pos : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v) {
                v2f o;
                o.pos = v.vertex.xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float blend = smoothstep(-_Threshold, _Threshold, i.pos.y);
                return lerp(_Color1, _Color2, blend);
            }
            ENDCG
        }
    }
}
