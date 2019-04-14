using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandelbrotController : MonoBehaviour {

    public Material mat;
    public Vector2 pos;
    public float scale, angle;

    private Vector2 smoothPos;
    private float smoothScale, smoothAngle;

    void FixedUpdate() {
        HandleInputs();
        UpdateShader();
    }

    private void HandleInputs() {
        // Scale
        if (Input.GetKey(KeyCode.KeypadPlus))
            scale *= .99f;

        if (Input.GetKey(KeyCode.KeypadMinus))
            scale *= 1.01f;

        // Position
        Vector2 dir = new Vector2(.01f * scale, 0);
        float s = Mathf.Sin(angle);
        float c = Mathf.Cos(angle);
        dir = new Vector2(dir.x * c - dir.y * s, dir.x * s + dir.y * c);

        if (Input.GetKey(KeyCode.A))
            pos -= dir;

        if (Input.GetKey(KeyCode.D))
            pos += dir;

        dir = new Vector2(-dir.y, dir.x);
        if (Input.GetKey(KeyCode.W))
            pos += dir;

        if (Input.GetKey(KeyCode.S))
            pos -= dir;

        // Angle
        if (Input.GetKey(KeyCode.Q))
            angle += .01f;

        if (Input.GetKey(KeyCode.E))
            angle -= .01f;
    }

    private void UpdateShader() {
        smoothPos = Vector2.Lerp(smoothPos, pos, .03f);
        smoothScale = Mathf.Lerp(smoothScale, scale, .03f);
        smoothAngle = Mathf.Lerp(smoothAngle, angle, .03f);

        float aspect = (float)Screen.width / (float)Screen.height;

        float scaleX = smoothScale;
        float scaleY = smoothScale;

        if (aspect > 1f)
            scaleY /= aspect;
        else
            scaleX *= aspect;

        mat.SetVector("_Area", new Vector4(smoothPos.x, smoothPos.y, scaleX, scaleY));
        mat.SetFloat("_Angle", smoothAngle);
    }
}
