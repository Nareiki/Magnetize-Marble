using System;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Events {
    [CreateAssetMenu(menuName = "Events/Vector3EventChannelSo")]
    public class Vector3EventChannelSo :  ScriptableObject {
        public event Action<Vector3> OnRaised;

        public void Raise(Vector3 value) {
            OnRaised?.Invoke(value);
        }
    }
}