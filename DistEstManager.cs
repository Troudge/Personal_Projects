using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class DistEstManager : MonoBehaviour
{
    public ComputeShader distanceEstimator;

    [Range(0, 360)]
    public float animate = 0;

    [Range(1,10)]
    public int iterate = 1;

    [Header("Animation Settings")]
    public float animateRate = 0.2f;

    RenderTexture target;
    Camera cam;
    Light directionalLight;

    [Header("Colour mixing")]
    [Range(0, 1)] public float redA;
    [Range(0, 1)] public float greenA;
    [Range(0, 1)] public float blueA = 1;
    [Range(0, 1)] public float redB;
    [Range(0, 1)] public float greenB;
    [Range(0, 1)] public float blueB = 1;

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Animate properties
    void Update()
    {
        if (Application.isPlaying)
        {
            animate += animateRate * Time.deltaTime;
        }
    }



    void SetParameters()
    {
        distanceEstimator.SetTexture(0, "Result", target);
        distanceEstimator.SetFloat("_Animate", animate);
        distanceEstimator.SetInt("_Iterate", iterate);
        distanceEstimator.SetVector("_ColorsA", new Vector3(redA, greenA, blueA));
        distanceEstimator.SetVector("_ColorsB", new Vector3(redB, greenB, blueB));
        distanceEstimator.SetMatrix("_CameraToWorld", cam.cameraToWorldMatrix);
        distanceEstimator.SetMatrix("_CameraInverseProjection", cam.projectionMatrix.inverse);
        distanceEstimator.SetVector("_LightDirection", directionalLight.transform.forward);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        InitRenderTexture();
        SetParameters();

        int threadGroupsX = Mathf.CeilToInt(cam.pixelWidth / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(cam.pixelHeight / 8.0f);
        distanceEstimator.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        Graphics.Blit(target, destination);
    }


    void InitRenderTexture()
    {
        cam = Camera.current;
        directionalLight = FindObjectOfType<Light>();
        if (target == null || target.width != cam.pixelWidth || target.height != cam.pixelHeight)
        {
            if (target != null)
            {
                target.Release();
            }
            target = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            target.enableRandomWrite = true;
            target.Create();
        }
    }
}
