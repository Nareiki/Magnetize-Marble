using System.Collections;
using System.Collections.Generic;
using Events;
using Helpers;
using Marble;
using UnityEngine;

namespace Managers {
    public class LevelManager : MonoBehaviour {
    
        [Header("Events")]
        [SerializeField] private VoidEventChannelSo goalReachedEvent;

        [Header("Optional Config")] 
        [SerializeField] private CubeLevelSo levelConfig;

        [Header("Cube References")] 
        [SerializeField] private Transform wholeCube;
        [SerializeField] private List<GameObject> levelBases;
        //[SerializeField] private List<BoxCollider> dragZones;
        //[SerializeField] private List<Transform> faceStartPoints;

        [Header("Managers & Actors")]
        [SerializeField] private MagnetsManager magnetManager;
        [SerializeField] private MarbleController marbleController;
    
        [Header("Animation Settings")]
        [SerializeField] private float rotationDuration = 1.5f;
        [SerializeField] private AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
        private int _currentFaceIndex = 0;
        private bool _isRotating = false;
    
        private float RotationAngle => levelConfig != null ? levelConfig.rotationAngle : 0;

        private class FaceRuntimeData {
            public BoxCollider dragZone;
            public Transform startPoint;
            public GameObject baseObject;
        }
    
        private readonly List<FaceRuntimeData> _faces = new List<FaceRuntimeData>();

        private void OnEnable() {
            if (goalReachedEvent != null) {
                goalReachedEvent.OnRaised += HandleGoalReached;
            }
        }
    
        private void OnDisable() {
            if (goalReachedEvent != null) {
                goalReachedEvent.OnRaised -= HandleGoalReached;
            }
        }

        private void Start() {
            BuildRuntimeFaces();

            if (marbleController) {
                marbleController.gameObject.SetActive(false);
            }
        
            // Define starting bounds
            if (magnetManager && _faces.Count > 0) {
                magnetManager.SetDragZone(_faces[_currentFaceIndex].dragZone);
            }
        }

        private void BuildRuntimeFaces() {
            _faces.Clear();
        
            foreach (var baseObj in levelBases) {
                if (baseObj == null) return;
            
                var dragZone = baseObj.GetComponentInChildren<DragZone>();
                var startPoint = baseObj.GetComponentInChildren<StartPoint>();
            
                if (dragZone == null || startPoint == null) {
                    Debug.LogError($"Base object {baseObj.name} is missing required components.");
                    continue;
                }
            
                var boxCollider = dragZone.GetComponent<BoxCollider>();
                if (boxCollider == null) {
                    Debug.LogError($"Base object {baseObj.name} is missing required components.");
                    continue;
                }
            
                _faces.Add(new FaceRuntimeData {
                    dragZone = boxCollider,
                    startPoint = startPoint.transform,
                    baseObject = baseObj
                });
            
            }
        }
    
        [ContextMenu("Begin Simulation")]
        public void StartSimulation() {
            if (_isRotating || !marbleController) {
                return;
            }

            if (_currentFaceIndex >= _faces.Count) return;
        
        
        
            marbleController.gameObject.SetActive(true);
            marbleController.LaunchFrom(_faces[_currentFaceIndex].startPoint);
        }
        [ContextMenu("Stop Simulation")]
        public void StopSimulation() {
            if (marbleController) {
                marbleController.gameObject.SetActive(false);
            }
        }

        private void HandleGoalReached() {
            CompleteLevelAndRotate();
        }

        [ContextMenu("Complete Level & Rotate")]
        public void CompleteLevelAndRotate() {
            if (_isRotating) {
                return;
            }

            _currentFaceIndex++;

            if (_currentFaceIndex < levelBases.Count) {
                StartCoroutine(RotateCubeRoutine());
            }
            else {
                Debug.Log("Escape Room Completed!");
            }
        }

        private IEnumerator RotateCubeRoutine() {
            _isRotating = true;

            if (magnetManager) {
                magnetManager.ClearAllMagnets();
            }
        
            var startRotation = wholeCube.rotation;
            var endRotation = startRotation * Quaternion.Euler(0, RotationAngle, 0);
        
            var elapsedTime = 0f;
        
            while (elapsedTime < rotationDuration) {
                elapsedTime += Time.deltaTime;
            
                var t = Mathf.Clamp01(elapsedTime / rotationDuration);
                var curveValue = rotationCurve.Evaluate(t);
            
                wholeCube.rotation = Quaternion.Slerp(startRotation, endRotation, curveValue);
            
                yield return null;
            }

            wholeCube.rotation = endRotation;

            if (_currentFaceIndex < _faces.Count && magnetManager) {
                magnetManager.SetDragZone(_faces[_currentFaceIndex].dragZone);
            }

            _isRotating = false;
        }
    }
}