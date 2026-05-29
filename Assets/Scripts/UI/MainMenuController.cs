using UnityEngine;
using Events;

namespace UI {
    public class MainMenuController : MonoBehaviour {

        [Header("Scene Event Channels")]
        [SerializeField] private SceneManagerChannelSo sceneLoadChannel;
        
        private Managers.GameManager _cachedGameManager;

        private void Awake() {
            _cachedGameManager = FindFirstObjectByType<Managers.GameManager>();
        }

        public void LoadTargetScene(string sceneName) {
            if (sceneLoadChannel != null) {
                sceneLoadChannel.RequestSceneLoad(sceneName);
            } else {
                Debug.LogError("MainMenuController: SceneLoadChannelSo reference is missing!");
            }
        }

        public void OnExitButtonClicked() {
            if (Managers.GameManager.Instance != null) {
                Managers.GameManager.Instance.ExitApplication();
            } else {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
            }
        }
    }
}