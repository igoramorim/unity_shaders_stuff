using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldParticle : MonoBehaviour {

    public float _moveSpeed;

    void Start() {
        
    }
    
    void Update() {
        this.transform.position += transform.forward * _moveSpeed * Time.deltaTime;
    }

    public void ApplyRotation(Vector3 rotation, float rotateSpeed) {
        Quaternion targetRotation = Quaternion.LookRotation(rotation.normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }
}
