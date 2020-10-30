using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOccupancyDescriptor
{
    bool IsInside(Vector3 point);
}
