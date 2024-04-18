Shader "Custom/AgoraShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ThresholdColor ("Threshold Color", Color) = (1,1,1,1)
        _ThresholdSensitivity ("Threshold Sensitivity", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _ThresholdColor;
            float _ThresholdSensitivity;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.texcoord);
                float d = distance(color.rgb, _ThresholdColor.rgb);
                if (d < _ThresholdSensitivity)
                {
                    discard;
                }
                return color;
            }
            ENDCG
        }
    }
}
