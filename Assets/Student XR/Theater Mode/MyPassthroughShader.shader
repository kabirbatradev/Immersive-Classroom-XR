Shader "Unlit/MyPassthroughShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        // extra added from standard passthrough shader thing
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 2 //"Back"
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Float) = 0 //"Off" // do not write z value because it should be infinitely far
        // Actually needs to be On because otherwise the passthrough just doesnt show up... (this results in objects being occluded by the passthrough being placed on the walls)

        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4 //"LessEqual" // do a z test, although this doesnt matter because the renderer queue value is set to before everything including geometry

        // [Enum(UnityEngine.Rendering.BlendOp)] _BlendOpColor("Blend Color", Float) = 2 //"ReverseSubtract"
        // [Enum(UnityEngine.Rendering.BlendOp)] _BlendOpAlpha("Blend Alpha", Float) = 3 //"Min"
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-2"} // set the render queue to 1998 (before 2000 when the geometry renders)
        // this allows for the render 
        // Tags { "RenderType"="Opaque"}
        LOD 100

        BlendOp RevSub
        Blend Zero One, One One

        Pass
        {

            // add these in too
            Cull[_Cull]
            ZWrite[_ZWrite]
            ZTest[_ZTest]
            // BlendOp[_BlendOpColor],[_BlendOpAlpha]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

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
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);
                // // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);
                // return col;
                float alpha = 1; // we want to be able to see passthrough full opacity
                return float4(0, 0, 0, alpha);
            }
            ENDCG
        }
    }
}
