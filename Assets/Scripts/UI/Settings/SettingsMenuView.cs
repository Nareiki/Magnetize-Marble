using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Settings {
    public class SettingsMenuView : MonoBehaviour {
        [Header("UI Component Elements")]
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private TMP_Dropdown screenModeDropdown;

        [Header("Text Display Elements")]
        [Tooltip("The TMPro Text component that will display the 0-100 volume readout.")]
        [SerializeField] private TextMeshProUGUI volumeTextLabel;
        
        public UnityAction<float> OnVolumeChanged;
        public UnityAction<int> OnScreenModeChanged;

        private void Start() {
            if (volumeSlider) {
                volumeSlider.onValueChanged.AddListener(val => {
                    UpdateVolumeTextDisplay(val);
                    OnVolumeChanged?.Invoke(val);
                });
            }
            if (screenModeDropdown) screenModeDropdown.onValueChanged.AddListener(val => OnScreenModeChanged?.Invoke(val));
        }

        public void SetVisualStates(float volume, int screenModeIndex) {
            if (volumeSlider) volumeSlider.SetValueWithoutNotify(volume);
            if (screenModeDropdown) screenModeDropdown.SetValueWithoutNotify(screenModeIndex);
        }
        
        private void UpdateVolumeTextDisplay(float rawSliderValue) {
            if (!volumeTextLabel) return;

            int percentageVolume = Mathf.RoundToInt(rawSliderValue * 100f);
            
            volumeTextLabel.text = percentageVolume.ToString();
        }
    }
}