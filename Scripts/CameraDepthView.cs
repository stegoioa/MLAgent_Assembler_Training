using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]

public class CameraDepthView : MonoBehaviour
{
    public Camera cam1;
    public Material mat;

    private void Awake()
    {
        if (cam1 == null)
        {
            cam1 = this.GetComponent<Camera>();
            cam1.depthTextureMode = DepthTextureMode.DepthNormals;
        }

        if (mat == null)
        {
            mat = new Material(Shader.Find("Hidden/depth"));
        }
    }

    private void FixedUpdate()
    {
        if (cam1 == null)
        {
            cam1 = this.GetComponent<Camera>();
            cam1.depthTextureMode = DepthTextureMode.DepthNormals;
        }

        if (mat == null)
        {
            mat = new Material(Shader.Find("Hidden/depth"));
        }

        //OnRenderImage(null, null);

    }


   
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }
}
