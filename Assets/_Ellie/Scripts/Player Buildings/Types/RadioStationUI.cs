using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace CarGame
{
    public class RadioStationUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private GraphicRaycaster raycaster;

        [Header("Animation Settings")]
        [SerializeField] private RectTransform animationParent;
        [SerializeField] private float offPosition = -340f;
        [SerializeField] private float inDuration = 0.2f;

        private bool isShowing;
        private Tween tween;

        private void Start()
        {
            animationParent.anchoredPosition = new Vector2(0, offPosition);
        }

        public void Show(Player player, bool animate = true)
        {
            isShowing = true;
            canvas.enabled = true;

            if (animate)
            {
                Animate();
            }
        }

        public void Hide(bool animate = true)
        {
            isShowing = false;

            if (animate)
            {
                Animate();
            }
            else
            {
                canvas.enabled = false;
                raycaster.enabled = false;
            }
        }

        private void Animate()
        {
            if (tween.isAlive)
            {
                tween.Stop();
            }

            float position = isShowing ? 0 : offPosition;
            tween = Tween.UIAnchoredPositionY(animationParent, endValue: position, duration: inDuration, ease: Ease.InOutQuart).OnComplete(() => AnimationComplete());
        }

        private void AnimationComplete()
        {
            if (isShowing)
            {
                raycaster.enabled = true;
            }
            else
            {
                canvas.enabled = false;
                raycaster.enabled = false;
            }
        }
    }
}
