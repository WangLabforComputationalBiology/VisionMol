Shader"Custom/depthBlitMul"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "black" {}
        //_DepthInverse("useDepthInverse", bool) = false
        //depthTexture("Tex", 2DArray) = "" {}
    }
    SubShader
    {
        Pass
        {
            ColorMask R
            ZWrite On
            ZTest Always

            HLSLPROGRAM
            #pragma target 3.5
            #pragma require 2darray
            
            #pragma vertex vert
            #pragma fragment frag

            //sampler2D _CameraDepthTexture;
            Texture2DArray _CameraDepthTexture;
            SamplerState sampler_CameraDepthTexture;
            //SAMPLE_TEXTURE2D_ARRAY(_CameraDepthTexture);
            //sampler2D _MainTex;
            int _DepthInverse;
            int _DepthArryIndex;

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

            struct FragOutput
            {
                float depth : SV_Depth;
                float4 color : SV_Target;
};

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings) 0;
                output.positionCS = float4(input.positionOS.xy * 2.0 - 1.0, 0.0, 1.0);
                output.baseUV = input.baseUV;
                return output;
            }

            FragOutput frag(Varyings i)
            {
                FragOutput output = (FragOutput) 0;
                //output.color = tex2D(_CameraDepthTexture, i.baseUV);
    output.color = _CameraDepthTexture.Sample(sampler_CameraDepthTexture, float3(i.baseUV, _DepthArryIndex));
                output.depth = output.color.r;
                if (_DepthInverse > 0)
                {
                    output.color.r = 1.0f - output.color.r;
                    output.depth = output.color.r;
                }
                return output;
            }
            ENDHLSL


        }
    }
    FallBack "Diffuse"
}
