using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorleyNoiseGenerator
{
    private ComputeShader computeShader;

    public Vector3Int texture3DResolution;

    public int numCellsPerAxis;


    public WorleyNoiseGenerator(ComputeShader computeShader)
    {
        this.computeShader = computeShader;
        numCellsPerAxis = 8;
        texture3DResolution = new Vector3Int(40, 40, 40);
    }

    private Vector3[] CreateWorleyPointsBuffer(int numCellsPerAxis)
    {
        var points = new Vector3[numCellsPerAxis * numCellsPerAxis * numCellsPerAxis];
        float cellSize = 1f / numCellsPerAxis;


        for(int x =0; x<numCellsPerAxis; x++)
        {
            for(int y = 0; y<numCellsPerAxis; y++)
            {
                for (int z = 0; z < numCellsPerAxis; z++)
                {
                    var randomOffset = new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                    var position = (new Vector3(x, y, z) + randomOffset) * cellSize;
                    int index = x + numCellsPerAxis * (y + z * numCellsPerAxis);
                    points[index] = position;
                }
            }
        }
        return points;
    }
    private ComputeBuffer CreateBuffer(Array data, int stride)
    {
        var buffer = new ComputeBuffer(data.Length, stride, ComputeBufferType.Raw);
        buffer.SetData(data);
        return buffer;
    }

    public Texture3D RunShader()
    {
        int kernel = computeShader.FindKernel("CSMain");

        var points = CreateWorleyPointsBuffer(numCellsPerAxis);
        var buffer = CreateBuffer(points, sizeof(float) * 3);
        computeShader.SetBuffer(kernel, "Points", buffer);

        computeShader.SetInt("NumCellsPerAxis", numCellsPerAxis);
        computeShader.SetVector("TextureResolution", new Vector4(texture3DResolution.x, texture3DResolution.y, texture3DResolution.z,0));

        //Create 3D Render Texture 1
        var renderTexture = new RenderTexture(texture3DResolution.x, texture3DResolution.y, 0);
        renderTexture.enableRandomWrite = true;
        renderTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        renderTexture.volumeDepth = texture3DResolution.z;
        renderTexture.Create();

        computeShader.SetTexture(kernel, "Result", renderTexture);

        
        computeShader.GetKernelThreadGroupSizes(kernel, out uint groupSizeX, out uint groupSizeY, out uint groupSizeZ);
        computeShader.Dispatch(kernel, texture3DResolution.x / (int)groupSizeX, texture3DResolution.y / (int)groupSizeY,  texture3DResolution.z/(int)groupSizeZ);
        

        Texture3D tex = new Texture3D(texture3DResolution.x, texture3DResolution.y, texture3DResolution.z,TextureFormat.RGBA32,false);
        Graphics.CopyTexture(renderTexture, tex);
        tex.Apply();
        return tex;
    }
}

