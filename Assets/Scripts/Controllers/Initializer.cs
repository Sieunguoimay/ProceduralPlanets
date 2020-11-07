using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Cloud cloud;
    [SerializeField] private MarchedObject marchedObject;
    [SerializeField] private Planet planet;

    private void OnValidate()
    {
        cameraController.Initialize();
        //cloud.Initialize();
        //marchedObject.Initialize();
        planet.GeneratePlanet();
    }
    void Start()
    {
        
    }
}
