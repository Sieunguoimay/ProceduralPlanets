using System.Collections;
using System.Collections.Generic;
using Unity.MPE;
using UnityEngine;

public class MarchedObject : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    [SerializeField] private OccupancyGridSettings occupancyGridSettings;
    [SerializeField] private NoiseOccupancySettings noiseOccupancySettings;

    //initalize at runtime
    void Start()
    {
        Initialize();
    }

    //initalize from editor
    void OnValidate()
    {
        Initialize();
    }

    public void Initialize()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        
        meshFilter = GetComponent<MeshFilter>();


        var marchingCude = new MarchingCube();

        var occupancyDescriptor = new NoiseOccupancyDescriptor(noiseOccupancySettings);

        var occupancyGrid = new OccupancyGrid(occupancyGridSettings,occupancyDescriptor);

        var gridMarcher = new GridMarcher(occupancyGrid, marchingCude);

        meshFilter.mesh = gridMarcher.BuildMesh();

    }
}
