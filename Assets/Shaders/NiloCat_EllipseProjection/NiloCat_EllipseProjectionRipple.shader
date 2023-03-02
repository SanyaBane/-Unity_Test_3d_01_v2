//see README here: 
//github.com/ColinLeung-NiloCat/UnityURPUnlitScreenSpaceDecalShader

Shader "NiloCat/EllipseProjectionRipple"
{
    Properties
    {
        [Header(Basic)]
        _MainTex("Texture", 2D) = "white" {}
        [HDR] _Color("_Color (default = 1,1,1,1)", color) = (1,1,1,1)

        [Header(Blending)]
        //https://docs.unity3d.com/ScriptReference/Rendering.BlendMode.html
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("_SrcBlend (default = SrcAlpha)", Float) = 5 //5 = SrcAlpha
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("_DstBlend (default = OneMinusSrcAlpha)", Float) = 10 //10 = OneMinusSrcAlpha

        [Header(Alpha remap(extra alpha control))]
        _AlphaRemap("_AlphaRemap (default = 1,0,0,0) _____alpha will first mul x, then add y    (zw unused)", vector) = (1,0,0,0)

        [Header(Prevent Side Stretching(Compare projection direction with scene normal and Discard if needed))]
        [Toggle(_ProjectionAngleDiscardEnable)] _ProjectionAngleDiscardEnable("_ProjectionAngleDiscardEnable (default = off)", float) = 0
        _ProjectionAngleDiscardThreshold("_ProjectionAngleDiscardThreshold (default = 0)", range(-1,1)) = 0

        [Header(Mul alpha to rgb)]
        [Toggle]_MulAlphaToRGB("_MulAlphaToRGB (default = off)", Float) = 0

        [Header(Ignore texture wrap mode setting)]
        [Toggle(_FracUVEnable)] _FracUVEnable("_FracUVEnable (default = off)", Float) = 0

        //====================================== below = usually can ignore in normal use case =====================================================================
        [Header(Stencil Masking)]
        //https://docs.unity3d.com/ScriptReference/Rendering.CompareFunction.html
        _StencilRef("_StencilRef", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComp("_StencilComp (default = Disable) _____Set to NotEqual if you want to mask by specific _StencilRef value, else set to Disable", Float) = 0 //0 = disable
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilPassOp ("Stencil Pass Op", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilFailOp ("Stencil Fail Op", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilZFailOp ("Stencil ZFail Op", Float) = 0

        [Header(ZTest)]
        //https://docs.unity3d.com/ScriptReference/Rendering.CompareFunction.html
        //default need to be Disable, because we need to make sure decal render correctly even if camera goes into decal cube volume, although disable ZTest by default will prevent EarlyZ (bad for GPU performance)
        [Enum(UnityEngine.Rendering.CompareFunction)]_ZTest("_ZTest (default = Disable) _____to improve GPU performance, Set to LessEqual if camera never goes into cube volume, else set to Disable", Float) = 0 //0 = disable

        [Header(Cull)]
        //https://docs.unity3d.com/ScriptReference/Rendering.CullMode.html
        //default need to be Front, because we need to make sure decal render correctly even if camera goes into decal cube
        [Enum(UnityEngine.Rendering.CullMode)]_Cull("_Cull (default = Front) _____to improve GPU performance, Set to Back if camera never goes into cube volume, else set to Front", Float) = 1 //1 = Front

        [Header(Unity Fog)]
        [Toggle(_UnityFogEnable)] _UnityFogEnable("_UnityFogEnable (default = on)", Float) = 1
        
        [PerRendererData] _AmountDegrees("AmountDegrees", Range (0, 360)) = 90
        [PerRendererData] _RippleDistance("RippleDistance", Float) = 0.5
        [PerRendererData] _RippleWidth("RippleWidth", Range(0, 0.1)) = 0.02
        [PerRendererData] _RippleStrength("RippleStrength", Range(0, 1)) = 1
    }

    SubShader
    {
        //To avoid render order problems, Queue must >= 2501, which enters the transparent queue, in transparent queue Unity will always draw from back to front
        //https://github.com/ColinLeung-NiloCat/UnityURPUnlitScreenSpaceDecalShader/issues/6#issuecomment-615940985
        /*
        //https://docs.unity3d.com/Manual/SL-SubShaderTags.html
        Queues up to 2500 (“Geometry+500”) are consided “opaque” and optimize the drawing order of the objects for best performance. 
        Higher rendering queues are considered for “transparent objects” and sort objects by distance, starting rendering from the furthest ones and ending with the closest ones. 
        Skyboxes are drawn in between all opaque and all transparent objects.
        */
        Tags { "RenderType" = "Overlay" "Queue" = "Transparent-495" "DisableBatching" = "True" }

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

            HLSLPROGRAM


            #pragma vertex vert
            #pragma fragment frag

            // make fog work
            #pragma multi_compile_fog

            //due to using ddx() & ddy()
            #pragma target 3.0

            #pragma shader_feature_local _ProjectionAngleDiscardEnable
            #pragma shader_feature_local _UnityFogEnable
            #pragma shader_feature_local _FracUVEnable

            // #define TAU 6.28318530718
            
            // Required by all Universal Render Pipeline shaders.
            // It will include Unity built-in shader variables (except the lighting variables)
            // (https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
            // It will also include many utilitary functions. 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // #include "UnityCG.cginc"
            
            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float4 screenUV : TEXCOORD0;
                float4 viewRayOS : TEXCOORD1;
                float4 cameraPosOSAndFogFactor : TEXCOORD2;
                float2 uv : TEXCOORD3;
                float3 worldPos : TEXCOORD4;
            };

            sampler2D _MainTex;
            float _AmountDegrees;
            float _RippleDistance;
            float _RippleWidth;
            float _RippleStrength;

            static const float _DegreesCondition = 180;
            
            CBUFFER_START(UnityPerMaterial)               
                float4 _MainTex_ST;
                float _ProjectionAngleDiscardThreshold;
                half4 _Color;
                half2 _AlphaRemap;
                half _MulAlphaToRGB;
            CBUFFER_END

            sampler2D _CameraDepthTexture;

            float4 ObjectToClipPos (float3 pos)
            {
                return mul (UNITY_MATRIX_VP, mul (UNITY_MATRIX_M, float4 (pos,1)));
            }

            Interpolators vert(MeshData v)
            {
                Interpolators o;

                //regular MVP
                // o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.vertex = TransformObjectToHClip(v.vertex);

                //regular unity fog
#if _UnityFogEnable
                o.cameraPosOSAndFogFactor.a = ComputeFogFactor(o.vertex.z);
#else
                o.cameraPosOSAndFogFactor.a = 0;
#endif

                //prepare depth texture's screen space UV
                o.screenUV = ComputeScreenPos(o.vertex);

                //get "camera to vertex" ray in View space
                float3 viewRay = TransformWorldToView(TransformObjectToWorld(v.vertex.xyz));

                //***WARNING***
                //=========================================================
                //"viewRay z division" must do in the fragment shader, not vertex shader! (due to rasteriazation varying interpolation's perspective correction)
                //We skip the "viewRay z division" in vertex shader for now, and pass the division value to varying o.viewRayOS.w first, we will do the division later when we enter fragment shader
                //viewRay /= viewRay.z; //skip the "viewRay z division" in vertex shader for now
                o.viewRayOS.w = viewRay.z;//pass the division value to varying o.viewRayOS.w
                //=========================================================

                viewRay *= -1; //unity's camera space is right hand coord(negativeZ pointing into screen), we want positive z ray in fragment shader, so negate it

                //it is ok to write very expensive code in decal's vertex shader, it is just a unity cube(4*6 vertices) per decal only, won't affect GPU performance at all.
                float4x4 ViewToObjectMatrix = mul(unity_WorldToObject, UNITY_MATRIX_I_V);

                //transform everything to object space(decal space) in vertex shader first, so we can skip all matrix mul() in fragment shader
                o.viewRayOS.xyz = mul((float3x3)ViewToObjectMatrix, viewRay);
                o.cameraPosOSAndFogFactor.xyz = mul(ViewToObjectMatrix, float4(0,0,0,1)).xyz;
                
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.worldPos = mul (unity_ObjectToWorld, v.vertex);
                
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
                rMatrix = rMatrix*2 - 1;
            
                //multiply the UVs by the rotation matrix
                UV.xy = mul(UV.xy, rMatrix);
                UV += Center;
            
                Out = UV;
            }

            half4 frag(Interpolators i) : SV_Target
            {
                //***WARNING***
                //=========================================================
                //now do "viewRay z division" that we skipped in vertex shader earlier.
                i.viewRayOS /= i.viewRayOS.w;
                //=========================================================

                float sceneCameraSpaceDepth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, i.screenUV).r, _ZBufferParams);

                //scene depth in any space = rayStartPos + rayDir * rayLength
                //here all data in ObjectSpace(OS) or DecalSpace
                float3 decalSpaceScenePos = i.cameraPosOSAndFogFactor.xyz + i.viewRayOS.xyz * sceneCameraSpaceDepth;

                //convert unity cube's [-0.5,0.5] vertex pos range to [0,1] uv. Only works if you use unity cube in mesh filter!
                float2 decalSpaceUV = decalSpaceScenePos.xy + 0.5;

                //discard logic
                //===================================================
                // discard "out of cube volume" pixels
                //2020-4-17: tried fix clip() bug by removing all possible bool
                //https://github.com/ColinLeung-NiloCat/UnityURPUnlitScreenSpaceDecalShader/issues/6#issuecomment-614633460            
                float mask = (abs(decalSpaceScenePos.x) < 0.5 ? 1.0 : 0.0) * (abs(decalSpaceScenePos.y) < 0.5 ? 1.0 : 0.0) * (abs(decalSpaceScenePos.z) < 0.5 ? 1.0 : 0.0);

#if _ProjectionAngleDiscardEnable
                // also discard "scene normal not facing decal projector direction" pixels
                float3 decalSpaceHardNormal = normalize(cross(ddx(decalSpaceScenePos), ddy(decalSpaceScenePos)));//reconstruct scene hard normal using scene pos ddx&ddy

                mask *= decalSpaceHardNormal.z > _ProjectionAngleDiscardThreshold ? 1.0 : 0.0;//compare scene hard normal with decal projector's dir, decalSpaceHardNormal.z equals dot(decalForwardDir,sceneHardNormalDir)
#endif
                //call discard
                clip(mask - 0.5);//if ZWrite is off, clip() is fast enough on mobile, because it won't write the DepthBuffer, so no pipeline stall(confirmed by ARM staff).
                //===================================================

                // sample the decal texture
                float2 uv = decalSpaceUV.xy;// * _MainTex_ST.xy + _MainTex_ST.zw;//Texture tiling & offset
#if _FracUVEnable
                uv = frac(uv);//add frac to ignore texture wrap setting
#endif

                // half4 col = tex2D(_MainTex, uv);
                // col *= _Color;//tint color

                float el_dist = length(uv * 2 - 1) - 1;
                float el_step = step(0, el_dist);
                half4 ellipse = half4(el_step, el_step, el_step, 1);
                ellipse = abs(1 - ellipse);

                float2 uv_offsetted = uv * 2 - 1;
                
                float amountRadians = DegToRad(_AmountDegrees) / 2;
                
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
                
                half3 mult_03 = ellipse.rgb * clamp_03;

                float2 uvsCentered = uv * 2 - 1;
                float radialDistance = length(uvsCentered);
                float halfWidth = _RippleWidth / 2;
                
                // float worldDist = length(decalSpaceScenePos);
                float ring = clamp(_RippleStrength - abs((radialDistance - _RippleDistance) / (halfWidth)), 0, 1);

                ring = clamp(ring, 0, 1);
                // ring = ring *  _Color;
                
                half4 col;
                col.rgb = mult_03 + ring;
				col.rgb *=  _Color; // add color at the very end

                col.a = mult_03 * _Color.a;

                // return float4(ring, 0, 0, 1);
                //return float4(mult_03, 1);
                return col;
            }

            
            ENDHLSL
        }
    }
}
