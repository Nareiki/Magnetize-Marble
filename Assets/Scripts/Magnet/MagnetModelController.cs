using System;
using UnityEngine;

namespace Magnet {
    public class MagnetModelController : MonoBehaviour {
        [Header("Logic Connection")] 
        [SerializeField] private MagnetController magnetBrain;
        
        [Header("Model Objects")] 
        [Tooltip("Index 0 = Deactivated, 1 = Mode 1, 2 = Mode 2, 3 = Mode 3")]
        [SerializeField] private GameObject[] strengthModes = new GameObject[4];

        [Header("Switch model")] 
        [SerializeField] private Transform switchModel;
        [Tooltip("The Y-axis rotation angles for Mode 0, 1, 2 and 3")] 
        
        [SerializeField] private float[] switchAnglesY = new float[] { 87f, 0f, -95f, 177f };

        [Header("Materials")] 
        [SerializeField] private Material activatedMaterial;
        [SerializeField] private Material defaultMaterial;
        
        private MeshRenderer[] cachedRenderers;

        private void Awake() {
            cachedRenderers = new MeshRenderer[strengthModes.Length];

            for (var i = 0; i < strengthModes.Length; i++) {
                if (strengthModes[i] != null) {
                    
                    cachedRenderers[i] = strengthModes[i].GetComponent<MeshRenderer>();
                } else {
                    cachedRenderers[i] = new MeshRenderer(); 
                }
            }
        }

        private void OnEnable() {
            if (magnetBrain) {
                magnetBrain.OnStrengthLevelChanged += UpdateVisualState;
            }
        }
        
        private void OnDisable() {
            if (magnetBrain) {
                magnetBrain.OnStrengthLevelChanged -= UpdateVisualState;
            }
        }

        private void Start() {
            if (magnetBrain) {
                UpdateVisualState(magnetBrain.CurrentStrengthLevel);
            }
        }

        private void UpdateVisualState(int newStrengthLevel) {
            for (var i = 0; i < strengthModes.Length; i++) {
                if (strengthModes[i] == null) continue;
                
                var isTheActiveMode = i == newStrengthLevel;
                var targetMaterial = isTheActiveMode ? activatedMaterial : defaultMaterial;
                cachedRenderers[i].sharedMaterial = targetMaterial;
            }

            if (switchModel && newStrengthLevel < switchAnglesY.Length) {
                var currentRotation = switchModel.localEulerAngles;
                switchModel.localEulerAngles = new Vector3(currentRotation.x, switchAnglesY[newStrengthLevel], currentRotation.z);
            }
        }
    }
}