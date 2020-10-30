using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/AtmosphereSettings")]
public class AtmosphereSettings : ScriptableObject
{
    public Material material;

    public float atmosphereRadius = 2;
    public int numOpticalDepthPoints = 10;
    public int numInScatteringPoints = 10;
    public float densityFallOff;
    public float scatteringStrength = 3;
    public Vector3 wavelengths = new Vector3(700, 530, 440);

    public void SetUniforms()
    {
        material.SetFloat("atmosphereRadius", atmosphereRadius);

        material.SetInt("numOpticalDepthPoints", numOpticalDepthPoints);
        material.SetInt("numInScatteringPoints", numInScatteringPoints);
        material.SetFloat("densityFallOff", densityFallOff);

        float scatterR = Mathf.Pow(400 / wavelengths.x, 4) * scatteringStrength;
        float scatterG = Mathf.Pow(400 / wavelengths.y, 4) * scatteringStrength;
        float scatterB = Mathf.Pow(400 / wavelengths.z, 4) * scatteringStrength;

        Vector3 scatteringCoefficients = new Vector3(scatterR, scatterG, scatterB);
        material.SetVector("scatteringCoefficients", scatteringCoefficients);

    }
}
