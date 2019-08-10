using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TangentCircles : CircleTangent {

    public GameObject _circlePrefab;
    public Vector4 _innerCircle, _outterCircle;
    [Range(1, 64)]
    public int _circleAmount;

    private GameObject _innerCircleGO, _outterCircleGO;
    private Vector4[] _tangentCircle;
    private GameObject[] _tangentObject;

    void Start() {
        _innerCircleGO = (GameObject)Instantiate(_circlePrefab);
        _outterCircleGO = (GameObject)Instantiate(_circlePrefab);
        _tangentCircle = new Vector4[_circleAmount];
        _tangentObject = new GameObject[_circleAmount];

        for (int i = 0; i < _circleAmount; i++) {
            GameObject tangentInstance = (GameObject)Instantiate(_circlePrefab);
            _tangentObject[i] = tangentInstance;
            _tangentObject[i].transform.parent = this.transform;
        }
    }

    void Update() {
        _innerCircleGO.transform.position = new Vector3(_innerCircle.x, _innerCircle.y, _innerCircle.z);
        _innerCircleGO.transform.localScale = new Vector3(_innerCircle.w, _innerCircle.w, _innerCircle.w) * 2;

        _outterCircleGO.transform.position = new Vector3(_outterCircle.x, _outterCircle.y, _outterCircle.z);
        _outterCircleGO.transform.localScale = new Vector3(_outterCircle.w, _outterCircle.w, _outterCircle.w) * 2;

        for (int i = 0; i < _circleAmount; i++) {
            _tangentCircle[i] = FindTangentCircle(_outterCircle, _innerCircle, (360f / _circleAmount) * i);
            _tangentObject[i].transform.position = new Vector3(_tangentCircle[i].x, _tangentCircle[i].y, _tangentCircle[i].z);
            _tangentObject[i].transform.localScale = new Vector3(_tangentCircle[i].w, _tangentCircle[i].w, _tangentCircle[i].w) * 2;
        }
    }

}
