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

        [Header("Continuous Visual FX")]
        [Tooltip("The continuous particle system ring emitter attached as a child object.")]
        [SerializeField] private ParticleSystem constantFieldParticles;
        [Tooltip("Base burst interval rate for strength level 1 (e.g., 1.2 seconds between rings)")]
        [SerializeField] private float baseEmissionInterval = 1.2f;
    
        public event Action<int> OnStrengthLevelChanged;
        
        private int _currentStrengthLevel = 1;
        
        public int CurrentStrengthLevel => _currentStrengthLevel;

        private float ActiveMagneticForce => polarity * _currentStrengthLevel * baseMagnetStrength;

        private Collider[] _overlappingColliders = new Collider[5];
        
        private ParticleSystem.EmissionModule _emissionModule;
        
        private void Awake() {
            if (constantFieldParticles) {
                _emissionModule = constantFieldParticles.emission;
            }
        }
        
        private void Start() {
            UpdateVisuals();
        }
    
        private void FixedUpdate() {
            ApplyMagneticForceToSurroundingRigidbodies();
        }

        private void ApplyMagneticForceToSurroundingRigidbodies() {
            if (_currentStrengthLevel == 0) return;
            
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
            UpdateVisuals();
        }
        
        private void UpdateVisuals() {
            if (!constantFieldParticles) return;

            if (_currentStrengthLevel == 0) {
                if (constantFieldParticles.isPlaying) {
                    constantFieldParticles.Stop(true);
                }
                return;
            }

            if (!constantFieldParticles.isPlaying) {
                constantFieldParticles.Play();
            }

            if (_emissionModule.enabled) {
                ParticleSystem.Burst burst = _emissionModule.GetBurst(0);
                
                burst.repeatInterval = baseEmissionInterval / _currentStrengthLevel;
                
                _emissionModule.SetBurst(0, burst);
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, magnetRange);
        }
    }
}
