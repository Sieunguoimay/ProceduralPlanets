// Upgrade NOTE: commented out 'float3 _WorldSpaceCameraPos', a built-in variable

// Upgrade NOTE: commented out 'float3 _WorldSpaceCameraPos', a built-in variable

Shader "Unlit/Atmosphere"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        //densityFallOff("densityFallOff", Float) = 1.0
        //atmosphereRadius("atmosphereRadius",Float) = 2.0
        //numOpticalDepthPoints("numOpticalDepthPoints",Int) = 10
        //numInScatteringPoints("numInScatteringPoints", Int) = 10
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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 viewDir : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _CameraDepthTexture;

            float4x4 frustumCorners;

            float3 planetCentre;
            float planetRadius;

            float atmosphereRadius;

            int numOpticalDepthPoints;
            int numInScatteringPoints;
            float densityFallOff;

            float3 scatteringCoefficients;

            const float maxFloat = 10000000;
            float2 raySphere(float3 sphereCentre, float sphereRadius, float3 rayOrigin, float3 rayDir) {
                float3 offset = rayOrigin - sphereCentre;
                float a = 1;
                float b = 2 * dot(offset, rayDir);
                float c = dot(offset, offset) - sphereRadius * sphereRadius;
                float d = b * b - 4 * a * c;

                //number of intersections: 0 when d<0; 1 when d=0; 2 when d>0
                if (d > 0) {
                    float s = sqrt(d);
                    float dstToSphereNear = max(0, (-b - s) / (2 * a));
                    float dstToSphereFar = (-b + s) / (2 * a);

                    //ignore intersections that occur behind the ray
                    if (dstToSphereFar >= 0) {
                        return float2(dstToSphereNear, dstToSphereFar - dstToSphereNear);
                    }
                }
                //ray didnot intersects with sphere
                return float2(10000000, 0);
            }
            float densityAtPoint(float3 densitySamplePoint) {
                float heightAboveSurface = length(densitySamplePoint - planetCentre) - planetRadius;
                float height01 = heightAboveSurface / (atmosphereRadius - planetRadius);
                float localDensity = exp(-height01 * densityFallOff) * (1 - height01);
                return localDensity;
            }
            float opticalDepth(float3 rayOrigin, float3 rayDir, float rayLength) {
                float3 densitySamplePoint = rayOrigin;
                float stepSize = rayLength / (numOpticalDepthPoints - 1);
                float opticalDepth = 0;
                for (int i = 0; i < numOpticalDepthPoints - 1; i++) {
                    float localDensity = densityAtPoint(densitySamplePoint);
                    opticalDepth += localDensity * stepSize;
                    densitySamplePoint += rayDir * stepSize;
                }
                return opticalDepth;
            }
            float3 calculateLight(float3 rayOrigin, float3 rayDir, float rayLength, float3 originalCol) {
                float3 inScatterPoint = rayOrigin;
                float stepSize = rayLength / (numInScatteringPoints - 1);
                float3 inScatteredLight = 0;

                float3 dirToSun = _WorldSpaceLightPos0;
                float viewRayOpticalDepth = 0;
                for (int i = 0; i < numInScatteringPoints - 1;i++) {
                    float sunRayLength = raySphere(planetCentre, atmosphereRadius, inScatterPoint, dirToSun).y;
                    float sunRayOpticalDepth = opticalDepth(inScatterPoint, dirToSun, sunRayLength);
                    viewRayOpticalDepth = opticalDepth(inScatterPoint, -rayDir, stepSize * i); 
                    float3 transmittance = exp(-(sunRayOpticalDepth+viewRayOpticalDepth)* scatteringCoefficients);

                    float localDensity = densityAtPoint(inScatterPoint);
                    inScatteredLight += localDensity * transmittance * scatteringCoefficients* stepSize;
                    inScatterPoint += rayDir * stepSize;
                }
                float originalColTransmittance = exp(-viewRayOpticalDepth);
                return originalCol* originalColTransmittance + inScatteredLight;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = v.uv;


                int frustumIndex = v.uv.x + (2 * o.uv.y);
                o.viewDir = frustumCorners[frustumIndex];
                o.viewDir.w = frustumIndex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float sceneDepthNonLinear = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float sceneDepth = LinearEyeDepth(sceneDepthNonLinear);

                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.viewDir);

                float2 hitInfo = raySphere(planetCentre, atmosphereRadius, rayOrigin, rayDir);
                float dstToAtmosphere = hitInfo.x;
                float dstThroughAtmosphere = min(hitInfo.y, sceneDepth - dstToAtmosphere);
            
                if (dstThroughAtmosphere > 0) {
                    float3 pointInAtmosphere = rayOrigin + rayDir * dstToAtmosphere;
                    float3 light = calculateLight(pointInAtmosphere, rayDir, dstThroughAtmosphere,col.rgb); 

                    return float4(light,0);
                }
                return col;// (dstThroughAtmosphere / (atmosphereRadius * 2.0));
            }
            ENDCG
        }
    }
}
