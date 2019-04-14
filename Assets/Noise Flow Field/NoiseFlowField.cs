using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFlowField : MonoBehaviour {

    FastNoise _fastNoise;

    public Vector3Int _gridSize;
    public float _cellSize;
    public float _increment;
    public Vector3 _offset, _offsetSpeed;
    public Vector3[,,] _flowFieldDirection;

    // Particles
    public GameObject _particlePrefab;
    public int _amountOfParticles;
    [HideInInspector]
    public List<FlowFieldParticle> _particles;

    public float _spawnRadius;
    public float _particleScale, _particleMoveSpeed, _particleRotateSpeed;
    
    bool _particleSpwanValidation(Vector3 position) {
        bool valid = true;

        foreach (FlowFieldParticle particle in _particles) {
            if (Vector3.Distance(position, particle.transform.position) < _spawnRadius) {
                valid = false;
                break;
            }
        }
        if (valid) {
            return true;
        } else {
            return false;
        }
    }

    void Start() {
        _flowFieldDirection = new Vector3[_gridSize.x, _gridSize.y, _gridSize.z];
        _fastNoise = new FastNoise();
        _particles = new List<FlowFieldParticle>();

        for (int i = 0; i < _amountOfParticles; i++) {
            int attempt = 0;

            while (attempt < 100) {
                Vector3 randomPos = new Vector3(
                    Random.Range(this.transform.position.x, this.transform.position.x + _gridSize.x * _cellSize),
                    Random.Range(this.transform.position.y, this.transform.position.y + _gridSize.y * _cellSize),
                    Random.Range(this.transform.position.z, this.transform.position.z + _gridSize.z * _cellSize));

                bool isValid = _particleSpwanValidation(randomPos);

                if (isValid) {
                    GameObject particleInstance = (GameObject)Instantiate(_particlePrefab);
                    particleInstance.transform.position = randomPos;
                    particleInstance.transform.parent = this.transform;
                    particleInstance.transform.localScale = new Vector3(_particleScale, _particleScale, _particleScale);
                    _particles.Add(particleInstance.GetComponent<FlowFieldParticle>());
                    break;
                }
                if (!isValid) {
                    attempt++;
                }
            }
        }
        Debug.Log(_particles.Count);
    }

    void Update() {
        CalculateFlowFieldDirections();
        ParticleBehaviour();
    }

    void CalculateFlowFieldDirections() {
        _offset = new Vector3(_offset.x + (_offsetSpeed.x * Time.deltaTime), _offset.y + (_offsetSpeed.y * Time.deltaTime), _offset.z + (_offsetSpeed.z * Time.deltaTime));

        float xOff = 0;
        for (int x = 0; x < _gridSize.x; x++) {
            float yOff = 0;
            for (int y = 0; y < _gridSize.y; y++) {
                float zOff = 0;
                for (int z = 0; z < _gridSize.z; z++) {
                    float noise = _fastNoise.GetSimplex(xOff + _offset.x, yOff + _offset.y, zOff + _offset.z) + 1;
                    Vector3 noiseDirection = new Vector3(Mathf.Cos(noise * Mathf.PI), Mathf.Sin(noise * Mathf.PI), Mathf.Cos(noise * Mathf.PI));
                    _flowFieldDirection[x, y, z] = Vector3.Normalize(noiseDirection);
                    zOff += _increment;
                }
                yOff += _increment;
            }
            xOff += _increment;
        }
    }

    void ParticleBehaviour() {
        foreach(FlowFieldParticle p in _particles) {
            // Check x edges
            if (p.transform.position.x > this.transform.position.x + (_gridSize.x * _cellSize)) {
                p.transform.position = new Vector3(this.transform.position.x, p.transform.position.y, p.transform.position.z);
            }
            if (p.transform.position.x < this.transform.position.x) {
                p.transform.position = new Vector3(this.transform.position.x + (_gridSize.x * _cellSize), p.transform.position.y, p.transform.position.z);
            }

            // Check y edges
            if (p.transform.position.y > this.transform.position.y + (_gridSize.y * _cellSize)) {
                p.transform.position = new Vector3(p.transform.position.x, this.transform.position.y, p.transform.position.z);
            }
            if (p.transform.position.y < this.transform.position.y) {
                p.transform.position = new Vector3(p.transform.position.x, this.transform.position.y + (_gridSize.y * _cellSize), p.transform.position.z);
            }

            // Check z edges
            if (p.transform.position.z > this.transform.position.z + (_gridSize.z * _cellSize)) {
                p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, this.transform.position.z);
            }
            if (p.transform.position.z < this.transform.position.z) {
                p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, this.transform.position.z + (_gridSize.z * _cellSize));
            }

            Vector3Int particlePos = new Vector3Int(
                Mathf.FloorToInt(Mathf.Clamp((p.transform.position.x - this.transform.position.x) / _cellSize, 0, _gridSize.x - 1)),
                Mathf.FloorToInt(Mathf.Clamp((p.transform.position.y - this.transform.position.y) / _cellSize, 0, _gridSize.y - 1)),
                Mathf.FloorToInt(Mathf.Clamp((p.transform.position.z - this.transform.position.z) / _cellSize, 0, _gridSize.z - 1))
            );
            p.ApplyRotation(_flowFieldDirection[particlePos.x, particlePos.y, particlePos.z], _particleRotateSpeed);
            p._moveSpeed = _particleMoveSpeed;
            p.transform.localScale = new Vector3(_particleScale, _particleScale, _particleScale);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(
            this.transform.position + new Vector3((_gridSize.x *_cellSize) * 0.5f,
                                                  (_gridSize.y * _cellSize) * 0.5f,
                                                  (_gridSize.z * _cellSize) * 0.5f),
            new Vector3(_gridSize.x * _cellSize,
                        _gridSize.y * _cellSize, 
                        _gridSize.z * _cellSize)
        );
    }
}
