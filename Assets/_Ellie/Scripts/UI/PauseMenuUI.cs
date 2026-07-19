using Ellie.Audio;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace CarGame
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private GraphicRaycaster raycaster;
        [SerializeField] private Image biomeImage;

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeTime = 0.25f;

        private bool isPaused;
        public bool IsPaused => isPaused;

        public void OnEscapeButton()
        {
            if (isPaused)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private void Awake()
        {
            canvas.enabled = false;
            raycaster.enabled = false;
            canvasGroup.alpha = 0;
        }
        Tween tween;
        public async void Show()
        {
            if (isPaused)
            {
                Debug.Log("Already paused");
                return;
            }

            isPaused = true;

            await SaveManager.Instance.SaveAsync();

            biomeImage.sprite = BiomeManager.Instance.CurrentBiome.icon;
            biomeImage.SetNativeSize();

            Pause(true);

            canvas.enabled = true;
            raycaster.enabled = true;

            if (tween.isAlive)
            {
                tween.Stop();
            }
            tween = Tween.Alpha(canvasGroup, 1f, fadeTime, useUnscaledTime: true);
            SoundManager.PlayUI(BiomeManager.Instance.CurrentBiome.pauseSound);
        }

        public void Hide()
        {
            Pause(false);

            raycaster.enabled = false;
            isPaused = false;
            if (tween.isAlive)
            {
                tween.Stop();
            }
            tween = Tween.Alpha(canvasGroup, 0f, fadeTime, useUnscaledTime: true).OnComplete(() => canvas.enabled = false);
        }

        private void Pause(bool pause)
        {
            if (pause)
            {
                GameManager.PauseGame();
            }
            else
            {
                GameManager.UnpauseGame();
            }
        }

        public void OnContinueButton()
        {
            Hide();
        }

        public async void OnExitButton()
        {
            //GameManager.Instance.SaveGame();

            //Hide();
            await SceneLoader.LoadScene("Main Menu Scene", SaveManager.Instance.SaveAsync);
            // await SceneLoader.Instance.LoadScene("Empty", SaveManager.Instance.SaveAsync);
        }
    }
}
