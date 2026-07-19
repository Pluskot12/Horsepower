using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace CarGame
{
    public class SceneLoaderUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private GraphicRaycaster raycaster;
        [SerializeField] private Image overlay;
        //[SerializeField] private TextMeshProUGUI loadingText;
        [SerializeField] private Image icon;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject[] backgrounds;

        [Header("Animation Settings")]
        [SerializeField] private float fadeTime = 0.3f;
        [SerializeField] private float delay = 0.2f;
        [SerializeField] private float spinSpeed = 6.2f;

        private void Awake()
        {
            canvas.enabled = false;
            raycaster.enabled = false;

            canvasGroup.alpha = 0;

            foreach (var bg in backgrounds)
            {
                bg.SetActive(false);
            }
        }

        private void Update()
        {
            icon.transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
        }

        public void UpdateProgress(float progress)
        {

        }

        public async Awaitable Show()
        {
            UpdateProgress(0);
            SelectRandomBackground();

            canvas.enabled = true;
            raycaster.enabled = true;

            //loadingText.alpha = 0;

            await Sequence.Create(useUnscaledTime: true)
                    .Group(Tween.Alpha(canvasGroup, 0, 1, fadeTime))
                    .Group(Tween.Alpha(icon, 0, 1, fadeTime, startDelay: delay));
        }

        public async Awaitable Hide()
        {
            await Sequence.Create(useUnscaledTime: true)
                    .Group(Tween.Alpha(canvasGroup, 1, 0, fadeTime, startDelay: delay))
                    .Group(Tween.Alpha(icon, 1, 0, fadeTime));

            canvas.enabled = false;
            raycaster.enabled = false;
        }

        private void SelectRandomBackground()
        {
            int random = Random.Range(0, backgrounds.Length);

            for (int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i].SetActive(i == random);
            }
        }
    }
}
