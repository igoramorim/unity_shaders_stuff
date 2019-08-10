using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class UnderwaterEffect : MonoBehaviour {

    public Material mat;

    [Range(0.001f, 0.1f)]
    public float pixelOffset;
    [Range(0.1f, 20f)]
    public float noiseScale;
    [Range(0.1f, 20f)]
    public float noiseFrequency;
    [Range(0.1f, 30f)]
    public float noiseSpeed;

    public float depthStart;
    public float depthDistance;

    void Start() {
        
    }

    void Update() {
        mat.SetFloat("_NoiseFrequency", noiseFrequency);
        mat.SetFloat("_NoiseSpeed", noiseSpeed);
        mat.SetFloat("_NoiseScale", noiseSpeed);
        mat.SetFloat("_PixelOffset", pixelOffset);
        mat.SetFloat("_DepthStart", depthStart);
        mat.SetFloat("_DepthDistance", depthDistance);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        Graphics.Blit(source, destination, mat);
    }
}
