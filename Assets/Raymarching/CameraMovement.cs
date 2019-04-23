using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public float movementSpeed;
    public Vector3 rotationAngle;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        transform.position += Vector3.forward * movementSpeed;
        transform.Rotate(rotationAngle.x, rotationAngle.y, rotationAngle.z, Space.Self);
    }
}
