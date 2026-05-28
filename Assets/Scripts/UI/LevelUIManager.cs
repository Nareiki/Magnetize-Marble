using UnityEngine;
using Events;

namespace UI {
    public class LevelUIManager : MonoBehaviour {
        
        [Header("State Channels")]
        [SerializeField] private VoidEventChannelSo simulationStartedEvent;
        [SerializeField] private VoidEventChannelSo simulationStoppedEvent;

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

        private void OnEnable() {
            if (simulationStartedEvent) simulationStartedEvent.OnRaised += HandleSimulationStarted;
            if (simulationStoppedEvent) simulationStoppedEvent.OnRaised += HandleSimulationStopped;
        }

        private void OnDisable() {
            if (simulationStartedEvent) simulationStartedEvent.OnRaised -= HandleSimulationStarted;
            if (simulationStoppedEvent) simulationStoppedEvent.OnRaised -= HandleSimulationStopped;
        }

        private void Start() {
            HandleSimulationStopped();
        }

        private void HandleSimulationStarted() {
            if (trashStopButtonView) {
                trashStopButtonView.SetIconSprite(stopSimulationSprite);
                trashStopButtonView.ToggleAnimations(true); // Animates when simulation runs
            }

            if (startResetButtonView) {
                startResetButtonView.SetIconSprite(resetSimulationSprite);
            }

            SetSpawnIconsState(disabledColor, false);
        }

        private void HandleSimulationStopped() {
            if (trashStopButtonView) {
                trashStopButtonView.SetIconSprite(trashCanSprite);
                trashStopButtonView.ToggleAnimations(false); 
            }

            if (startResetButtonView) {
                startResetButtonView.SetIconSprite(startSimulationSprite);
            }

            SetSpawnIconsState(activeColor, true);
        }

        private void SetSpawnIconsState(Color targetColor, bool enableAnimations) {
            foreach (var uiButtonStateView in magnetSpawnViews) {
                if (!uiButtonStateView) continue;
                uiButtonStateView.SetIconColor(targetColor);
                uiButtonStateView.ToggleAnimations(enableAnimations);
            }
        }
    }
}