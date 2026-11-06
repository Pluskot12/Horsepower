using PrimeTween;
using UnityEngine;

namespace CarGame
{
    public class SeekerMark : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private float iconFadeInTime = 0.1f;
        [SerializeField] private float iconFadeOutTime = 0.45f;

        public void Show()
        {
            gameObject.SetActive(true);

            Tween.Alpha(icon, 0f, 1f, duration: iconFadeInTime);
            Tween.PunchScale(icon.transform, strength: Vector3.one * 0.25f, duration: .3f, frequency: 7);
        }

        public void Hide()
        {
            Tween.Alpha(icon, 0f, duration: iconFadeOutTime).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}
