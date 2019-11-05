// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Based upon the example at: http://www.battlemaze.com/?p=153
// Also: https://developer.nvidia.com/gpugems/GPUGems/gpugems_ch07.html

Shader "Custom/Grass Geo Shader" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_LightDirYVal("Light Direction Y Value", Float) = 0
		_SpecColor("Specular Material Color", Color) = (1,1,1,1)
		_Shininess("Shininess", Float) = 1.0
		_Cutoff("Cutoff", Range(0,1)) = 0.25
		_GrassHeight("Grass Height", Float) = 0.25
		_GrassTopWidth("Grass Width at Top", Float) = 0.25
		_GrassBotWidth("Grass Width at Bottom", Float) = 0.25
		_WindSpeed("Wind Speed", Float) = 100
		_WindStength("Wind Strength", Float) = 0.05
	}
		SubShader{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout"}
		LOD 200

		Pass
	{
			CULL OFF

			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdbase
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _SpecMap;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			uniform float _LightDirYVal;
			uniform float4 _LightColor0;
			uniform float4 _SpecMap_ST;
			half _GrassHeight;
			half _GrassTopWidth;
			half _GrassBotWidth;
			uniform half _Cutoff;
			half _WindStength;
			half _WindSpeed;

			struct v2g
			{
			float4 pos : SV_POSITION;
			float3 normal : NORMAL;
			float2 uv : TEXCOORD0;
			float3 color : TEXCOORD1;
			};

			struct g2f
			{
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float3 specularColor : TEXCOORD2;
			};

			v2g vert(appdata_full v)
			{
			float3 v0 = v.vertex.xyz;

			v2g OUT;
			OUT.pos = v.vertex;
			OUT.normal = v.normal;
			OUT.uv = v.texcoord;
			OUT.color = tex2Dlod(_MainTex, v.texcoord).rgb;
			return OUT;
			}

			[maxvertexcount(24)]
			void geom(point v2g IN[1], inout TriangleStream<g2f> triStream)
			{

				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

			float3 perpendicularAngle = float3(0, 0, 1);
			float3 faceNormal = cross(perpendicularAngle, IN[0].normal);
			float3 normalDirection;
			normalDirection = normalize(mul(float4(IN[0].normal, 0.0), modelMatrixInverse).xyz);

			float3 v0 = IN[0].pos.xyz;
			float3 v1 = IN[0].pos.xyz + IN[0].normal * _GrassHeight;

			float3 centerPos = (v0 + v1) / 2.0;

			float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
			float3 viewDirection = normalize(_WorldSpaceCameraPos - mul(modelMatrix, float4(centerPos, 0.0)).xyz);
			float attenuation = 1.0;

			float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * IN[0].color.rgb;
			float3 specularReflection;

			float3 wind = float3(sin(_Time.x * _WindSpeed + v0.x) + sin(_Time.x * _WindSpeed + v0.z * 2) + sin(_Time.x * _WindSpeed * 0.1 + v0.x), 0,
			cos(_Time.x * _WindSpeed + v0.x * 2) + cos(_Time.x * _WindSpeed + v0.z));
			v1 += wind * _WindStength;

			float3 color = (IN[0].color);

			float sin30 = 0.5;
			float sin60 = 0.866f;
			float cos30 = sin60;
			float cos60 = sin30;

			g2f OUT;

			// QUAD1
			float4 q1_0 = UnityObjectToClipPos(v0 + perpendicularAngle * _GrassBotWidth);


			float3 q1 = normalize(cross(v1 - v0, q1_0 - v0));
			normalDirection = normalize(mul(float4(q1, 0.0), modelMatrixInverse).xyz);

			float multiplier;
			if (_WorldSpaceLightPos0.w == 0)
			{
				multiplier = 1;
			}
			else
			{
				multiplier = 0;
			}

				specularReflection = multiplier * attenuation * _LightColor0.rgb * _SpecColor.rgb * pow(max(0.0, max(dot(
					reflect(viewDirection, -normalDirection), reflect(viewDirection, normalDirection)),
					viewDirection)), _Shininess);

			OUT.pos = q1_0;
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(1, 0);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v1 + perpendicularAngle * _GrassTopWidth);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(1, 1);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v0);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(0.5, 0);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v1);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(0.5, 1);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v1 - perpendicularAngle * _GrassTopWidth);
			OUT.specularColor = specularReflection;
			OUT.uv = float2(0, 1);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v0 - perpendicularAngle *_GrassBotWidth);
			OUT.specularColor = specularReflection;
			OUT.uv = float2(0, 0);
			triStream.Append(OUT);


			OUT.pos = UnityObjectToClipPos(v0);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(0.5, 0);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v1);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(0.5, 1);
			triStream.Append(OUT);

			// Quad 2

			float4 q2_0 = UnityObjectToClipPos(v0 + float3(sin60, 0, -cos60) * _GrassBotWidth);

			float3 q2 = normalize(cross(v1 - v0, q2_0 - v0));
			normalDirection = normalize(mul(float4(q2, 0.0), modelMatrixInverse).xyz);


			specularReflection = multiplier * attenuation * _LightColor0.rgb * _SpecColor.rgb * pow(max(0.0, max(dot(
				reflect(viewDirection, -normalDirection), reflect(viewDirection, normalDirection)),
				viewDirection)), _Shininess);



			OUT.pos = q2_0;
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(1, 0);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v1 + float3(sin60, 0, -cos60)* _GrassTopWidth);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(1, 1);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v0);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(0.5, 0);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v1);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(0.5, 1);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v0 - float3(sin60, 0, -cos60) * _GrassBotWidth);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(0, 0);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v1 - float3(sin60, 0, -cos60) * _GrassTopWidth);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(0, 1);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v0);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(0.5, 0);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v1);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(0.5, 1);
			triStream.Append(OUT);

			// Quad 3 - Positive


			float4 q3_0 = UnityObjectToClipPos(v0 + float3(sin60, 0, cos60) * _GrassBotWidth);

			float3 q3 = normalize(cross(v1 - v0, q3_0 - v0));
			normalDirection = normalize(mul(float4(q3, 0.0), modelMatrixInverse).xyz);


			specularReflection = multiplier * attenuation * _LightColor0.rgb * _SpecColor.rgb * pow(max(0.0, max(dot(
				reflect(viewDirection, -normalDirection), reflect(viewDirection, normalDirection)),
				viewDirection)), _Shininess);


			OUT.pos = q3_0;
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(1, 0);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v1 + float3(sin60, 0, cos60)* _GrassTopWidth);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(1, 1);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v0);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(0.5, 0);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v1);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(0.5, 1);
			triStream.Append(OUT);

			// Quad 3 - NEgative

			OUT.pos = UnityObjectToClipPos(v0 - float3(sin60, 0, cos60) * _GrassBotWidth);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(0, 0);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v1 - float3(sin60, 0, cos60) * _GrassTopWidth);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(0, 1);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v0);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(0.5, 0);
			triStream.Append(OUT);

			OUT.pos = UnityObjectToClipPos(v1);
			OUT.specularColor = ambientLighting + specularReflection;
			OUT.uv = float2(0.5, 1);
			triStream.Append(OUT);
			}

			half4 frag(g2f IN) : COLOR
			{
				fixed4 c = tex2D(_MainTex, IN.uv);
				c.rgb *= IN.specularColor;
				clip(c.a - _Cutoff);
				return c;
			}
			ENDCG
		}
	}
}