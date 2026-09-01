using Ellie.Audio;
using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private Camera mainCamera;
        [SerializeField] private Player player;

        [SerializeField] private Vector3 spawnPoint;
        public Vector3 SpawnPoint => spawnPoint;
        [SerializeField] private Vector3 defaultSpawnPoint;

        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private WorldObjectDatabase worldObjectDatabase;
        [SerializeField] private Chest sorryChest;

        public ItemDatabase ItemDatabase => itemDatabase;
        public WorldObjectDatabase WorldObjectDatabase => worldObjectDatabase;

        private Vector3 mousePosition;
        public Vector3 MousePosition => GetMousePosition();

        public Camera Camera => mainCamera;
        public Player Player => player;

        public static bool GamePaused { get; private set; }

        private void Awake()
        {
            Instance = this;

            defaultSpawnPoint = spawnPoint;
        }
        [SerializeField] private float startTime = 1100;

        private void Start()
        {
            //TimeManager.Instance.SetTime(startTime);
        }
        public void Init()
        {
            Time.timeScale = 0;

            if (useLoadedData)
            {
                // Load stuff
                SaveManager.Instance.Load();
            }
            else
            {
                TerrainManager.Instance.GenerateNewWorld();
                TimeManager.Instance.SetTime(startTime);
                player.StartGame();

                sorryChest.gameObject.SetActive(true);
                sorryChest.OnPlace(null);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIMananger.Instance.OnEscapeKey();
            }
        }

        public void StartGame()
        {
            Time.timeScale = 1f;
        }

        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(1f);
        }

        public void Respawn()
        {

            Player.Respawn();
            //;
            UIMananger.Instance.ShowPlayerUI();
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private CareCenter currentCareCenter;

        public CareCenter GetSpawnPoint()
        {
            return currentCareCenter;
        }

        public void SetSpawnPoint(CareCenter careCenter, Vector3 spawnPoint)
        {
            Debug.Log("Spawnpoint set");
            currentCareCenter = careCenter;
            this.spawnPoint = spawnPoint;
        }

        public void RevertSpawnPoint()
        {
            currentCareCenter = null;
            spawnPoint = defaultSpawnPoint;
        }

        public void OnPlayerDeath()
        {
            UIMananger.Instance.ShowDeathScreen();
        }

        public Vector3 GetMousePosition()
        {
            mousePosition = Input.mousePosition;
            mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
            mousePosition.z = 0;

            return mousePosition;
        }

        public Vector2 GetUIPosition(GameObject go)
        {
            return RectTransformUtility.WorldToScreenPoint(mainCamera, go.transform.TransformPoint(Vector3.zero));
        }

        public Vector2 GetUIPosition(Vector3 position)
        {
            return RectTransformUtility.WorldToScreenPoint(mainCamera, position);
        }

        public Vector2 TestPos(Vector3 pos, Canvas canvas)
        {
            Vector2 screenPos = mainCamera.WorldToScreenPoint(pos);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                screenPos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
                out Vector2 localPoint
            );

            return localPoint;
        }

        public bool IsVisibleOnScreen(Vector3 worldPosition, float margin = 0.1f)
        {
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(worldPosition);

            return viewportPos.x > -margin && viewportPos.x < 1 + margin &&
                   viewportPos.y > -margin && viewportPos.y < 1 + margin &&
                   viewportPos.z > 0;
        }
        /*
        public void SaveGame()
        {
            SaveManager.Instance.Save();
        }
        */
        static bool useLoadedData;

        public static void NewGame()
        {
            useLoadedData = false;
        }

        public static void LoadGame()
        {
            useLoadedData = true;
        }

        public static void PauseGame()
        {
            //AudioListener.pause = true;
            SoundManager.MuteEffects(true);

            GamePaused = true;
            Time.timeScale = 0f;
        }

        public static void UnpauseGame()
        {
            //AudioListener.pause = false;
            SoundManager.MuteEffects(false);

            GamePaused = false;
            Time.timeScale = 1f;
        }
    }
}