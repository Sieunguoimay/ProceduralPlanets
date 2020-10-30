using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Cloud : MonoBehaviour, IPostProcessing
{
    [SerializeField] private ComputeShader worleyComputeShader;
    [SerializeField] private CameraController cameraController;
    public CloudSettings settings;

    private WorleyNoiseGenerator worleyNoiseGenerator;

    private INoiseFilter noiseFilter;


    //Editor Events
    private void OnValidate()
    {
        Initialize();
    }

    void Start()
    {
        Initialize();
    }

    //Self 

    public void UpdateFromEditor()
    {
        GenerateTexture();
    }

    public void Initialize()
    {

        cameraController.AddPostProcessingClient(this);

        //worleyNoiseGenerator = new WorleyNoiseGenerator(worleyComputeShader);
        //cloudSettings.texture = worleyNoiseGenerator.RunShader();

        noiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.noiseSettings);
        settings.texture = new Texture3D(40, 40, 40, TextureFormat.RGBA32, true);

        GenerateTexture();
    }

    public void GenerateTexture()
    {
        Color[] pixels = new Color[40 * 40 * 40];
        for (int x = 0; x < 40; x++)
        {
            for (int y = 0; y < 40; y++)
            {
                for (int z = 0; z < 40; z++)
                {
                    float a = (x / 40f) * (y / 40f) * (z / 40f);
                    pixels[x + 40 * (y + 40 * z)] = new Color(noiseFilter.Evaluate(new Vector3(x, y, z)), 0, 0);
                }
            }
        }
        settings.texture.SetPixels(pixels);
        settings.texture.filterMode = FilterMode.Bilinear;
        settings.texture.wrapMode = TextureWrapMode.Repeat;
        settings.texture.Apply();
    }

    //ExecuteInEditMode & Runtime Events
    void Update()
    {
        settings.bounds= new Bounds() { center = transform.position, size = transform.localScale };
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(settings.bounds.center, settings.bounds.size);
    }
    public Material OnDrawPostProcessing()
    {
        settings.material.SetMatrix("FrustumCorners", cameraController.FrustumCornersArray);

        settings.material.SetVector("BoundsMax", settings.bounds.center + settings.bounds.size * 0.5f);
        settings.material.SetVector("BoundsMin", settings.bounds.center - settings.bounds.size * 0.5f);

        settings.material.SetTexture("ShapeNoise", settings.texture);

        settings.material.SetVector("CloudScale", settings.CloudScale);
        settings.material.SetVector("CloudOffset", settings.CloudOffset);

        settings.material.SetFloat("DensityMultiplier", settings.DensityMultiplier);
        settings.material.SetFloat("DensityThreshold", settings.DensityThreshold);

        settings.material.SetInt("NumSteps", settings.NumSteps);
        settings.material.SetInt("NumStepsLight", settings.NumStepsLight);

        settings.material.SetFloat("LightAbsorptionTowardSun", settings.LightAbsorptionTowardSun);
        settings.material.SetFloat("LightAbsorptionThroughCloud", settings.LightAbsorptionThroughCloud);
        settings.material.SetFloat("DarknessThreshold", settings.DarknessThreshold);
        settings.material.SetFloat("PhaseVal", settings.PhaseVal);

        return settings.material;
    }
}
