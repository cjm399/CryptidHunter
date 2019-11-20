// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

// Based upon the example at: http://www.battlemaze.com/?p=153
// Also: https://developer.nvidia.com/gpugems/GPUGems/gpugems_ch07.html

Shader "Custom/Grass Geo Shader" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_SpecularColor("Specular Material Color", Color) = (1,1,1,1)
		_Shininess("Shininess", Float) = 1.0
		_Cutoff("Cutoff", Range(0,1)) = 0.25
		_GrassHeight("Grass Height", Float) = 0.25
		_GrassTopWidth("Grass Width at Top", Float) = 0.25
		_GrassBotWidth("Grass Width at Bottom", Float) = 0.25
		_WindSpeed("Wind Speed", Float) = 100
		_WindStength("Wind Strength", Float) = 0.05
	}
		SubShader{
		Tags{ "Shadows" = "Off" "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout"}
		LOD 200

		Pass
		{
			Tags{"LightMode" = "ForwardBase"}
			//CULL OFF

			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			#pragma multi_compile DIRECTIONAL POINT SPOT
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			uniform float4 _SpecularColor;
			uniform float _Shininess;
			uniform float3 _LightColor0;
			half _GrassHeight;
			half _GrassTopWidth;
			half _GrassBotWidth;
			uniform half _Cutoff;
			half _WindStength;
			half _WindSpeed;
			float _FlashLightRange;

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

			float GetSpecularReflection(float multiplier, float attenuation, float viewDirection, float normalDirection)
			{
				return multiplier * attenuation * _LightColor0.rgb * _SpecularColor.rgb * _Shininess;
			}

			v2g vert(appdata_full v)
			{

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

			float3 perpendicularAngle = float3(0, 0, 1);
			float3 faceNormal = cross(perpendicularAngle, IN[0].normal);
			float3 normalDirection;
			normalDirection = normalize(mul(float4(IN[0].normal, 0.0), unity_WorldToObject).xyz);

			float3 v0 = IN[0].pos.xyz;
			float3 v1 = IN[0].pos.xyz + IN[0].normal * _GrassHeight;

			float3 centerPos = (v0 + v1) / 2.0;

			float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
			float3 viewDirection = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, float4(centerPos, 0.0)).xyz);
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


			
			float multiplier = 1;

			if (_WorldSpaceLightPos0.w == 0)
			{
				multiplier = max(0, dot(_WorldSpaceLightPos0.xyz, normalDirection)) *.5;
			}
			else 
			{
				float3 dV = mul(unity_ObjectToWorld, float4(IN[0].pos.xyz, 1)).xyz;

				float3 lightToVert = dV - _WorldSpaceLightPos0.xyz;
				float distance = length(lightToVert);


				float3 lightVec = _WorldSpaceLightPos0.xyz - dV;
				attenuation = 1 / (dot(lightVec, lightVec));

				if (distance < _FlashLightRange)
				{
					float fallOff = 1 / max(1, (pow(distance, 5)));
					multiplier = fallOff;
				}
				else
				{
					multiplier = 1;
				}
			}


			float3 q1 = normalize(cross(v1 - v0, q1_0 - v0));
			normalDirection = normalize(mul(float4(q1, 0.0), unity_ObjectToWorld).xyz);
			centerPos = (v0 + v1) / 2.0;
			specularReflection = GetSpecularReflection(multiplier, attenuation, viewDirection, normalDirection);

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
			normalDirection = normalize(mul(float4(q2, 0.0), unity_WorldToObject).xyz);


			specularReflection = GetSpecularReflection(multiplier, attenuation, viewDirection, normalDirection);



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
			normalDirection = normalize(mul(float4(q3, 0.0), unity_WorldToObject).xyz);


			specularReflection = GetSpecularReflection(multiplier, attenuation, viewDirection, normalDirection);


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
		Pass
		{
			Tags{"LightMode" = "ForwardAdd"}
			Blend One One
			ZWrite Off
			//CULL OFF

			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			#pragma multi_compile DIRECTIONAL POINT SPOT
			#include "AutoLight.cginc"
			#include "UnityPBSLighting.cginc"

			sampler2D _MainTex;
			uniform float4 _SpecularColor;
			uniform float _Shininess;
			half _GrassHeight;
			half _GrassTopWidth;
			half _GrassBotWidth;
			uniform half _Cutoff;
			half _WindStength;
			half _WindSpeed;
			float _FlashLightRange;

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

			float GetSpecularReflection(float multiplier, float attenuation, float viewDirection, float normalDirection)
			{
				//return  unity_4LightAtten0 * _LightColor0.rgb * _SpecularColor.rgb * _Shininess;
				return multiplier * attenuation * _LightColor0.rgb * _SpecularColor.rgb * _Shininess;
				//float ref = max(reflect(normalDirection, -normalDirection), reflect(-normalDirection, normalDirection));
				//return multiplier * attenuation * _LightColor0.rgb * _SpecularColor.rgb * pow(max(0.0, dot(ref,
				//	viewDirection)), _Shininess);
			}

			v2g vert(appdata_full v)
			{

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

			float3 perpendicularAngle = float3(0, 0, 1);
			float3 faceNormal = cross(perpendicularAngle, IN[0].normal);
			float3 normalDirection;
			normalDirection = normalize(mul(float4(IN[0].normal, 0.0), unity_WorldToObject).xyz);

			float3 v0 = IN[0].pos.xyz;
			float3 v1 = IN[0].pos.xyz + IN[0].normal * _GrassHeight;

			float3 centerPos = (v0 + v1) / 2.0;

			float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
			float3 viewDirection = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, float4(centerPos, 0.0)).xyz);
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



			float multiplier;
			//float3 dV = mul(unity_ObjectToWorld, float4(IN[0].pos.xyz, 1)).xyz;
			//float3 lightVec = _WorldSpaceLightPos0.xyz - dV.xyz;
			//float3 lightDir = normalize(lightVec);
			//float ndotl = max(0, dot(IN[0].normal, lightDir));
			//multiplier = 1 / (1 + dot(lightVec, lightVec));


			if (_WorldSpaceLightPos0.w == 0)
			{
				multiplier = 0;// max(0, dot(_WorldSpaceLightPos0.xyz, normalDirection)) *.5;
			}
			else
			{
				float3 dV = mul(unity_ObjectToWorld, float4(IN[0].pos.xyz, 1)).xyz;

				float3 lightToVert = dV - _WorldSpaceLightPos0.xyz;
				float distance = length(lightToVert);


				float3 lightVec = _WorldSpaceLightPos0.xyz - dV;
				attenuation = 1;// 1 - (distance / (1 / _LightPositionRange.w));//1;//1 / (dot(lightVec, lightVec));

				float fallOff = 2 / max(0.001, pow(distance, 2.5));

				multiplier = fallOff;
			}


			float3 q1 = normalize(cross(v1 - v0, q1_0 - v0));
			normalDirection = normalize(mul(float4(q1, 0.0), unity_ObjectToWorld).xyz);
			centerPos = (v0 + v1) / 2.0;
			specularReflection = GetSpecularReflection(multiplier, attenuation, viewDirection, normalDirection);

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

			OUT.pos = UnityObjectToClipPos(v0 - perpendicularAngle * _GrassBotWidth);
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
			normalDirection = normalize(mul(float4(q2, 0.0), unity_WorldToObject).xyz);


			specularReflection = GetSpecularReflection(multiplier, attenuation, viewDirection, normalDirection);



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
			normalDirection = normalize(mul(float4(q3, 0.0), unity_WorldToObject).xyz);


			specularReflection = GetSpecularReflection(multiplier, attenuation, viewDirection, normalDirection);


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