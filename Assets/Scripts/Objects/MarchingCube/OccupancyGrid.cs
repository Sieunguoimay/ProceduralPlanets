using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupancyGrid 
{
    public IOccupancyDescriptor occupancyDescriptor;

    public OccupancyGridSettings settings;

    public OccupancyGrid(OccupancyGridSettings settings,IOccupancyDescriptor occupancyDescriptor)
    {
        this.occupancyDescriptor = occupancyDescriptor;
        this.settings = settings;
        this.settings.resolution = Vector3Int.FloorToInt(settings.bounds.size*(1.0f/ settings.cellSize));
    }


    public Vector3 GetPosition(int x, int y, int z)
    {
        Vector3 point = new Vector3(x, y, z) * settings.cellSize;
        Vector3 relPos = point - settings.bounds.size * 0.5f;
        return relPos;
    }
}
