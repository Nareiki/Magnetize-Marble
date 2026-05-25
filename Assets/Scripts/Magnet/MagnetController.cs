using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Magnet {
    public class MagnetController : MonoBehaviour, IPointerClickHandler {
        [Header("Physics Settings")]
        [SerializeField] private float baseMagnetStrength = 10f;
        [SerializeField] private float magnetRange = 5f;
        [SerializeField] private LayerMask affectedLayers;
        
        [Header("Magnet Settings")]
        [Tooltip("Positive for Push, Negative for Pull")]
        [SerializeField] private float polarity = 1f; 
    
        public event Action<int> OnStrengthLevelChanged;
        
        private int _currentStrengthLevel = 1;
        
        public int CurrentStrengthLevel => _currentStrengthLevel;

        private float ActiveMagneticForce => polarity * _currentStrengthLevel * baseMagnetStrength;

        private Collider[] _overlappingColliders = new Collider[5];
    
        private void FixedUpdate() {
            ApplyMagneticForceToSurroundingRigidbodies();
        }

        private void ApplyMagneticForceToSurroundingRigidbodies() {
            var numCollidersFound = Physics.OverlapSphereNonAlloc(transform.position, magnetRange, _overlappingColliders, affectedLayers);
        
            for (var i = 0; i < numCollidersFound; i++) {
                var rb = _overlappingColliders[i].attachedRigidbody;
                if (!rb) {
                    continue;
                }

                var direction = rb.position - transform.position;
                var distance = direction.magnitude;

                if (distance <= 0.1) {
                    continue;
                }

                var force = direction.normalized * ActiveMagneticForce / distance;

                rb.AddForce(force);
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.dragging) return;

            _currentStrengthLevel++;
            if (_currentStrengthLevel > 3)
            {
                _currentStrengthLevel = 0;
            }

            OnStrengthLevelChanged?.Invoke(_currentStrengthLevel);
            //UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            float scaleMultiplier = 1f + (_currentStrengthLevel * 0.2f);
            transform.localScale = Vector3.one * scaleMultiplier;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, magnetRange);
        }
    }
}
