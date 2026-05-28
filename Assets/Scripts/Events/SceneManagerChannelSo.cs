using UnityEngine;

namespace Events {
    [CreateAssetMenu(menuName = "Events/Scene Manager Channel")]
    public class SceneManagerChannelSo : ScriptableObject {
        public System.Action<string> OnSceneLoadRequested;

        public void RequestSceneLoad(string sceneName) {
            OnSceneLoadRequested?.Invoke(sceneName);
        }
    }
}