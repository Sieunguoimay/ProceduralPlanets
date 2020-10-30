using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidNoiseFilter:INoiseFilter
{
    Noise noise = new Noise();
    NoiseSettings settings;

    public RigidNoiseFilter(NoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;// (noise.Evaluate(point*settings.roughness+settings.centre) + 1) * .5f;
        float frequency = settings.baseRoughness;
        float amplitude = 1;
        float weight = 1;
        for (int i = 0; i < settings.numLayers; i++)
        {
            float v = 1-Mathf.Abs(noise.Evaluate(point * frequency + settings.centre));
            v *= v;
            v *= weight;
            weight = v;

            noiseValue += v* amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistance;
        }
        noiseValue =noiseValue - settings.minValue;
        return noiseValue * settings.strength;
    }
}
