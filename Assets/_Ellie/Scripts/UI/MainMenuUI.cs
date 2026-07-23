using PrimeTween;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CarGame
{
    public class MainMenuUI : MonoBehaviour
    {
        private const string GAME_SCENE = "Game Scene";

        [SerializeField] private Button startButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button exitButton;

        [SerializeField] private TextMeshProUGUI continueText;
        [SerializeField] private Image continueIcon;
        [SerializeField] private Color disabledColor;
        [SerializeField] private Color disabledTextColor;

        [SerializeField] private Image clouds;
        [SerializeField] private float scrollSpeed;


        private float cloudWidth;
        private float startX;

        void Awake()
        {
            //PlayerPrefs.DeleteAll();

            PrimeTweenConfig.warnEndValueEqualsCurrent = false;

            cloudWidth = clouds.rectTransform.rect.width / 3f;
            startX = clouds.rectTransform.anchoredPosition.x;

            if (!SaveManager.HasSaveFile())
            {
                continueButton.interactable = false;

                continueButton.image.color = disabledColor;
                continueIcon.color = disabledTextColor;
                continueText.color = disabledTextColor;
            }
        }

        private void Start()
        {
            GameManager.UnpauseGame();
        }

        private void Update()
        {
            Vector2 pos = clouds.rectTransform.anchoredPosition;
            pos.x -= scrollSpeed * Time.deltaTime;

            if (pos.x <= startX - cloudWidth)
            {
                pos.x += cloudWidth;
            }

            clouds.rectTransform.anchoredPosition = pos;
        }

        private IEnumerator LoadScene(string sceneName)
        {
            yield return new WaitForSeconds(0.1f);

            Awaitable awaitable = SceneLoader.LoadScene(sceneName);
        }

        public void OnNewGameButton()
        {
            DisableButtons();

            GameManager.NewGame();

            // Create new Save Data
            //Tween.PunchScale(inventoryButton.transform, strength: Vector3.one * 0.2f, duration: .3f, frequency: 7);

            StartCoroutine(LoadScene(GAME_SCENE));
        }

        public void OnContinueButton()
        {
            DisableButtons();

            GameManager.LoadGame();

            StartCoroutine(LoadScene(GAME_SCENE));
        }

        public void OnExitButton()
        {
            DisableButtons();

            Application.Quit();
        }

        private void DisableButtons()
        {
            startButton.interactable = false;
            continueButton.interactable = false;
            exitButton.interactable = false;
        }

    }
}
