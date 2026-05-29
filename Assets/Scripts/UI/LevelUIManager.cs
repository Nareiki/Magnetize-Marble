using UnityEngine;
using Events;

namespace UI {
    public class LevelUIManager : MonoBehaviour {
        
        [Header("State Channels")]
        [SerializeField] private VoidEventChannelSo simulationStartedEvent;
        [SerializeField] private VoidEventChannelSo simulationStoppedEvent;
        [SerializeField] private BoolEventChannelSo onMagnetDragStateChanged;

        [Header("Trash / Stop View")]
        [SerializeField] private UIButtonStateView trashStopButtonView;
        [SerializeField] private Sprite trashCanSprite;
        [SerializeField] private Sprite stopSimulationSprite;

        [Header("Start / Reset View")]
        [SerializeField] private UIButtonStateView startResetButtonView;
        [SerializeField] private Sprite startSimulationSprite;
        [SerializeField] private Sprite resetSimulationSprite;

        [Header("Magnet Spawn Views")]
        [Tooltip("Drag the Button Frames that have the UIButtonStateView attached here.")]
        [SerializeField] private UIButtonStateView[] magnetSpawnViews;

        [Header("Visual Configurations")]
        [SerializeField] private Color activeColor = Color.white;
        [SerializeField] private Color disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        [SerializeField] private float disabledAlpha = 0.4f;
        [SerializeField] private float enabledAlpha = 1f;
        
        private bool _isSimulationActive = false;
        
        private void OnEnable() {
            if (simulationStartedEvent) simulationStartedEvent.OnRaised += HandleSimulationStarted;
            if (simulationStoppedEvent) simulationStoppedEvent.OnRaised += HandleSimulationStopped;
            if (onMagnetDragStateChanged) onMagnetDragStateChanged.OnRaised += HandleMagnetDragStateChanged;
        }

        private void OnDisable() {
            if (simulationStartedEvent) simulationStartedEvent.OnRaised -= HandleSimulationStarted;
            if (simulationStoppedEvent) simulationStoppedEvent.OnRaised -= HandleSimulationStopped;
            if (onMagnetDragStateChanged) onMagnetDragStateChanged.OnRaised -= HandleMagnetDragStateChanged;
        }

        private void Start() {
            HandleSimulationStopped();
        }

        private void HandleSimulationStarted() {
            _isSimulationActive = true;
            
            ConfigureButtonState(trashStopButtonView, stopSimulationSprite, true, true, enabledAlpha);
            trashStopButtonView.SetDragHoverState(true, false);

            ConfigureButtonState(startResetButtonView, resetSimulationSprite, true, true, enabledAlpha);

            SetSpawnIconsState(disabledColor, false, false, disabledAlpha);
        }

        private void HandleSimulationStopped() {
            _isSimulationActive = false;
            
            ConfigureButtonState(trashStopButtonView, trashCanSprite, false, true, enabledAlpha);
            trashStopButtonView.SetDragHoverState(false, false);

            ConfigureButtonState(startResetButtonView, startSimulationSprite, true, true, enabledAlpha);

            SetSpawnIconsState(activeColor, true, true, enabledAlpha);
        }

        private void HandleMagnetDragStateChanged(bool isDragging) {
            if (_isSimulationActive) return;
            
            var targetAlpha = isDragging ? disabledAlpha : enabledAlpha;
            var allowInteraction = !isDragging;

            ConfigureButtonState(startResetButtonView, startSimulationSprite, allowInteraction, allowInteraction, targetAlpha);

            SetSpawnIconsState(activeColor, allowInteraction, allowInteraction, targetAlpha);

            if (trashStopButtonView) {
                trashStopButtonView.ToggleAnimations(isDragging);
                trashStopButtonView.SetDragHoverState(isDragging, allowAnimatorPulse: isDragging);
            }
        }
        
        private void SetSpawnIconsState(Color targetColor, bool enableAnimations, bool isInteractable, float alpha) {
            foreach (var uiButtonStateView in magnetSpawnViews) {
                if (!uiButtonStateView) continue;
                uiButtonStateView.SetIconColor(targetColor);
                uiButtonStateView.ToggleAnimations(enableAnimations);
                uiButtonStateView.SetInteractable(isInteractable, alpha);
            }
        }
        
        private void ConfigureButtonState(UIButtonStateView targetView, Sprite targetSprite, bool enableAnimation, bool isInteractable, float alpha) {
            if (!targetView) return;
            targetView.SetIconSprite(targetSprite);
            targetView.ToggleAnimations(enableAnimation);
            targetView.SetInteractable(isInteractable, alpha);
        }
    }
}