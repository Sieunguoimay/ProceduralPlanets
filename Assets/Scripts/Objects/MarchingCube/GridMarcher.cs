using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GridMarcher
{
    private OccupancyGrid occupancyGrid;

    private MarchingCube marchingCube;

    private ComputeShader computeShader;


    public GridMarcher(OccupancyGrid occupancyGrid, ComputeShader computeShader)
    {
        this.occupancyGrid = occupancyGrid;

        this.marchingCube = new MarchingCube();

        marchingCube.Init();

        this.computeShader = computeShader;
    }

    public Mesh BuildMesh()
    {
        if (occupancyGrid.settings.resolution.x < 1) return null;
        if (occupancyGrid.settings.resolution.y < 1) return null;
        if (occupancyGrid.settings.resolution.z < 1) return null;
        if (occupancyGrid.settings.cellSize <= 0.0f) return null;

        List<Vector3> vertices = new List<Vector3>();

        List<int> triangles = new List<int>();
        Vector3Int[] vertexTable = new Vector3Int[]
        {
            new Vector3Int(0,0,0),
            new Vector3Int(0,0,1),
            new Vector3Int(1,0,1),
            new Vector3Int(1,0,0),
            new Vector3Int(0,1,0),
            new Vector3Int(0,1,1),
            new Vector3Int(1,1,1),
            new Vector3Int(1,1,0),
        };
        float[] values = new float[8];

        for (int x = 0; x < occupancyGrid.settings.resolution.x - 1; x++)
        {
            for (int y = 0; y < occupancyGrid.settings.resolution.y - 1; y++)
            {
                for (int z = 0; z < occupancyGrid.settings.resolution.z - 1; z++)
                {
                    int cubeIndex = 0;

                    for(int i = 0; i<vertexTable.Length; i++)
                    {
                        var v = new Vector3Int(x,y,z)+vertexTable[i];

                        if (!occupancyGrid.occupancyDescriptor.IsInside(v))
                        {
                            marchingCube.CalculateCubeIndex(ref cubeIndex, i);
                        }

                        values[i] = occupancyGrid.occupancyDescriptor.GetValue(v);
                    }

                    //bottom-left-backward vertex
                    Vector3 offset = occupancyGrid.GetPosition(x,y,z);

                    marchingCube.AddVertices(vertices,triangles, occupancyGrid.settings.cellSize, offset, cubeIndex,values,occupancyGrid.occupancyDescriptor.GetMiddlePoint());
                }
            }
        }
        
        BuildMeshInGPU();

        Mesh mesh = new Mesh();
        
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();   
        
        mesh.RecalculateNormals();
        
        return mesh;
    }

    public void BuildMeshInGPU()
    {

        float[] data = new float[16];
        int size = data.Length;

        ComputeBuffer computeBuffer = new ComputeBuffer(size,sizeof(float));
        
        int kernel = computeShader.FindKernel("CSMain");
        
        computeShader.SetBuffer(kernel, "Output", computeBuffer);

        computeShader.SetInts("Size",new int[]{4,4,1});
        
        computeShader.Dispatch(kernel, 4/2, 4/2, 1);

        computeBuffer.GetData(data);

        for(int i = 0; i<data.Length; i++)
        {
            Debug.Log("Data: " + data[i]);
        }
    }
}
