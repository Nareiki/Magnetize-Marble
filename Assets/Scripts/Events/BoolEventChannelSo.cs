using UnityEngine;

namespace Events {
    [CreateAssetMenu(menuName = "Events/Bool Event Channel")]
    public class BoolEventChannelSo : ScriptableObject {
        public System.Action<bool> OnRaised;

        public void Raise(bool value) {
            OnRaised?.Invoke(value);
        }
    }
}