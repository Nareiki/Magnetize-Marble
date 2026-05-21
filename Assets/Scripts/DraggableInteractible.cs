

using System;
using Events;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableInteractible : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [Header("Drag Feel")]
    [Tooltip("How fast the magnet will follow the mouse. Higher values will feel more 'snappy', while lower values will feel more 'floaty'.")]
    [SerializeField] private float dragSpeed = 10f;
    [Tooltip("Pushes the magnet UP on the screen, to prevent it from being obscured by the mouse cursor. Higher values will push the magnet higher up.")]
    [SerializeField] private Vector2 cursorOffset = new Vector2(0f, 1.5f);

    [Header("Events")] [SerializeField] private GameObjectEventChannelSo onMagnetRemoved;

    
    private bool _isDragging = false;
    private Vector3 _targetWorldPosition;
    
    void Start() {
        _targetWorldPosition = transform.position;
    }

    private void Update() {
        if (_isDragging) {
            transform.position = Vector3.Lerp(transform.position, _targetWorldPosition, dragSpeed * Time.deltaTime);
        }
    }


    public void OnBeginDrag(PointerEventData eventData) {
        _isDragging = true;
        //_startPosition = transform.position;
        //TODO: Sound Effects could go here (like a 'pickup' sound)
    }
    
    public void OnDrag(PointerEventData eventData) {
        var offsetTouch = eventData.position + cursorOffset;
        
        var mm = FindFirstObjectByType<MagnetsManager>();
        if (mm == null) return;

        var calculatedPos = mm.GetClampedWorldPos(offsetTouch);
        if (calculatedPos != Vector3.zero)
            _targetWorldPosition = calculatedPos;
        
        if (calculatedPos != Vector3.zero) {
            _targetWorldPosition = calculatedPos;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData) {
        _isDragging = false;

        // Snapping to grid
        // float snapX = Mathf.Round(_targetWorldPosition.x);
        // float snapY = Mathf.Round(_targetWorldPosition.y);
        //
        // transform.position = new Vector3(snapX, snapY, _targetWorldPosition.z);
        
        transform.position = _targetWorldPosition;
        
        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("RemoveInteractable")) {
            onMagnetRemoved?.Raise(gameObject);
        }
        else {
            //TODO: Sound Effects could go here (like a 'drop' sound)
            //transform.position = _startPosition;
        }
    }
}