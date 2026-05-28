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

        private void Awake() {
            if (innerIconGraphic == null) {
                innerIconGraphic = GetComponentInChildren<Graphic>();
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
            if (buttonAnimator != null) {
                buttonAnimator.enabled = enable;
                
                if (!enable) {
                    buttonAnimator.Rebind();
                }
            }
        }
    }
}