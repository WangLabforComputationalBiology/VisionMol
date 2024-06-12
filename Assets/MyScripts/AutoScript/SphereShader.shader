Shader "Custom/SphereShader"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1, 1, 1, 1)
        _Glossiness ("Glossiness", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        CGPROGRAM
        #pragma surface surf Lambert
        
        sampler2D _MainTex;
        fixed4 _MainColor;
        half _Glossiness;
        
        struct Input
        {
            float2 uv_MainTex;
            float3 worldNormal;
        };
        
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 mainColor = tex2D(_MainTex, IN.uv_MainTex) * _MainColor;
            
            o.Albedo = mainColor.rgb;
            o.Alpha = mainColor.a;
            o.Gloss = _Glossiness;
        }
        
        // 顶点着色器部分
        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.worldNormal = UnityObjectToWorldNormal(v.normal);
        }
        
        ENDCG
    }
    
    FallBack "Diffuse"
}