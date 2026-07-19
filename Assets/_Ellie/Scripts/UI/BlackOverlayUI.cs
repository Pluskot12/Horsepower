using PrimeTween;
using System;
using UnityEngine;

namespace CarGame
{
    public class BlackOverlayUI : MonoBehaviour
    {
        public static BlackOverlayUI Instance { get; private set; }

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeTime;
        [SerializeField] private float stayTime;

        private void Awake()
        {
            Instance = this;

            canvasGroup.alpha = 0;
        }

        //public async void Show(Func<Awaitable> action = null)
        public async void Show(Action action, Action action2)
        {
            await Tween.Alpha(canvasGroup, 1, fadeTime).OnComplete(() =>
            {
                action?.Invoke();
                action2?.Invoke();
            });
            await Awaitable.WaitForSecondsAsync(stayTime);

            Hide();
        }

        public void Hide()
        {
            Tween.Alpha(canvasGroup, 0, fadeTime);
        }
    }
}
