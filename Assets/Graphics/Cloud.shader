Shader "Unlit/Cloud"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        ShapeNoise("ShapeNoise", 3D) = "white" {}
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
            #include"UnityLightingCommon.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 viewVector: TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _CameraDepthTexture;

            float4x4 FrustumCorners;

            float3 BoundsMin;
            float3 BoundsMax;

            Texture3D<float4> ShapeNoise;
            SamplerState samplerShapeNoise;

            float3 CloudScale;
            float3 CloudOffset;
            float DensityMultiplier;
            float DensityThreshold;

            int NumSteps;
            int NumStepsLight;

            float LightAbsorptionTowardSun;
            float LightAbsorptionThroughCloud;
            float DarknessThreshold;

            float PhaseVal;

            float2 rayBoxDst(float3 boundsMin, float3 boundsMax, float3 rayOrigin, float3 rayDir) {
                float3 t0 = (boundsMin - rayOrigin) / rayDir;
                float3 t1 = (boundsMax - rayOrigin) / rayDir;

                float3 tmin = min(t0, t1);
                float3 tmax = max(t0, t1);

                float dstA = max(max(tmin.x, tmin.y), tmin.z);
                float dstB = min(tmax.x, min(tmax.y, tmax.z));

                float dstToBox = max(0, dstA);
                float dstInsideBox = max(0, dstB - dstToBox);
                return float2(dstToBox, dstInsideBox);
            }

            float sampleDensity(float3 position) {
                float3 uvw = position* CloudScale - CloudOffset -float3(0.5,0.5,0.5);
                float4 shape = ShapeNoise.SampleLevel(samplerShapeNoise, uvw, 0);
                float density = max(0, shape.r-DensityThreshold)* DensityMultiplier;
                return density;
            }
            float lightmarch(float3 position) {
                float3 dirToLight = _WorldSpaceLightPos0.xyz;
                float dstInsideBox = rayBoxDst(BoundsMin, BoundsMax, position, 1 / dirToLight).y;

                float stepSize = dstInsideBox / (float)NumStepsLight;
                float totalDensity = 0;

                for (int step = 0; step < NumStepsLight; step++) {
                    position += dirToLight * stepSize;
                    totalDensity += max(0, sampleDensity(position) * stepSize);
                }

                float transmittance = exp(-totalDensity * LightAbsorptionTowardSun);

                return DarknessThreshold + transmittance * (1 - DarknessThreshold);
            }
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                int frustumIndex = v.uv.x + (2 * o.uv.y);
                o.viewVector = FrustumCorners[frustumIndex];
                o.viewVector.w = frustumIndex;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.viewVector);

                float nonLinearDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float depth = LinearEyeDepth(nonLinearDepth);

                float2 rayBoxInfo = rayBoxDst(BoundsMin, BoundsMax, rayOrigin, rayDir);
                float dstToBox = rayBoxInfo.x;
                float dstInsideBox = rayBoxInfo.y;

                if (dstInsideBox > 0) {
                    float dstTravelled = 0;
                    float stepSize = dstInsideBox / (float)NumSteps;
                    float dstLimit = min(depth - dstToBox, dstInsideBox);

                    float transmittance = 1;
                    float3 lightEnergy = 0;

                    while (dstTravelled < dstLimit) {
                        float3 rayPos = rayOrigin + rayDir * (dstToBox + dstTravelled);
                        float density = sampleDensity(rayPos);

                        if (density > 0) {
                            float lightTransmittance = lightmarch(rayPos);
                            lightEnergy += density * stepSize * transmittance * lightTransmittance * PhaseVal;
                            transmittance *= exp(-density * stepSize * LightAbsorptionThroughCloud);

                            if (transmittance < 0.01) {
                                break;
                            }
                        }
                        dstTravelled += stepSize;
                    }

                    float3 cloudCol = lightEnergy * _LightColor0;


                    return float4(col.rgb * transmittance + cloudCol, 0);
                }
                return col;
            }
            ENDCG
        }
    }
}
