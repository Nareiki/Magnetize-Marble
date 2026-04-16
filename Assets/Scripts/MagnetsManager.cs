using System.Collections.Generic;
using UnityEngine;

public class MagnetsManager : MonoBehaviour
{
    [Header("Placement Settings")]
    [Tooltip("How far from the center of the world should the magnets sit?")]
    [SerializeField] private float zDepth = -9.9f;
    
    private BoxCollider _activeLevelBounds;
    private Camera _mainCam;
    
    private List<GameObject> _activeMagnets = new List<GameObject>();

    void Start() {
        _mainCam = Camera.main;
    }

    public void SetNewBounds(BoxCollider newBounds) {
        _activeLevelBounds = newBounds;
    }

    public void ClearAllMagnets() {
        for (int i = _activeMagnets.Count - 1; i >= 0; i--) {
            if (_activeMagnets[i]) {
                Destroy( _activeMagnets[i] );
            }
        }
        _activeMagnets.Clear();
    }
    
    public void RegisterMagnet(GameObject magnet) {
        if (!_activeMagnets.Contains(magnet))
        {
            _activeMagnets.Add(magnet);
        }
    }

    public void RemoveMagnet(GameObject magnet) {
        if (!_activeMagnets.Contains(magnet)) return;
        
        _activeMagnets.Remove(magnet);
        Destroy(magnet);
    }

    public Vector3 GetClampedWorldPos(Vector2 screenPosition)
    {
        if (!_activeLevelBounds || !_mainCam) {
            return Vector3.zero;
        }
        
        var placementPlane = new Plane(Vector3.forward, new Vector3(0, 0, zDepth));
        var ray = _mainCam.ScreenPointToRay(screenPosition);

        if (placementPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);

            Bounds b = _activeLevelBounds.bounds;
            worldPoint.x = Mathf.Clamp(worldPoint.x, b.min.x, b.max.x);
            worldPoint.y = Mathf.Clamp(worldPoint.y, b.min.y, b.max.y);
            worldPoint.z = Mathf.Clamp(worldPoint.z, b.min.z, b.max.z);
            
            return worldPoint;
        }
        return Vector3.zero;
    }
}