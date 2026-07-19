using Ellie.Audio;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CarGame
{
    public class ButtonPunchAnimation : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        [SerializeField] private AudioClip pressSound;
        [SerializeField] private AudioClip hoverSound;

        public void OnPointerClick(PointerEventData eventData)
        {
            Tween.PunchScale(transform, strength: Vector3.one * 0.15f, duration: .3f, frequency: 7, useUnscaledTime: true);

            SoundManager.PlayUI(pressSound);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SoundManager.PlayUI(hoverSound);
        }
    }
}
