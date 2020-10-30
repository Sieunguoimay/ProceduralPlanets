using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseOccupancyDescriptor : IOccupancyDescriptor
{
    private INoiseFilter noiseFilter3D;

    private NoiseOccupancySettings settings;

    public NoiseOccupancyDescriptor(NoiseOccupancySettings settings)
    {
        noiseFilter3D = NoiseFilterFactory.CreateNoiseFilter(settings.noiseSettings);
    }

    public bool IsInside(Vector3 point)
    {
        return noiseFilter3D.Evaluate(Vector3.Scale(point,settings.scale)+settings.offset) > settings.threshold;
    }
}
