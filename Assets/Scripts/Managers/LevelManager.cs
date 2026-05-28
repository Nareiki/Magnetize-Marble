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
        [SerializeField] private VoidEventChannelSo simulationStartedEvent;
        [SerializeField] private VoidEventChannelSo simulationStoppedEvent;
        [SerializeField] private VoidEventChannelSo resetZoneEvent;

        [Header("Optional Config")] 
        [SerializeField] private CubeLevelSo levelConfig;

        [Header("Cube References")] 
        [SerializeField] private Transform wholeCube;
        [SerializeField] private List<GameObject> levelBases;
        [SerializeField] private List<GameObject> modelBases;
        //[SerializeField] private List<BoxCollider> dragZones;
        //[SerializeField] private List<Transform> faceStartPoints;

        [Header("Managers & Actors")]
        [SerializeField] private MagnetsManager magnetManager;
        [SerializeField] private MarbleController marbleController;
    
        [Header("Animation Settings")]
        [SerializeField] private float rotationDuration = 1.5f;
        [SerializeField] private AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
        private int _currentFaceIndex;
        private bool _isRotating;
        private bool _isSimulating ;
    
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

            if (resetZoneEvent != null) {
                resetZoneEvent.OnRaised += StopSimulation;
            }
        }
    
        private void OnDisable() {
            if (goalReachedEvent != null) {
                goalReachedEvent.OnRaised -= HandleGoalReached;
            }
            
            if (resetZoneEvent != null) {
                resetZoneEvent.OnRaised -= StopSimulation;
            }
        }

        private void Start() {
            BuildRuntimeFaces();

            if (marbleController) {
                marbleController.gameObject.SetActive(false);
            }
            
            InitializeFaceVisibility();
        
            // Define starting bounds
            if (magnetManager && _faces.Count > 0) {
                magnetManager.SetDragZone(_faces[_currentFaceIndex].dragZone);
                magnetManager.UpdateZDepth(_currentFaceIndex);
            }
        }

        private void BuildRuntimeFaces() {
            _faces.Clear();
        
            foreach (var baseObj in levelBases) {
                if (baseObj == null) continue;
            
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

            if (_currentFaceIndex >= _faces.Count) {
                return;
            }
        
            _isSimulating = true;
        
            marbleController.gameObject.SetActive(true);
            marbleController.LaunchFrom(_faces[_currentFaceIndex].startPoint);
            
            simulationStartedEvent?.Raise();
        }
        
        [ContextMenu("Stop Simulation")]
        public void StopSimulation() {
            if (!_isSimulating) {
                return;
            }
            
            _isSimulating = false;
            
            if (marbleController) {
                marbleController.gameObject.SetActive(false);
            }
            
            simulationStoppedEvent?.Raise();
        }
        
        private void InitializeFaceVisibility() {
            for (var i = 0; i < _faces.Count; i++) {
                if (_faces[i].baseObject != null) {
                    // Only face 0 starts enabled; others are completely culled
                    _faces[i].baseObject.SetActive(i == 0);
                    modelBases[i].SetActive(i == 0);
                }
            }
        }

        private void HandleGoalReached() {
            CompleteLevelAndRotate();
        }

        [ContextMenu("Complete Level & Rotate")]
        private void CompleteLevelAndRotate() {
            if (_isRotating) {
                return;
            }
            
            StopSimulation();
            
            int oldFaceIndex = _currentFaceIndex;

            _currentFaceIndex++;

            if (_currentFaceIndex < _faces.Count && magnetManager) {
                StartCoroutine(RotateCubeRoutine(oldFaceIndex));
            }
            else {
                Debug.Log("Escape Room Completed!");
                
                simulationStoppedEvent?.Raise();
            }
        }
        
        private IEnumerator RotateCubeRoutine(int previousFaceIndex) {
            _isRotating = true;

            if (magnetManager) {
                magnetManager.ClearAllMagnets();
            }
            
            if (_currentFaceIndex < _faces.Count) {
                _faces[_currentFaceIndex].baseObject.SetActive(true);
                modelBases[_currentFaceIndex].SetActive(true);
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
            
            if (previousFaceIndex >= 0 && previousFaceIndex < _faces.Count) {
                _faces[previousFaceIndex].baseObject.SetActive(false);
                modelBases[previousFaceIndex].SetActive(false);
            }

            if (_currentFaceIndex < _faces.Count && magnetManager) {
                magnetManager.SetDragZone(_faces[_currentFaceIndex].dragZone);
                magnetManager.UpdateZDepth(_currentFaceIndex);
            }

            _isRotating = false;
        }
    }
}