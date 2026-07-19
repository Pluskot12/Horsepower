using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CarGame
{
    public class SceneLoader : MonoBehaviour
    {
        private const string SCENE_NAME = "Scene Loader";
        public static SceneLoader Instance { get; private set; }

        [SerializeField] private SceneLoaderUI ui;
        [SerializeField] private float minLoading = 0.1f;

        private static Scene currentGameScene;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                currentGameScene = SceneManager.GetActiveScene();
                //transform.SetParent(null);
                // DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }



        private static async Awaitable EnsureLoaded()
        {
            if (SceneManager.GetSceneByName(SCENE_NAME).isLoaded)
            {
                return;
            }

            await SceneManager.LoadSceneAsync(SCENE_NAME, LoadSceneMode.Additive);
        }


        public static async Awaitable LoadScene(string sceneName, Func<Awaitable> onBeforeLoad = null)
        {
            await EnsureLoaded();

            await Instance.ui.Show();

            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            Debug.Log("Loading Scene");

            if (onBeforeLoad != null)
            {
                await onBeforeLoad();
            }

            Scene currentScene = SceneManager.GetActiveScene();
            if (currentGameScene.isLoaded && currentGameScene.name != SCENE_NAME)
            {
                Debug.Log("Unloading " + currentGameScene.name);
                await SceneManager.UnloadSceneAsync(currentGameScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                await Resources.UnloadUnusedAssets();
                Debug.Log("Scene unloaded");
            }


            Time.timeScale = 1f;

            AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            scene.allowSceneActivation = false;

            while (!scene.isDone)
            {
                float smoothed = Mathf.Clamp01(scene.progress / 0.9f);
                Instance.ui.UpdateProgress(smoothed);

                if (scene.progress >= 0.9f)
                {

                    await Awaitable.WaitForSecondsAsync(Instance.minLoading);
                    scene.allowSceneActivation = true;

                    await Awaitable.NextFrameAsync();

                    Scene loadedScene = SceneManager.GetSceneByName(sceneName);
                    while (!loadedScene.isLoaded)
                    {
                        await Awaitable.NextFrameAsync();
                        loadedScene = SceneManager.GetSceneByName(sceneName);

                    }

                    currentGameScene = loadedScene;
                    SceneManager.SetActiveScene(currentGameScene);

                }

                await Awaitable.NextFrameAsync();
            }

            if (currentGameScene.name == "Game Scene")
            {

                GameManager.Instance.Init();

                await WaitForSecondsRealtimeAsync(1.25f);

                GameManager.Instance.StartGame();

                await Awaitable.WaitForSecondsAsync(0.15f);

            }

            //currentGameScene = SceneManager.GetSceneByName(sceneName);
            //SceneManager.SetActiveScene(currentGameScene);
            Debug.Log("Loading Completed");

            await Instance.ui.Hide();
        }

        private static async Awaitable WaitForSecondsRealtimeAsync(float seconds)
        {
            float elapsed = 0f;
            while (elapsed < seconds)
            {
                elapsed += Time.unscaledDeltaTime;
                await Awaitable.NextFrameAsync();
            }
        }
    }
}
