using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Teste : MonoBehaviour {

    public Material mat;

    AudioSpectrum spectrum;

    private void Awake() {
        spectrum = FindObjectOfType(typeof(AudioSpectrum)) as AudioSpectrum;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        Graphics.Blit(source, destination, mat);
    }

    private void FixedUpdate() {
        float scale = 0.0f;
        scale = spectrum.MeanLevels[0];
        //Debug.Log(scale + " - " + scale * 20.0f);
        mat.SetFloat("_Audio", scale);
    }

}
