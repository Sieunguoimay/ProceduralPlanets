using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmosphere : MonoBehaviour, IPostProcessing
{
    public AtmosphereSettings settings;

    public Planet planet;

    [SerializeField] private CameraController cameraController;

    private void OnValidate()
    {
        planet = GetComponent<Planet>();
        cameraController.AddPostProcessingClient(this);
    }
    private void Awake()
    {
        planet = GetComponent<Planet>();
        cameraController.AddPostProcessingClient(this);
    }

    public Material OnDrawPostProcessing()
    {

        //Give the shader the frustum corners array
        settings.material.SetMatrix("frustumCorners", cameraController.FrustumCornersArray);

        settings.material.SetVector("planetCentre", planet.transform.position);
        settings.material.SetFloat("planetRadius", planet.Radius);

        settings.SetUniforms();

        return settings.material;
    }
}
