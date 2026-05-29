using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UIButtonStateView : MonoBehaviour {
        [Header("Child Graphic References")]
        [Tooltip("The inner child Image or RawImage that holds the visual icon.")]
        [SerializeField] private Graphic innerIconGraphic;
        
        [Header("Animation Config")]
        [Tooltip("Optional: The animator component attached to this button frame or child.")]
        [SerializeField] private Animator buttonAnimator;
        
        [Header("Drag Hover Visuals")]
        [Tooltip("The sprite used when a magnet is actively being dragged by the player.")]
        [SerializeField] private Sprite activeTrashHoverSprite;
        [Tooltip("A highlight color applied when an item is held over or ready to drop.")]
        [SerializeField] private Color heldHighlightColor = Color.white;
        
        [Header("Group Settings")]
        [Tooltip("The Canvas Group component attached to this button frame.")]
        [SerializeField] private CanvasGroup canvasGroup;

        private void Awake() {
            if (innerIconGraphic == null) {
                innerIconGraphic = GetComponentInChildren<Graphic>();
            }
            if (buttonAnimator == null) {
                buttonAnimator = GetComponent<Animator>();
            }
            if (canvasGroup == null) {
                canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        public void SetIconColor(Color targetColor) {
            if (innerIconGraphic) {
                innerIconGraphic.color = targetColor;
            }
        }

        public void SetIconSprite(Sprite newSprite) {
            if (innerIconGraphic is Image uiImage && newSprite) {
                uiImage.sprite = newSprite;
            }
        }
        
        public void ToggleAnimations(bool enable) {
            if (buttonAnimator) {
                buttonAnimator.enabled = enable;
                
                if (!enable) {
                    buttonAnimator.Rebind();
                }
            }
        }
        
        public void SetInteractable(bool isInteractable, float targetAlpha = 1.0f) {
            if (!canvasGroup) return;
            
            canvasGroup.interactable = isInteractable;
            canvasGroup.blocksRaycasts = isInteractable;
            canvasGroup.alpha = targetAlpha;
        }
        
        public void SetDragHoverState(bool isItemInHand, bool allowAnimatorPulse) {
            if (isItemInHand) {
                if (activeTrashHoverSprite) {
                    SetIconSprite(activeTrashHoverSprite);
                }
                SetIconColor(heldHighlightColor);
            } else {
                SetIconColor(Color.white);
            }

            if (buttonAnimator && buttonAnimator.enabled) {
                buttonAnimator.SetBool("IsTargetActive", isItemInHand && allowAnimatorPulse);
            }
        }
    }
}