using System;
using UnityEngine;

namespace Events {
    [CreateAssetMenu(menuName = "Events/VoidEventChannelSo")]
    public class VoidEventChannelSo : ScriptableObject {
        public event Action OnRaised;

        public void Raise() {
            OnRaised?.Invoke();
        }
    }
}