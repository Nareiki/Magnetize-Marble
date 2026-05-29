using System.Collections.Generic;
using Events;
using UnityEngine;

namespace Managers {
    public class MagnetsManager : MonoBehaviour {
        [Header("Events")] [SerializeField] private GameObjectEventChannelSo onMagnetCreated;
        [SerializeField] private GameObjectEventChannelSo onMagnetRemoved;
        [SerializeField] private VoidEventChannelSo magnetsClearedEvent;
    
        [Header("Placement Settings")]
        [Tooltip("How far from the center of the world should the magnets sit?")]
        [SerializeField] private float[] zDepth = {-9.9f, -9f, -9.9f, -9f};
        [SerializeField] private GameObject magnetRootComponent;
    
        private BoxCollider _activeLevelBounds;
        private Camera _mainCam;
    
        private List<GameObject> _activeMagnets = new List<GameObject>();

        private Plane _placementPlane;
        
        [SerializeField] private float _currentZDepth;

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
            
            _placementPlane = new Plane(Vector3.forward, new Vector3(0, 0, _currentZDepth));
            
            if (!magnetRootComponent) return;
        
            foreach (Transform child in magnetRootComponent.transform) {
                if (!_activeMagnets.Contains(child.gameObject))
                {
                    _activeMagnets.Add(child.gameObject);
                }
            }
        }

        private void HandleMagnetCreated(GameObject magnet) {
            RegisterMagnet(magnet);
        }

        private void HandleMagnetRemoved(GameObject magnet) {
            RemoveMagnet(magnet);
        }

        public void UpdateZDepth(int newZDepth) {
            if (newZDepth < zDepth.Length) {
                _currentZDepth = zDepth[newZDepth];
                _placementPlane = new Plane(Vector3.forward, new Vector3(0, 0, _currentZDepth));
            }
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
        
            var ray = _mainCam.ScreenPointToRay(screenPosition);

            if (_placementPlane.Raycast(ray, out var distance))
            {
                var worldPoint = ray.GetPoint(distance);

                var bounds = _activeLevelBounds.bounds;
                worldPoint.x = Mathf.Clamp(worldPoint.x, bounds.min.x, bounds.max.x);
                worldPoint.y = Mathf.Clamp(worldPoint.y, bounds.min.y, bounds.max.y);
                worldPoint.z = Mathf.Clamp(worldPoint.z, bounds.min.z, bounds.max.z);
            
                return worldPoint;
            }
            return Vector3.zero;
        }
        
        public void UpdateObjectPositionToCursor(GameObject obj, Vector2 screenPosition, Vector2 pixelOffset, bool instantSnap)
        {
            var combinedCoords = screenPosition + pixelOffset;
            var targetWorldPos = GetClampedWorldPos(combinedCoords);

            if (targetWorldPos != Vector3.zero)
            {
                if (instantSnap) {
                    obj.transform.position = targetWorldPos;
                }
            }
        }
    }
}