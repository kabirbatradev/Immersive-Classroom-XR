/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

Shader "TheWorldBeyond/StandardPassthrough" {
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _InvertedAlpha("Inverted Alpha", float) = 1

        [Header(DepthTest)]
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 2 //"Back"
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Float) = 0 //"Off"
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4 //"LessEqual"
        [Enum(UnityEngine.Rendering.BlendOp)] _BlendOpColor("Blend Color", Float) = 2 //"ReverseSubtract"
        [Enum(UnityEngine.Rendering.BlendOp)] _BlendOpAlpha("Blend Alpha", Float) = 3 //"Min"
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" }
            LOD 100

            Pass
            {
                Cull[_Cull]
                ZWrite[_ZWrite]
                ZTest[_ZTest]
                BlendOp[_BlendOpColor],[_BlendOpAlpha]
                Blend Zero One, One One

                CGPROGRAM
            // Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members center)
            //#pragma exclude_renderers d3d11
                        #pragma vertex vert
                        #pragma fragment frag

                        #include "UnityCG.cginc"

                        struct appdata
                        {
                            float4 vertex : POSITION;
                            float2 uv : TEXCOORD0;
                            float3 normal : NORMAL;
                        };

                        struct v2f
                        {
                            float2 uv : TEXCOORD0;
                            float4 vertex : SV_POSITION;
                        };

                        sampler2D _MainTex;
                        float4 _MainTex_ST;
                        float _InvertedAlpha;

                        v2f vert(appdata v)
                        {
                            v2f o;
                            o.vertex = UnityObjectToClipPos(v.vertex);
                            float4 origin = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));
                            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                            return o;
                        }

                        fixed4 frag(v2f i) : SV_Target {
                            fixed4 col = tex2D(_MainTex, i.uv);
                            float alpha = lerp(col.r, 1 - col.r, _InvertedAlpha);
                            return float4(0, 0, 0, alpha);
                        }
                        ENDCG
                    }
        }
}