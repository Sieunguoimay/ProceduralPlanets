using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeOccupancyDescriptor : IOccupancyDescriptor
{
    public float GetMiddlePoint()
    {
        return 0.5f;
    }

    public float GetValue(Vector3 point)
    {
        return 0;
    }

    public bool IsInside(Vector3 point)
    {
        //float y = Mathf.Sqrt(point.x * point.x + point.z * point.z) + 4 * Mathf.Cos(Mathf.Sin(point.x * point.x + point.z * point.z));
        float x = point.x;
        float y = point.z;
        //float z = -10 * x * y * Mathf.Exp(-x * x - y * y);
        //float z = Mathf.Cos(Mathf.Abs(x) + Mathf.Abs(y));
        return Vector3.Dot(point,point) > .5;
    }
}
