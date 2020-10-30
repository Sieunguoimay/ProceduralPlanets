using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupancyGrid 
{
    private IOccupancyDescriptor occupancyDescriptor;

    public OccupancyGridSettings settings;

    public OccupancyGrid(OccupancyGridSettings settings,IOccupancyDescriptor occupancyDescriptor)
    {
        this.occupancyDescriptor = occupancyDescriptor;
        this.settings = settings;
        this.settings.size = Vector3Int.FloorToInt(settings.bounds.size *(1.0f/ settings.cellSize));
    }

    public bool IsInside(int x, int y, int z)
    {
        float cellSize = settings.bounds.size.x / settings.size.x;
        Vector3 point = new Vector3(x, y, z) * cellSize;
        Vector3 origin = new Vector3(settings.size.x, settings.size.y, settings.size.z) * cellSize * 0.5f;
        Vector3 relPos = point - origin;

        return occupancyDescriptor.IsInside(relPos);
    }

    public Vector3 GetPosition(int x, int y, int z)
    {
        return new Vector3();
    }
}
