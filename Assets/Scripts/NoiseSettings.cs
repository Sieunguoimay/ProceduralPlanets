using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NoiseSettings 
{
    public enum FilterType { Simple, Rigid};
    public FilterType filterType;

    public float strength = 1;
    [Range(1, 8)]
    public int numLayers = 1;
    public float baseRoughness = 1;
    public float roughness = 2;
    public float persistance = .5f;
    public Vector3 centre;

    public float minValue = 1;
}
