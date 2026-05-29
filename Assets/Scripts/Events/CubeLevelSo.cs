using System.Collections.Generic;
using UnityEngine;

namespace Events {
    [CreateAssetMenu(menuName = "Magnetize/Config/Cube Level")]
    public class CubeLevelSo : ScriptableObject
    {
        [Tooltip("Rotation angle per face (degrees)")]
        public float rotationAngle = 90f;

        [Tooltip("Optional list describing face ordering or IDs. Scene provides the actual StartPoint transforms.")]
        public List<string> faceIds = new List<string>();
    }
}