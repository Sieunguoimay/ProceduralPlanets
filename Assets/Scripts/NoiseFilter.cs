using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFilter :INoiseFilter
{
    Noise noise = new Noise();
    NoiseSettings settings;

    public NoiseFilter(NoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;// (noise.Evaluate(point*settings.roughness+settings.centre) + 1) * .5f;
        float frequency = settings.baseRoughness;
        float amplitude = 1;
        for(int i = 0; i<settings.numLayers; i++)
        {
            float v = noise.Evaluate(point * frequency + settings.centre);
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistance;
        }
        noiseValue = noiseValue - settings.minValue;
        return noiseValue*settings.strength;
    }
}
