using System;
using Events;
using UnityEngine;

namespace Marble {
    [RequireComponent(typeof(Rigidbody))]
    public class MarbleController : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] private VoidEventChannelSo goalReachedEvent;
        [SerializeField] private VoidEventChannelSo resetZoneEvent;
        
        private Rigidbody _rb;

        private void Awake() {
            _rb = GetComponent<Rigidbody>();
        }

        public void LaunchFrom(Transform startPoint) {
            transform.position = startPoint.position;
            transform.rotation = startPoint.rotation;
            
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.position = startPoint.position;
            
            //transform.position = startPoint.position;

            if (TryGetComponent(out TrailRenderer trailRenderer)) {
                trailRenderer.Clear();
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Goal")) {
                goalReachedEvent?.Raise();
                
                gameObject.SetActive(false);
            }
            else if (other.CompareTag("ResetZone")) {
                resetZoneEvent?.Raise();
                
                gameObject.SetActive(false);
            }
        }
    }
}
