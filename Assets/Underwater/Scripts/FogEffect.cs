using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class FogEffect : MonoBehaviour {

    public Material mat;
    public Color fogColor;
    public float depthStart;
    public float depthDistance;

    void Start() {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

    void Update() {
        mat.SetColor("_FogColor", fogColor);
        mat.SetFloat("_DepthStart", depthStart);
        mat.SetFloat("_DepthDistance", depthDistance);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        Graphics.Blit(source, destination, mat);
    }

}
