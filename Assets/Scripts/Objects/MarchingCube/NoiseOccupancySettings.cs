using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/NoiseOccupancySettings")]
public class NoiseOccupancySettings : ScriptableObject
{
    public float threshold = 0.5f;
    public Vector3 offset = Vector3.zero;
    public Vector3 scale = Vector3.one;


    public NoiseSettings noiseSettings;
}
