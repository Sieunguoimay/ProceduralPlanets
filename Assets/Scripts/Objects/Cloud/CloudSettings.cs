using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="ScriptableObjects/CloudSettings")]
public class CloudSettings : ScriptableObject
{
    public Bounds bounds;
    public Material material;
    public Texture3D texture;

    public Vector3 CloudOffset;
    public Vector3 CloudScale;

    public float DensityMultiplier;
    public float DensityThreshold;

    public int NumSteps;
    public int NumStepsLight;

    public float LightAbsorptionTowardSun = 0.1f;
    public float LightAbsorptionThroughCloud;
    public float DarknessThreshold = 0;
    public float PhaseVal;

    public NoiseSettings noiseSettings;
}
