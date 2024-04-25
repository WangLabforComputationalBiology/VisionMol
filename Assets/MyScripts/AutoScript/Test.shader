Shader "Test"
{
     Properties
	{
		_MainTex("MainTex", 2D) = "white" {}//纹理贴图
		_Diffuse("Diffuse", Color) = (1,1,1,1)//漫反射颜色
		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.5//设置的控制透明部分的系数
	}
 
	SubShader
	{
		Tags { 
			"Queue"="AlphaTest" //在不透明物体之后、透明物体之前渲染该物体；
			"IgnoreProjector"="True" //不产生阴影；
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
				fixed3 worldNormal: TEXCOORD0;//世界空间法线
				float3 worldPos: TEXCOORD1;//世界空间顶点坐标
				float2 uv : TEXCOORD2;//uv坐标
			};
 
			v2f vert (appdata_base v)//appdata_base是自带的一个结构体，含有vertex、normal等的定义，这样就不用自己再定义一个类似的结构体了。
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
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;//环境光
				fixed4 texColor = tex2D(_MainTex, i.uv);//读取纹理贴图颜色值
 
				if((texColor.a - _Cutoff)<0)//满足条件时不显示该片元，这里的条件为纹理贴图的α通道的值小于_Cutoff
				{
					discard;//剪除、不显示该片元
				}
 
				//漫反射
				fixed3 worldLightDir = UnityWorldSpaceLightDir(i.worldPos);
				fixed3 diffuse = _LightColor0.rgb * texColor.rgb * _Diffuse.rgb * (dot(worldLightDir,i.worldNormal)*0.5+0.5);
 
				fixed3 color = ambient + diffuse;
				return fixed4(color,1);
			}
			ENDCG
		}
	}
	FallBack "VertexLit"//虽然之前的标签了设置了不产生阴影，但FallBack的方案也会产生阴影
}