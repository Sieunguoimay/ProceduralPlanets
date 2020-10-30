using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    private Vector3[] frustumCorners = new Vector3[4];

    private Matrix4x4 frustumCornersArray = Matrix4x4.identity;

    private Camera cam;

    private List<IPostProcessing> postProcessings;// = new List<IPostProcessing>();


    //Events Trigger by UnityEditor
    void OnValidate()
    {
        Initialize();
    }
    private void Awake()
    {
        Initialize();
    }
    //Events triggered by ExecuteInEditMode
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        CalculateFrustumCorners();

        foreach (var postProcessing in postProcessings)
        {
            Graphics.Blit(source, destination, postProcessing.OnDrawPostProcessing());
        }
        if(postProcessings.Count==0)
        {
            Graphics.Blit(source, destination);
        }
    }


    //Camera Real Functions
    public void Initialize()
    {
        cam = GetComponent<Camera>();

        cam.depthTextureMode |= DepthTextureMode.Depth;

        postProcessings = new List<IPostProcessing>();
    }

    public void AddPostProcessingClient(IPostProcessing client)
    {
        postProcessings.Add(client);
    }

    private void CalculateFrustumCorners()
    {
        if (!cam)
        {
            return;
        }
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.farClipPlane, cam.stereoActiveEye, frustumCorners);

        var bottomLeft = transform.TransformVector(frustumCorners[0]);
        var topLeft = transform.TransformVector(frustumCorners[1]);
        var topRight = transform.TransformVector(frustumCorners[2]);
        var bottomRight = transform.TransformVector(frustumCorners[3]);

        frustumCornersArray.SetRow(0, bottomLeft);
        frustumCornersArray.SetRow(1, bottomRight);
        frustumCornersArray.SetRow(2, topLeft);
        frustumCornersArray.SetRow(3, topRight);
    }

    public Matrix4x4 FrustumCornersArray { get { return frustumCornersArray; } }


}
