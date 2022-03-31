Shader "Unlit/Portal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        Lighting Off
        Cull Back
        ZWrite On
        ZTest Less

        Fog{Mode Off}

    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"

        struct appdata
        {
            float4 vertex : POSITION;
            float4 uv : TEXCOORD0;
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            float4 screenPos : TEXCOORD1;
        };


        v2f vert(appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.screenPos = ComputeScreenPos(o.vertex);
            return o;
        }
        sampler2D _MainTex;


        fixed4 frag(v2f i) : SV_Target
        {
            float2 uv = i.screenPos.xy / i.screenPos.w;
            float4 portalCol = tex2D(_MainTex, uv);
            return portalCol;
        }

        ENDCG
    }    
    }
}