using System;
using System.Collections;
using System.Collections.Generic;
using Marble;
using UnityEngine;

public class CubeManager : MonoBehaviour {

    [Header("Cube References")] 
    [SerializeField] private Transform wholeCube;
    [SerializeField] private List<BoxCollider> faceBounds;
    
    [SerializeField] private List<Transform> faceStartPoints;

    [Header("Animation Settings")]
    [SerializeField] private float rotationDuration = 1.5f;
    [SerializeField] private AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Managers & Actors")]
    [SerializeField] private MagnetsManager magnetManager;
    [SerializeField] private MarbleController marbleController;
    
    private int _currentFaceIndex = 0;
    private bool _isRotating = false;

    private void Start() {
        if (faceBounds == null) {
            faceBounds = new List<BoxCollider>();
        }
        
        if (faceBounds.Count > 0 && marbleController && faceStartPoints.Count > 0) {
            magnetManager.SetNewBounds(faceBounds[_currentFaceIndex]);
        }
        marbleController.gameObject.SetActive(false);
    }
    
    [ContextMenu("Begin Simulation")]
    public void StartSimulation() {
        if (_isRotating || !marbleController) {
            return;
        }

        if (_currentFaceIndex < faceStartPoints.Count) {
            marbleController.gameObject.SetActive(true);
            marbleController.LaunchFrom(faceStartPoints[_currentFaceIndex]);
        }
    }
    [ContextMenu("Stop Simulation")]
    public void StopSimulation() {
        if (marbleController) {
            marbleController.LaunchFrom(faceStartPoints[_currentFaceIndex]);
            marbleController.gameObject.SetActive(false);
        }
    }

    [ContextMenu("Complete Level & Rotate")]
    public void CompleteLevelAndRotate() {
        if (_isRotating) {
            return;
        }

        _currentFaceIndex++;

        if (_currentFaceIndex < 4) {
            StartCoroutine(RotateCubeRoutine());
        }
        else {
            Debug.Log("Escape Room Completed!");
        }
    }

    private IEnumerator RotateCubeRoutine() {
        _isRotating = true;

        magnetManager.ClearAllMagnets();
        
        var startRotation = wholeCube.rotation;
        var endRotation = startRotation * Quaternion.Euler(0, 90f, 0);
        
        var elapsedTime = 0f;
        
        while (elapsedTime < rotationDuration) {
            elapsedTime += Time.deltaTime;
            
            var t = elapsedTime / rotationDuration;
            var curveValue = rotationCurve.Evaluate(t);
            
            wholeCube.rotation = Quaternion.Slerp(startRotation, endRotation, curveValue);
            
            yield return null;
        }

        wholeCube.rotation = endRotation;
        
        magnetManager.SetNewBounds(faceBounds[_currentFaceIndex]);

        _isRotating = false;
    }
}