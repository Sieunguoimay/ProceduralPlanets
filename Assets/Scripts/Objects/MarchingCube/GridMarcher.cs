using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GridMarcher
{
    private OccupancyGrid occupancyGrid;
    private MarchingCube marchingCube;

    public GridMarcher(OccupancyGrid occupancyGrid, MarchingCube marchingCube)
    {
        this.occupancyGrid = occupancyGrid;
        this.marchingCube = marchingCube;
    }

    public Mesh BuildMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int x = 0; x < occupancyGrid.settings.size.x - 1; x++)
        {
            for (int y = 0; y < occupancyGrid.settings.size.y - 1; y++)
            {
                for (int z = 0; z < occupancyGrid.settings.size.z - 1; z++)
                {
                    int cubeIndex = 0;

                    if (occupancyGrid.IsInside(x, y, z)) marchingCube.CalculateCubeIndex(cubeIndex, 0);
                    if (occupancyGrid.IsInside(x + 1, y, z)) marchingCube.CalculateCubeIndex(cubeIndex, 3);
                    if (occupancyGrid.IsInside(x, y + 1, z)) marchingCube.CalculateCubeIndex(cubeIndex, 4);
                    if (occupancyGrid.IsInside(x, y, z + 1)) marchingCube.CalculateCubeIndex(cubeIndex, 1);
                    if (occupancyGrid.IsInside(x + 1, y + 1, z)) marchingCube.CalculateCubeIndex(cubeIndex, 5);
                    if (occupancyGrid.IsInside(x + 1, y, z + 1)) marchingCube.CalculateCubeIndex(cubeIndex, 2);
                    if (occupancyGrid.IsInside(x, y + 1, z + 1)) marchingCube.CalculateCubeIndex(cubeIndex, 7);
                    if (occupancyGrid.IsInside(x + 1, y + 1, z + 1)) marchingCube.CalculateCubeIndex(cubeIndex, 6);

                    //bottom-left-backward vertex
                    Vector3 offset = occupancyGrid.GetPosition(x,y,z);

                    marchingCube.AddVertices(vertices,triangles, occupancyGrid.settings.cellSize, offset, cubeIndex);
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}
