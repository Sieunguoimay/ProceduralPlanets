using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MPE;
using UnityEngine;

public class MarchedObject : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    public OccupancyGridSettings occupancyGridSettings;
    public NoiseOccupancySettings noiseOccupancySettings;

    [SerializeField] private ComputeShader computeShader;

    [HideInInspector]
    public bool occupancyGridSettingsFoldout;
    [HideInInspector]
    public bool noiseOccupancySettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;
    

    private GridMarcher gridMarcher;

    public ColorSettings colorSettings;
    public ColorGenerator colorGenerator = new ColorGenerator();

    //initalize at runtime
    void Start()
    {
        Initialize();
    }

    //initalize from editor
    //void OnValidate()
    //{
    //    Initialize();
    //}

    public void Initialize()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        meshFilter = GetComponent<MeshFilter>();



        var occupancyDescriptor = new NoiseOccupancyDescriptor(noiseOccupancySettings);

        var occupancyGrid = new OccupancyGrid(occupancyGridSettings, occupancyDescriptor);

        gridMarcher = new GridMarcher(occupancyGrid,computeShader);

        meshFilter.mesh = gridMarcher.BuildMesh();
    }

    internal void OnColorSettingsUpdated()
    {
        Initialize();
    }

    public void OnOccupancyGridSettingsUpdated()
    {
        Initialize();
    }
    public void OnNoiseOccupancySettingsUpdated()
    {
        Initialize();
    }
    public void OnDrawGizmos()
    {
        occupancyGridSettings.bounds.center = transform.position;
        Gizmos.DrawWireCube(occupancyGridSettings.bounds.center, occupancyGridSettings.bounds.size);
    }
}
