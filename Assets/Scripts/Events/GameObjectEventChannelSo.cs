using System;
using UnityEngine;

namespace Events {
    [CreateAssetMenu(menuName = "Events/GameObjectEventChannelSo")]
    public class GameObjectEventChannelSo : ScriptableObject {
        public event Action<GameObject> OnRaised;

        public void Raise(GameObject obj) {
            OnRaised?.Invoke(obj);
        }
    }
}