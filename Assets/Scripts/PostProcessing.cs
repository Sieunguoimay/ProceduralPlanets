using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostProcessing : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private Planet planet;

    private new Camera camera;
    private Matrix4x4 frustumCornersArray;
    Vector3[] frustumCorners = new Vector3[4];

    private void Awake()
    {
        camera = GetComponent<Camera>();

        camera.depthTextureMode |= DepthTextureMode.Depth;
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        if (!camera)
        {
            return;
        }
        camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, camera.stereoActiveEye, frustumCorners);

        var bottomLeft = transform.TransformVector(frustumCorners[0]);
        var topLeft = transform.TransformVector(frustumCorners[1]);
        var topRight = transform.TransformVector(frustumCorners[2]);
        var bottomRight = transform.TransformVector(frustumCorners[3]);

        frustumCornersArray = Matrix4x4.identity;
        frustumCornersArray.SetRow(0, bottomLeft);
        frustumCornersArray.SetRow(1, bottomRight);
        frustumCornersArray.SetRow(2, topLeft);
        frustumCornersArray.SetRow(3, topRight);

        //Give the shader the frustum corners array
        material.SetVector("planetCentre", planet.transform.position);
        material.SetFloat("atmosphereRadius", planet.Radius*1.5f);
        material.SetMatrix("frustumCorners", frustumCornersArray);

        Graphics.Blit(source,destination, material);
    }
}
