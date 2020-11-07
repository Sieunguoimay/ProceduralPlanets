using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseOccupancyDescriptor : IOccupancyDescriptor
{
    private INoiseFilter noiseFilter3D;

    private NoiseOccupancySettings settings;

    public NoiseOccupancyDescriptor(NoiseOccupancySettings settings)
    {
        this.settings = settings;

        noiseFilter3D = NoiseFilterFactory.CreateNoiseFilter(settings.noiseSettings);
    }

    public float GetMiddlePoint()
    {
        return settings.threshold;
    }

    public float GetValue(Vector3 point)
    {
        return noiseFilter3D.Evaluate(Vector3.Scale(point, settings.scale) + settings.offset);
    }

    public bool IsInside(Vector3 point)
    {
        return  GetValue(point) > settings.threshold;
    }

}
