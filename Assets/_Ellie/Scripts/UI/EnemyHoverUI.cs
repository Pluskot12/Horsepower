using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CarGame
{
    public class EnemyHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Animation")]
        [SerializeReference] private CanvasGroup canvasGroup;
        [SerializeReference] private TextMeshProUGUI hpText;
        [SerializeReference] private float inDuration = 0.45f;
        [SerializeReference] private float outDuration = 0.45f;

        public void Init(int hp)
        {
            transform.SetParent(null);
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            SetHpText(hp);
        }

        public void Dispose()
        {
            if (this != null)
            {
                Destroy(gameObject);
            }
        }

        public void SetHpText(int hp)
        {
            hpText.text = hp.ToString();
        }

        private void AnimateIn()
        {
            Sequence.Create().Group(Tween.Custom(0f, 1f, duration: inDuration, onValueChange: newVal => canvasGroup.alpha = newVal));
        }

        private void AnimateOut()
        {
            Sequence.Create().Group(Tween.Custom(1f, 0f, duration: outDuration, onValueChange: newVal => canvasGroup.alpha = newVal));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this == null)
            {
                return;
            }

            AnimateIn();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (this == null)
            {
                return;
            }

            AnimateOut();
        }
    }
}