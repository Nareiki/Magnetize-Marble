using UnityEngine;
using Events;

namespace UI {
    public class ReturnToMenuController : MonoBehaviour {

        [Header("Scene Event Channels")]
        [SerializeField] private SceneManagerChannelSo sceneLoadChannel;
        
        [Header("Target Configuration")]
        [SerializeField] private string mainMenuSceneName = "MainMenuScene";

        public void ReturnToMainMenu() {
            if (sceneLoadChannel != null) {
                sceneLoadChannel.RequestSceneLoad(mainMenuSceneName);
            }
        }
    }
}