using Events;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Interactables {
    public class DraggableInteractible : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        [Header("Drag Feel")]
        [Tooltip("How fast the magnet will follow the mouse. Higher values will feel more 'snappy', while lower values will feel more 'floaty'.")]
        [SerializeField] private float dragSpeed = 10f;
        [Tooltip("Pushes the magnet UP on the screen, to prevent it from being obscured by the mouse cursor. Higher values will push the magnet higher up.")]
        [SerializeField] private Vector2 cursorOffset = new Vector2(0f, 1.5f);

        [Header("Events")] 
        [SerializeField] private GameObjectEventChannelSo onMagnetRemoved;
        [SerializeField] private VoidEventChannelSo onSimulationStart;
        [SerializeField] private VoidEventChannelSo onSimulationEnd;
        [SerializeField] private BoolEventChannelSo onMagnetDragStateChanged;

    
        private bool _isDragging;
        private Vector3 _targetWorldPosition;
        private bool _draggingEnabled = true;
        
        private MagnetsManager _magnetsManager;
        
        private int _currentPointerId = -1;

        private void Awake() {
            _magnetsManager = FindFirstObjectByType<MagnetsManager>();
        }

        private void OnEnable() {
            if (onSimulationStart != null) {
                onSimulationStart.OnRaised += OnDisableDragging;
            }
            
            if (onSimulationEnd != null) {
                onSimulationEnd.OnRaised += OnEnableDragging;
            }
        }

        private void OnDisable() {
            if (onSimulationStart != null) {
                onSimulationStart.OnRaised -= OnDisableDragging;
            }

            if (onSimulationEnd != null) {
                onSimulationEnd.OnRaised -= OnEnableDragging;
            }
        }

        void Start() {
            _targetWorldPosition = transform.position;
        }

        private void Update() {
            if (_isDragging && _draggingEnabled) {
                transform.position = Vector3.Lerp(transform.position, _targetWorldPosition, dragSpeed * Time.deltaTime);
            }
        }

        private void OnEnableDragging() {
            _draggingEnabled = true;
        }

        private void OnDisableDragging() {
            _draggingEnabled = false;
            
            _isDragging = false;
            _currentPointerId = -1;
        }


        public void OnBeginDrag(PointerEventData eventData) {
            if (!_draggingEnabled || _currentPointerId != -1) {
                return; 
            }
            _isDragging = true;
            _currentPointerId = eventData.pointerId;

            if (onMagnetDragStateChanged != null) {
                onMagnetDragStateChanged?.Raise(true);
            }
            
            //_startPosition = transform.position;
            //TODO: Sound Effects could go here (like a 'pickup' sound)
        }
    
        public void OnDrag(PointerEventData eventData) {
            if (!_isDragging || !_draggingEnabled || !_magnetsManager || eventData.pointerId != _currentPointerId) {
                return;
            }
            
            var offsetTouch = eventData.position + cursorOffset;
            var calculatedPos = _magnetsManager.GetClampedWorldPos(offsetTouch);
            
            if (calculatedPos != Vector3.zero) {
                _targetWorldPosition = calculatedPos;
            }
        }
    
        public void OnEndDrag(PointerEventData eventData) {
            if (eventData.pointerId != _currentPointerId) {
                return;
            }

            if (eventData.pointerId == _currentPointerId) {
                if (onMagnetDragStateChanged != null) {
                    onMagnetDragStateChanged?.Raise(false);
                }
            }
            
            _isDragging = false;
            _currentPointerId = -1;
            
            if (!_draggingEnabled) {
                return;
            }

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
}