using UnityEngine;
using UnityEngine.EventSystems;

public class UIInteractableButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [Header("References")] 
    [SerializeField] private MagnetsManager magnetManager;
    [SerializeField] private GameObject magnetPrefab;
    private GameObject _activeMagnet;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _activeMagnet = Instantiate(magnetPrefab);
        magnetManager.RegisterMagnet(_activeMagnet);
        UpdateMagnetPosition(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_activeMagnet)
        {
            UpdateMagnetPosition(eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _activeMagnet = null;
        {
            magnetManager.RemoveMagnet(_activeMagnet);
            _activeMagnet = null;
        }
    }

    private void UpdateMagnetPosition(Vector2 screenPos)
    {
        var newPosition = magnetManager.GetClampedWorldPos(screenPos);
        if (newPosition != Vector3.zero) {
            _activeMagnet.transform.position = newPosition;
        }
    }
}