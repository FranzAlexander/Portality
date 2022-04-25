Shader "Portal/URPUnlitPortalCutout"
{
    Properties 
    {
        _MainTex("MainTex", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        LOD 100
        Cull Back
        ZWrite On
        ZTest Less

        Pass
        {
            HLSLPROGRAM
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma vertex vert
            #pragma fragment frag

    	  
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO 
            };

            // #if SRC_TEXTURE2D_X_ARRAY
            // TEXTURE2D_ARRAY(_MainTex);
            // #else
            // TEXTURE2D(_MainTex);
            // #endif
            TEXTURE2D_X(_MainTex);
            float4 _InactiveColour;
            SAMPLER(sampler_MainTex);

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                ZERO_INITIALIZE(Varyings, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.positionCS =TransformWorldToHClip(IN.positionOS.xyz);
                OUT.uv =  ComputeScreenPos(OUT.positionCS);
                // VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                // OUT.positionCS = positionInputs.positionCS;
                // OUT.uv = positionInputs.positionNDC;

                return OUT;
            }

            float4 frag(Varyings IN) : SV_TARGET 
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
                float2 uv = IN.uv.xy / IN.uv.w;

                #if UNITY_STEREO_INSTANCING_ENABLED || UNITY_STEREO_MULTIVIEW_ENABLED
                float4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];
                uv = (uv - scaleOffset.zw) / scaleOffset.xy;
                #endif
                float4 portalCol = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, uv);
                
                // #if SRC_TEXTURE2D_X_ARRAY
                // portalCol = SAMPLE_TEXTURE2D_ARRAY(_MainTex, sampler_MainTex, uv, 1);
                // #else
                // portalCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                // #endif

                return portalCol;
            }

            ENDHLSL
        }
    }
}
// Shader "Portal/URPUnlitPortalCutout"
// {
//     Properties
//     {
//         _MainTex ("Main Texture", 2D) = "white" {}
//     }
//     SubShader
//     {
//         Tags
//         {
//             "RenderPipeline"="UniversalPipeline"
//             "RenderType"="Transparent"
//             "UniversalMaterialType" = "Unlit"
//             "Queue"="Transparent"
//         }
//         Pass
//         {
//             Name "Universal Forward"

//             Cull Back

//             HLSLPROGRAM

//             // Pragmas
//             #pragma target 4.5
//             #pragma exclude_renderers gles gles3 glcore
//             #pragma multi_compile_instancing
//             #pragma multi_compile_fog
//             #pragma instancing_options renderinglayer
//             #pragma multi_compile _ DOTS_INSTANCING_ON
//             #pragma vertex vert
//             #pragma fragment frag

//             // Includes
//             #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
//             #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
//             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
//             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
//             #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

//             struct Attributes
//             {
//                 float4 vertex : POSITION;

//                 #if UNITY_ANY_INSTANCING_ENABLED
//                 uint instanceID : INSTANCEID_SEMANTIC
//                 #endif
//             };

//             struct Varyings
//             {
//                 float4 vertex : SV_POSITION;
//                 float4 screenPosition : TEXCOORD0;

//                 #if UNITY_ANY_INSTANCING_ENABLED
//                 uint instanceID : INSTANCEID_SEMANTIC
//                 #endif

//                 #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
//                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
//                 #endif
//                 #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
//                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
//                 #endif
//                 #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
//                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
//                 #endif
//             };

//             Varyings vert(Attributes input)
//             {
//                 Varyings output;
//                 ZERO_INITIALIZE(Varyings, output);
//                 #if UNITY_ANY_INSTANCING_ENABLED
//                 output.instanceID = input.instanceID;
//                 #endif
//                 #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
//                 output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
//                 #endif
//                 #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
//                 output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
//                 #endif
//                 #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
//                 output.cullFace = input.cullFace;
//                 #endif

//                 return output;
//             }

//             float4 frag(Varyings input) 
//             {

//             }

//             ENDHLSL
//         }
//     }
// }