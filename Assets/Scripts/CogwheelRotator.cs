using UnityEngine;

public class CogwheelRotator : MonoBehaviour {
    [Header("Rotation Settings")]
    [Tooltip("Rotationsgeschwindigkeit in Grad pro Sekunde")]
    public float rotationSpeed = 90f;

    [Tooltip("Richtung umkehren (Clockwise/Counter-Clockwise)")]
    public bool reverseDirection = false;

    [Tooltip("Rotation aktiviert/deaktiviert")]
    public bool isRotating = true;

    [Header("Optional")]
    [Tooltip("Lokale Rotation (true) oder Welt-Rotation (false)")]
    public bool useLocalSpace = true;

    void Update() {
        if (!isRotating) return;

        float direction = reverseDirection ? -1f : 1f;
        float rotationThisFrame = rotationSpeed * direction * Time.deltaTime;

        Space space = useLocalSpace ? Space.Self : Space.World;
        transform.Rotate(0f, rotationThisFrame, 0f, space);
    }
}