Shader "Custom/depthBlit"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "black" {}
    }
    SubShader
    {
        Pass
        {

            ZWrite On
            ZTest Always

            HLSLPROGRAM
            #pragma target 3.5
            
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _CameraDepthTexture;
            //sampler2D _MainTex;

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 baseUV : TEXCOORD0;
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 baseUV : VAR_BASE_UV;
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings) 0;
                output.positionCS = float4(input.positionOS.xy * 2.0 - 1.0, 0.0, 1.0);
                output.baseUV = input.baseUV;
                return output;
            }

            float frag(Varyings i) : SV_Depth
            {
                float4 color = tex2D(_CameraDepthTexture, i.baseUV);
                return color.r;
            }
            ENDHLSL


        }
    }
    FallBack "Diffuse"
}
