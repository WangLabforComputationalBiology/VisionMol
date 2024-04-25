Shader "Test"
{
     Properties
	{
		_MainTex("MainTex", 2D) = "white" {}//������ͼ
		_Diffuse("Diffuse", Color) = (1,1,1,1)//��������ɫ
		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.5//���õĿ���͸�����ֵ�ϵ��
	}
 
	SubShader
	{
		Tags { 
			"Queue"="AlphaTest" //�ڲ�͸������֮��͸������֮ǰ��Ⱦ�����壻
			"IgnoreProjector"="True" //��������Ӱ��
			}
		LOD 100
 
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
 
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Diffuse;
			float _Cutoff;
 
			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed3 worldNormal: TEXCOORD0;//����ռ䷨��
				float3 worldPos: TEXCOORD1;//����ռ䶥������
				float2 uv : TEXCOORD2;//uv����
			};
 
			v2f vert (appdata_base v)//appdata_base���Դ���һ���ṹ�壬����vertex��normal�ȵĶ��壬�����Ͳ����Լ��ٶ���һ�����ƵĽṹ���ˡ�
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal);
				o.worldNormal = worldNormal;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;//������
				fixed4 texColor = tex2D(_MainTex, i.uv);//��ȡ������ͼ��ɫֵ
 
				if((texColor.a - _Cutoff)<0)//��������ʱ����ʾ��ƬԪ�����������Ϊ������ͼ�Ħ�ͨ����ֵС��_Cutoff
				{
					discard;//����������ʾ��ƬԪ
				}
 
				//������
				fixed3 worldLightDir = UnityWorldSpaceLightDir(i.worldPos);
				fixed3 diffuse = _LightColor0.rgb * texColor.rgb * _Diffuse.rgb * (dot(worldLightDir,i.worldNormal)*0.5+0.5);
 
				fixed3 color = ambient + diffuse;
				return fixed4(color,1);
			}
			ENDCG
		}
	}
	FallBack "VertexLit"//��Ȼ֮ǰ�ı�ǩ�������˲�������Ӱ����FallBack�ķ���Ҳ�������Ӱ
}