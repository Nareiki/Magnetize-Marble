using System.Collections.Generic;
using Events;
using Magnet;
using UnityEngine;

namespace Managers {
    public class MagnetsManager : MonoBehaviour {
        [Header("Events")] [SerializeField] private GameObjectEventChannelSo onMagnetCreated;
        [SerializeField] private GameObjectEventChannelSo onMagnetRemoved;
        [SerializeField] private VoidEventChannelSo magnetsClearedEvent;
    
        [Header("Placement Settings")]
        [Tooltip("How far from the center of the world should the magnets sit?")]
        [SerializeField] private float zDepth = -9.9f;
        [SerializeField] private GameObject magnetRootComponent;
    
        private BoxCollider _activeLevelBounds;
        private Camera _mainCam;
    
        private List<GameObject> _activeMagnets = new List<GameObject>();

        private void OnEnable() {
            if (!onMagnetCreated) return;
            onMagnetCreated.OnRaised += HandleMagnetCreated;
        
            if (!onMagnetRemoved) return;
            onMagnetRemoved.OnRaised += HandleMagnetRemoved;
        }
    
        private void OnDisable() {
            if (!onMagnetCreated) return;
            onMagnetCreated.OnRaised -= HandleMagnetCreated;
        
            if (!onMagnetRemoved) return;
            onMagnetRemoved.OnRaised -= HandleMagnetRemoved;
        }

        private void Start() {
            _mainCam = Camera.main;
            if (!magnetRootComponent) return;
        
            foreach (var child in magnetRootComponent.GetComponentsInChildren<MagnetController>()) {
                _activeMagnets.Add(child.gameObject);
            }
        }

        private void HandleMagnetCreated(GameObject magnet) {
            RegisterMagnet(magnet);
        }

        private void HandleMagnetRemoved(GameObject magnet) {
            RemoveMagnet(magnet);
        }
    

        public void SetDragZone(BoxCollider newBounds) {
            _activeLevelBounds = newBounds;
        }

        public void ClearAllMagnets() {
            for (var i = _activeMagnets.Count - 1; i >= 0; i--) {
                if (_activeMagnets[i]) {
                    Destroy( _activeMagnets[i] );
                }
            }
            _activeMagnets.Clear();
            magnetsClearedEvent?.Raise();
        }
    
        public void RegisterMagnet(GameObject magnet) {
            magnet.transform.SetParent(magnetRootComponent.transform);
        
            if (!_activeMagnets.Contains(magnet))
            {
                _activeMagnets.Add(magnet);
            }
        }

        public void RemoveMagnet(GameObject magnet) {
            if (magnet == null) return;
        
            if (!_activeMagnets.Contains(magnet)) return;
        
            _activeMagnets.Remove(magnet);
            Destroy(magnet);
        }

        public Vector3 GetClampedWorldPos(Vector2 screenPosition)
        {
            if (!_activeLevelBounds || !_mainCam) {
                return Vector3.zero;
            }
        
            var placementPlane = new Plane(Vector3.forward, new Vector3(0, 0, zDepth));
            var ray = _mainCam.ScreenPointToRay(screenPosition);

            if (placementPlane.Raycast(ray, out float distance))
            {
                var worldPoint = ray.GetPoint(distance);

                var b = _activeLevelBounds.bounds;
                worldPoint.x = Mathf.Clamp(worldPoint.x, b.min.x, b.max.x);
                worldPoint.y = Mathf.Clamp(worldPoint.y, b.min.y, b.max.y);
                worldPoint.z = Mathf.Clamp(worldPoint.z, b.min.z, b.max.z);
            
                return worldPoint;
            }
            return Vector3.zero;
        }
    }
}