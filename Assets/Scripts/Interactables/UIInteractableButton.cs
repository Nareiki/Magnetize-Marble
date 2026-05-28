using Events;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Interactables {
    public class UIInteractableButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        [Header("References")] 
        [SerializeField] private GameObject magnetPrefab;

        [Header("Events")] 
        [SerializeField] private GameObjectEventChannelSo onMagnetCreated;
        [SerializeField] private GameObjectEventChannelSo onMagnetRemoved;
        [SerializeField] private VoidEventChannelSo onSimulationStart;
        [SerializeField] private VoidEventChannelSo onSimulationEnd;
        [SerializeField] private BoolEventChannelSo onMagnetDragStateChanged;
    
        private GameObject _activeMagnet;
        private bool _spawningEnabled = true;
        private MagnetsManager _magnetManager;

        private int _currentPointerId = -1;

        private void Awake() {
            _magnetManager = FindFirstObjectByType<MagnetsManager>();
        }

        private void OnEnable() {
            if (onSimulationStart != null) {
                onSimulationStart.OnRaised += OnDisableSpawning;
            }
            
            if (onSimulationEnd != null) {
                onSimulationEnd.OnRaised += OnEnableSpawning;
            }
        }

        private void OnDisable() {
            if (onSimulationStart != null) {
                onSimulationStart.OnRaised -= OnDisableSpawning;
            }

            if (onSimulationEnd != null) {
                onSimulationEnd.OnRaised -= OnEnableSpawning;
            }
        }
        
        private void OnEnableSpawning() {
            _spawningEnabled = true;
        }

        private void OnDisableSpawning() {
            _spawningEnabled = false;
            CancelActiveDrag();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_spawningEnabled || !_magnetManager || _currentPointerId != -1) return;
            
            if (onMagnetDragStateChanged != null) {
                onMagnetDragStateChanged?.Raise(true);
            }
            
            _currentPointerId = eventData.pointerId;
            
            _activeMagnet = Instantiate(magnetPrefab);
            onMagnetCreated?.Raise(_activeMagnet);
            _magnetManager.UpdateObjectPositionToCursor(_activeMagnet, eventData.position, Vector2.zero, true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_spawningEnabled || eventData.pointerId != _currentPointerId) {
                CancelActiveDrag();
                return;
            }
            
            if (_activeMagnet && _magnetManager)
            {
                _magnetManager.UpdateObjectPositionToCursor(_activeMagnet, eventData.position, Vector2.zero, true);
            }
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (eventData.pointerId != _currentPointerId) {
                return;
            }
            
            if (onMagnetDragStateChanged != null) {
                onMagnetDragStateChanged?.Raise(false);
            }
            
            if (!_activeMagnet) {
                return;
            }
            
            if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("RemoveInteractable"))
            {
                
            }
        
            _activeMagnet = null;
            _currentPointerId = -1;
        }

        private void CancelActiveDrag() {
            if (!_activeMagnet) {
                return;
            }
            
            onMagnetRemoved?.Raise(_activeMagnet);
            _activeMagnet = null;
            _currentPointerId = -1;
        }
    }
}