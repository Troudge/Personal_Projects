Shader "Unlit/RayMarch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_RecursionDepth("Recursion Depth", Int ) = 3
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			//#pragma multi_compile_fog

			#include "UnityCG.cginc"

			#define MAX_STEPS 100
			#define MAX_DIST 100
			#define MIN_SURF_DIST 1e-3

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				//UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3 rayStart : TEXCOORD1;
				float3 hitPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			int _RecursionDepth = 3;

			float DivideVertex(v2f i)
			{
				i.vertex;
			}

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				o.rayStart = mul(unity_WorldToObject,float4(_WorldSpaceCameraPos,1));
				o.hitPos = v.vertex;
				return o;
			}

			float MengerDist(int n, float3 pos) { //by recursively digging a box
				float x = pos.x, y = pos.y, z = pos.z;
				x = x * 0.5 + 0.5; y = y * 0.5 + 0.5; z = z * 0.5 + 0.5; //center it by changing position and scale

				float xx = abs(x - 0.5) - 0.5, yy = abs(y - 0.5) - 0.5, zz = abs(z - 0.5) - 0.5;
				float d1 = max(xx, max(yy, zz)); //distance to the box
				float d = d1; //current computed distance
				float p = 1.0;
				for (int i = 1; i <= n; ++i) {
					float xa = fmod(3.0 * x * p, 3.0);
					float ya = fmod(3.0 * y * p, 3.0);
					float za = fmod(3.0 * z * p, 3.0);
					p *= 3.0;

					//we can also translate/rotate (xa,ya,za) without affecting the DE estimate

					float xx = 0.5 - abs(xa - 1.5), yy = 0.5 - abs(ya - 1.5), zz = 0.5 - abs(za - 1.5);
					d1 = min(max(xx, zz), min(max(xx, yy), max(yy, zz))) / p; //distance inside the 3 axis-aligned square tubes

					d = max(d, d1); //intersection
				}
				//return d*2.0; //the distance estimate. The *2 is because of the scaling we did at the beginning of the function
				return d;
			}

			float3 GetNormal(float3 pos)
			{
				float2 offset = float2(0.1e-2, 0);
				float3 normal = MengerDist(_RecursionDepth, pos) - float3(
					MengerDist(_RecursionDepth, pos - offset.xyy),
					MengerDist(_RecursionDepth, pos - offset.yxy),
					MengerDist(_RecursionDepth, pos - offset.yyx)
					);
				return normalize(normal);
			}

			float Raymarch(float3 rayStart, float3 rayDir)
			{
				float distToStart = 0;
				float distToSurface;
				for (int i = 0; i < MAX_STEPS; i++)
				{
					float3 currPos = rayStart + (distToStart * rayDir);
					distToSurface = MengerDist(_RecursionDepth,currPos);
					distToStart += distToSurface;
					if (distToSurface < MIN_SURF_DIST || distToStart > MAX_DIST) break;

				}
				return distToStart;


			}

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;
				//create starting ray
				float3 rayStart = i.rayStart;
				float3 rayDir = normalize(i.hitPos - rayStart);
				
				//Find distance using raymarcher
				float distance = Raymarch(rayStart, rayDir);
				fixed4 col = tex2D(_MainTex, i.uv);
				if (distance < MAX_DIST) 
				{
					float3 pos = rayStart + rayDir * distance;
					float3 normal = GetNormal(pos);
					col.rgb = distance;
				}
				
				
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
