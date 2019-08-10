using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class RaymarchingCamera : SceneViewFilter {

    [SerializeField]
    private Shader _shader;

    public Material _raymarchMaterial {
        get {
            if (!_raymarchMat && _shader) {
                _raymarchMat = new Material(_shader);
                _raymarchMat.hideFlags = HideFlags.HideAndDontSave;
            }
            return _raymarchMat;
        }
    }

    private Material _raymarchMat;

    public Camera _camera {
        get {
            if (!_cam) {
                _cam = GetComponent<Camera>();
            }
            return _cam;
        }
    }

    private Camera _cam;
    public float _maxDistance;
    [Range(1, 300)]
    public int _MaxIterations;
    [Range(0.1f, 0.001f)]
    public float _Accuracy;

    [Header("Directional Light")]
    public Transform _directionalLight;
    public Color _LightCol;
    public float _LightIntensity;

    [Header("Shadow")]
    [Range(0, 4)]
    public float _ShadowIntensity;
    public Vector2 _ShadowDistance;
    [Range(1, 128)]
    public float _ShadowPenumbra;

    [Header("Ambient Occlusion")]
    [Range(0.01f, 10.0f)]
    public float _AoStepsize;
    [Range(1, 5)]
    public int _AoIterations;
    [Range(0, 1)]
    public float _AoIntensity;

    [Header("Reflection")]
    [Range(0, 2)]
    public int _ReflectionCount;
    [Range(0, 1)]
    public float _ReflectionIntensity;
    [Range(0, 1)]
    public float _EnvReflIntensity;
    public Cubemap _ReflectionCube;

    [Header("Signed Distance Field")]
    public Color _mainColor;
    // public Vector4 _sphere1, _sphere2;
    // public Vector4 _box1;
    // public float _box1round;
    // public float _boxSphereSmooth;
    // public float _sphereIntersectSmooth;
    // public Vector4 _torus1;
    // public Vector3 _modInterval;
    public Vector4 _sphere;
	public float _sphereSmooth;
    public float _degreeRotate;

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (!_raymarchMaterial) {
            Graphics.Blit(source, destination);
            return;
        }

        // Camera
        _raymarchMaterial.SetMatrix("_CamFrustum", CamFrustum(_camera));
        _raymarchMaterial.SetMatrix("_CamToWorld", _camera.cameraToWorldMatrix);
        _raymarchMaterial.SetFloat("_maxDistance", _maxDistance);
        _raymarchMaterial.SetInt("_MaxIterations", _MaxIterations);
        _raymarchMaterial.SetFloat("_Accuracy", _Accuracy);

        // Directional Light
        _raymarchMaterial.SetVector("_LightDir", _directionalLight ? _directionalLight.forward : Vector3.down);
        _raymarchMaterial.SetColor("_LightCol", _LightCol);
        _raymarchMaterial.SetFloat("_LightIntensity", _LightIntensity);

        // Shadow
        _raymarchMaterial.SetFloat("_ShadowIntensity", _ShadowIntensity);
        _raymarchMaterial.SetVector("_ShadowDistance", _ShadowDistance);
        _raymarchMaterial.SetFloat("_ShadowPenumbra", _ShadowPenumbra);

        // Ambient Occlusion
        _raymarchMaterial.SetFloat("_AoStepsize", _AoStepsize);
        _raymarchMaterial.SetInt("_AoIterations", _AoIterations);
        _raymarchMaterial.SetFloat("_AoIntensity", _AoIntensity);

        // Reflection
        _raymarchMaterial.SetInt("_ReflectionCount", _ReflectionCount);
        _raymarchMaterial.SetFloat("_ReflectionIntensity", _ReflectionIntensity);
        _raymarchMaterial.SetFloat("_EnvReflIntensity", _EnvReflIntensity);
        _raymarchMaterial.SetTexture("_ReflectionCube", _ReflectionCube);

        // Signed Distance Field
        _raymarchMaterial.SetColor("_mainColor", _mainColor);
        // _raymarchMaterial.SetVector("_sphere1", _sphere1);
        // _raymarchMaterial.SetVector("_sphere2", _sphere2);
        // _raymarchMaterial.SetVector("_box1", _box1);
        // _raymarchMaterial.SetFloat("_box1round", _box1round);
        // _raymarchMaterial.SetFloat("_boxSphereSmooth", _boxSphereSmooth);
        // _raymarchMaterial.SetFloat("_sphereIntersectSmooth", _sphereIntersectSmooth);

        // _raymarchMaterial.SetVector("_torus1", _torus1);
        // _raymarchMaterial.SetVector("_modInterval", _modInterval);

        _raymarchMaterial.SetVector("_sphere", _sphere);
        _raymarchMaterial.SetFloat("_sphereSmooth", _sphereSmooth);
        _raymarchMaterial.SetFloat("_degreeRotate", _degreeRotate);

        RenderTexture.active = destination;
        _raymarchMaterial.SetTexture("_MainTex", source);
        GL.PushMatrix();
        GL.LoadOrtho();
        _raymarchMaterial.SetPass(0);
        GL.Begin(GL.QUADS);

        // bottom left
        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 3.0f);
        // bottom right
        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 2.0f);
        // top right
        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 1.0f);
        // top left
        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();
    }

    private Matrix4x4 CamFrustum(Camera cam) {
        Matrix4x4 frustum = Matrix4x4.identity;
        float fov = Mathf.Tan((cam.fieldOfView * 0.5f) * Mathf.Deg2Rad);

        Vector3 goUp = Vector3.up * fov;
        Vector3 goRight = Vector3.right * fov * cam.aspect;

        Vector3 TL = (-Vector3.forward - goRight + goUp); // top left
        Vector3 TR = (-Vector3.forward + goRight + goUp); // top right
        Vector3 BR = (-Vector3.forward + goRight - goUp); // bottom right
        Vector3 BL = (-Vector3.forward - goRight - goUp); // bottom left

        frustum.SetRow(0, TL);
        frustum.SetRow(1, TR);
        frustum.SetRow(2, BR);
        frustum.SetRow(3, BL);

        return frustum;
    }
}
