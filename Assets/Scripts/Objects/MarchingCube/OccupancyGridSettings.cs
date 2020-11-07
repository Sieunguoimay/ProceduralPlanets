using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/OccupancyGridSettings")]
public class OccupancyGridSettings : ScriptableObject
{
    public Bounds bounds;
    public float cellSize;

    public Vector3Int resolution;
}
