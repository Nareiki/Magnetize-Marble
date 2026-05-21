using Events;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInteractableButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [Header("References")] 
    [SerializeField] private GameObject magnetPrefab;

    [Header("Events")] 
    [SerializeField] private GameObjectEventChannelSo onMagnetCreated;
    [SerializeField] private GameObjectEventChannelSo onMagnetRemoved;
    
    private GameObject _activeMagnet;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _activeMagnet = Instantiate(magnetPrefab);
        onMagnetCreated?.Raise(_activeMagnet);
        UpdateMagnetPosition(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_activeMagnet)
        {
            UpdateMagnetPosition(eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!_activeMagnet) return; 
        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("RemoveInteractable"))
        {
            onMagnetRemoved?.Raise(_activeMagnet);
        }
        
        _activeMagnet = null;
    }

    private void UpdateMagnetPosition(Vector2 screenPos)
    {
        var magnetManager = FindFirstObjectByType<MagnetsManager>();
        if (magnetManager == null) return;

        var newPosition = magnetManager.GetClampedWorldPos(screenPos);
        if (newPosition != Vector3.zero && _activeMagnet)
        {
            _activeMagnet.transform.position = newPosition;
        }
    }
}