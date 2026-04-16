using System;
using UnityEngine;

namespace Marble {
    [RequireComponent(typeof(Rigidbody))]
    public class MarbleController : MonoBehaviour
    {
        
        [Header("Settings")]
        [Tooltip("The amount of drag to apply to the magnet")]
        
        [SerializeField] private CubeManager cubeManager;
        
        private Rigidbody _rb;

        private void Awake() {
            _rb = GetComponent<Rigidbody>();
        }

        private void Start() {
            cubeManager = FindFirstObjectByType<CubeManager>();
            
        }

        public void LaunchFrom(Transform startPoint) {
            transform.position = startPoint.position;
            
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            
            _rb.position = startPoint.position;
            transform.position = startPoint.position;

            if (TryGetComponent(out TrailRenderer trailRenderer)) {
                trailRenderer.Clear();
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Goal")) {
                if (cubeManager) {
                    cubeManager.CompleteLevelAndRotate();
                }
                gameObject.SetActive(false);
            }
            else if (other.CompareTag("ResetZone")) {
                gameObject.SetActive(false);
            }
        }
    }
}
