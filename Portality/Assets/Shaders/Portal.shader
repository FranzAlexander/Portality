
Shader "Unlit/Portal"
{
    Properties
    {
        [MainTexture] _MainTex ("_MainTex", 2D) = "White"
    }
    SubShader
    {
        Tags 
        {           
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
        }
        
        pass
        {
            Name "PortalPass"

            Cull Back
            ZTest Less
            ZWrite On

            HLSLPROGRAM

            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float4  uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 position : SV_POSITION; 
                float4 uv : TEXCOORD0;

                UNITY_VERTEX_OUTPUT_STEREO
            };

      
            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);

                // #if defined(UNITY_COMPILER_HLSL)
                // #define UNITY_INITIALIZE_OUTPUT(type,name) name = (type)0;
                // #else
                // #define UNITY_INITIALIZE_OUTPUT(type,name)
                // #endif

                // UNITY_INITIALIZE_OUTPUT(Varyings, output);

                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.position = TransformObjectToHClip(input.vertex.xyz);
                output.uv = ComputeScreenPos(output.position);

                // #if UNITY_UV_STARTS_AT_TOP
                // output.position.y*= -1;
                // #endif

                return output;
            }

      
           TEXTURE2D_X(_MainTex);
           SAMPLER(sampler_MainTex);


            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.uv.xy / input.uv.w;

                float4 portalPosition = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, uv); 

                return portalPosition;
            }

            ENDHLSL
        } 
    }
}