using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/OccupancyGridSettings")]
public class OccupancyGridSettings : ScriptableObject
{
    public Bounds bounds;
    public float cellSize;

    [NonSerialized] public Vector3Int size;
}
