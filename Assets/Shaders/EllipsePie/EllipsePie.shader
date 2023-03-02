Shader "Unlit/EllipsePie"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        [PerRendererData] [HDR] _Color("_Color (default = 1,1,1,1)", color) = (1,1,1,1)

        [Header(Blending)]
        //https://docs.unity3d.com/ScriptReference/Rendering.BlendMode.html
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("_SrcBlend (default = SrcAlpha)", Float) = 5 //5 = SrcAlpha
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("_DstBlend (default = OneMinusSrcAlpha)", Float) = 10 //10 = OneMinusSrcAlpha

        [Header(Stencil Masking)]
        //https://docs.unity3d.com/ScriptReference/Rendering.CompareFunction.html
        _StencilRef("_StencilRef", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComp("_StencilComp (default = Disable)", Float) = 0 //0 = disable
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilPassOp ("Stencil Pass Op", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilFailOp ("Stencil Fail Op", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilZFailOp ("Stencil ZFail Op", Float) = 0

        [Header(ZTest)]
        //https://docs.unity3d.com/ScriptReference/Rendering.CompareFunction.html
        [Enum(UnityEngine.Rendering.CompareFunction)]_ZTest("_ZTest (default = Less)", Float) = 2 //2 = Less

        [Header(Cull)]
        //https://docs.unity3d.com/ScriptReference/Rendering.CullMode.html
        [Enum(UnityEngine.Rendering.CullMode)]_Cull("_Cull (default = Off)", Float) = 0 //0 = Off


        [PerRendererData] _AmountDegrees("AmountDegrees", Range (0, 360)) = 90
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "Queue" = "Transparent-496"
        }

        Pass
        {
            Stencil
            {
                Ref [_StencilRef]
                Comp [_StencilComp]
                Pass [_StencilPassOp]
                Fail [_StencilFailOp]
                ZFail [_StencilZFailOp]
            }

            Cull[_Cull]
            ZTest[_ZTest]

            ZWrite off
            Blend[_SrcBlend][_DstBlend]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            half4 _Color;

            float _AmountDegrees;

            static const float _DegreesCondition = 180;
            static const float DegToRad = (UNITY_PI * 2.0) / 360.0;

            Interpolators vert(MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            void Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation, out float2 Out)
            {
                //rotation matrix
                UV -= Center;
                float s = sin(Rotation);
                float c = cos(Rotation);

                //center rotation matrix
                float2x2 rMatrix = float2x2(c, -s, s, c);
                rMatrix *= 0.5;
                rMatrix += 0.5;
                rMatrix = rMatrix * 2 - 1;

                //multiply the UVs by the rotation matrix
                UV.xy = mul(UV.xy, rMatrix);
                UV += Center;

                Out = UV;
            }

            fixed4 frag(Interpolators i) : SV_Target
            {
                // return float4(i.uv, 0, 1);

                float2 uv = i.uv;

                float el_dist = length(uv * 2 - 1) - 1;
                float el_step = step(0, el_dist);
                half4 ellipse = half4(el_step, el_step, el_step, 1);
                ellipse = abs(1 - ellipse);

                float2 uv_offsetted = uv * 2 - 1;
                // float amountRadians = DegToRad(_AmountDegrees) / 2;
                float amountRadians = _AmountDegrees * DegToRad / 2;

                float2 uv_01;
                Unity_Rotate_Radians_float(uv_offsetted, float2(0, 0), amountRadians, uv_01);
                half atan2_01 = atan2(uv_01.r, 0);
                half clamp_01 = clamp(atan2_01, 0, 1);

                if (_AmountDegrees < _DegreesCondition)
                {
                    clamp_01 = abs(1 - clamp_01); // invert colors
                }

                float2 uv_02;
                Unity_Rotate_Radians_float(uv_offsetted, float2(0, 0), amountRadians * -1 + 3.14159, uv_02);
                half atan2_02 = atan2(uv_02.r, 0);
                half clamp_02 = clamp(atan2_02, 0, 1);

                if (_AmountDegrees < _DegreesCondition)
                {
                    clamp_02 = abs(1 - clamp_02); // invert colors
                }

                half add_03 = clamp_01 + clamp_02;
                half clamp_03 = clamp(add_03, 0, 1);

                if (_AmountDegrees < _DegreesCondition)
                {
                    clamp_03 = abs(1 - clamp_03); // invert colors
                }

                half4 mult_03 = ellipse * clamp_03;

                float4 mainTex2D = tex2D(_MainTex, i.uv);
                mainTex2D *= _Color;

                fixed4 col;
                col.rgb = mainTex2D;
                col.a = mult_03 * mainTex2D.a;
                // col.a = 1;

                return col;
            }
            ENDCG
        }
    }
}