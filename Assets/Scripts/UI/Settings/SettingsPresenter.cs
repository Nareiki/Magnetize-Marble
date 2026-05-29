using UnityEngine;

namespace UI.Settings {
    public class SettingsPresenter : MonoBehaviour {
        [Header("Architecture Layer Binds")]
        [SerializeField] private SettingsMenuView view;

        private const string VolumeKey = "Settings_Volume";
        private const string ScreenModeKey = "Settings_ScreenMode";

        private void Awake() {
            if (view == null) view = GetComponent<SettingsMenuView>();
            LoadAndApplySavedSettings();
        }

        private void OnEnable() {
            if (!view) return;
            view.OnVolumeChanged += HandleVolumeChanged;
            view.OnScreenModeChanged += HandleScreenModeChanged;
        }

        private void OnDisable() {
            if (!view) return;
            view.OnVolumeChanged -= HandleVolumeChanged;
            view.OnScreenModeChanged -= HandleScreenModeChanged;
        }

        private void LoadAndApplySavedSettings() {
            float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1.0f);
            int savedScreenMode = PlayerPrefs.GetInt(ScreenModeKey, 2);

            if (view) {
                view.SetVisualStates(savedVolume, savedScreenMode);
            }

            ApplyVolumeToEngine(savedVolume);
            ApplyScreenModeToEngine(savedScreenMode);
        }

        private void HandleVolumeChanged(float newVolume) {
            PlayerPrefs.SetFloat(VolumeKey, newVolume);
            ApplyVolumeToEngine(newVolume);
            PlayerPrefs.Save();
        }

        private void HandleScreenModeChanged(int index) {
            PlayerPrefs.SetInt(ScreenModeKey, index);
            ApplyScreenModeToEngine(index);
            PlayerPrefs.Save();
        }

        private void ApplyVolumeToEngine(float volume) {
            AudioListener.volume = volume; 
        }

        private void ApplyScreenModeToEngine(int screenModeIndex) {
            switch (screenModeIndex) {
                case 0:
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    Screen.fullScreen = true;
                    Debug.Log("Screen Mode Applied: Exclusive Full Screen");
                    break;
                case 1:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    Screen.fullScreen = false;
                    Debug.Log("Screen Mode Applied: Windowed");
                    break;
                case 2:
                default:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    Screen.fullScreen = true;
                    Debug.Log("Screen Mode Applied: Full Screen Windowed (Borderless)");
                    break;
            }
        }
    }
}