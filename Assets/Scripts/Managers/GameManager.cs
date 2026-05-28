using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Events;

namespace Managers {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance { get; private set; }

        [Header("Global Architectural Events")]
        [SerializeField] private SceneManagerChannelSo sceneLoadChannel;

        public float LoadingProgress { get; private set; } = 0f;
        private bool _isSceneLoading = false;

        private void Awake() {
            InitializeSingleton();
        }

        private void InitializeSingleton() {
            if (Instance != null && Instance != this) {
                Debug.Log($"GameManager: Duplicate manager detected on '{gameObject.scene.name}'. Self-destructing to preserve original instance.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable() {
            if (Instance == this && sceneLoadChannel != null) {
                sceneLoadChannel.OnSceneLoadRequested += HandleSceneLoadRequest;
            }
        }

        private void OnDisable() {
            if (Instance == this && sceneLoadChannel != null) {
                sceneLoadChannel.OnSceneLoadRequested -= HandleSceneLoadRequest;
            }
        }

        private void HandleSceneLoadRequest(string sceneName) {
            if (string.IsNullOrEmpty(sceneName)) {
                Debug.LogWarning("GameManager: Received an empty or null scene name request.");
                return;
            }

            if (_isSceneLoading) {
                Debug.LogWarning($"GameManager: Already loading a scene. Ignored request for '{sceneName}'.");
                return;
            }

            StartCoroutine(LoadSceneAsyncRoutine(sceneName));
        }

        private IEnumerator LoadSceneAsyncRoutine(string sceneName) {
            _isSceneLoading = true;
            LoadingProgress = 0f;

            Debug.Log($"GameManager: Starting Asynchronous load for '{sceneName}'...");
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            
            if (asyncLoad != null) {
                asyncLoad.allowSceneActivation = true;

                while (!asyncLoad.isDone) {
                    LoadingProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                    yield return null;
                }
            }

            Debug.Log($"GameManager: Successfully loaded scene '{sceneName}'.");
            _isSceneLoading = false;
            LoadingProgress = 1f;
        }

        public void ExitApplication() {
            Debug.Log("Exiting Application...");
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}