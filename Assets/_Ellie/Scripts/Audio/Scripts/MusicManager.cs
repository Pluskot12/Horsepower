using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        [Header("BGM Sauce")]
        [SerializeField] private AudioSource baseSource;

        [Header("Override Sources")]
        [SerializeField] private AudioSource overrideA;
        [SerializeField] private AudioSource overrideB;

        [Header("Settings")]
        [SerializeField] private float defaultFadeDuration = 1.5f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Music")]
        [SerializeField] private AudioClip bgm;
        [SerializeField] private AudioClip chaseMusic;

        [Header("Gameplay Music Settings")]
        [SerializeField] private float tickRate = 10f;
        [SerializeField, Range(0f, 1f)] private float playChance = 0.5f;

        private AudioSource activeOverride;
        private AudioSource inactiveOverride;
        private AudioClip currentOverrideClip;

        private Coroutine baseFadeRoutine;
        private Coroutine overrideFadeRoutine;

        private float timer;

        private bool bgmPlaying;

        private void Awake()
        {
            Instance = this;

            ConfigureSource(baseSource);
            ConfigureSource(overrideA);
            ConfigureSource(overrideB);

            activeOverride = overrideA;
            inactiveOverride = overrideB;

            EnemyController.OnAggro += EnemyController_Aggro;
        }

        private void EnemyController_Aggro(int count)
        {
            // Debug.Log("Aggro " + count);

            if (count >= 1)
            {
                PlayOverride(chaseMusic, 1.0f);
            }
            else
            {
                ClearOverride(3f);
            }
        }

        private void Update()
        {
            float time = TimeManager.Instance.GetTime();
            float duskStart = TimeManager.Instance.DuskStartTime - 10;
            float dayStart = TimeManager.Instance.DayStartTime + 10;

            if (bgmPlaying && time >= duskStart && time <= dayStart)
            {
                FadeOutBase();
            }

            timer += Time.deltaTime;

            if (timer >= tickRate)
            {
                timer = 0f;

                if (TimeManager.Instance.GetTimeOfDay() != TimeManager.TimeOfDay.Day)
                {
                    return;
                }

                float random = Random.value;

                if (random <= playChance)
                {
                    SetBaseMusic(bgm);
                }
            }
        }

        private void ConfigureSource(AudioSource src)
        {
            src.loop = false;
            src.playOnAwake = false;
            src.volume = 0f;
        }

        public void SetBaseMusic(AudioClip clip, float fadeDuration = -1f)
        {
            if (fadeDuration < 0f) fadeDuration = defaultFadeDuration;

            if (baseSource.clip != clip)
            {
                baseSource.clip = clip;
                baseSource.Play();
            }
            else if (!baseSource.isPlaying)
            {
                baseSource.Play();
            }

            bgmPlaying = true;

            float target = currentOverrideClip == null ? 1f : 0f;
            FadeSource(ref baseFadeRoutine, baseSource, target, fadeDuration);
        }

        public void FadeOutBase(float fadeDuration = -1f, bool stopPlayback = false)
        {
            if (fadeDuration < 0f)
            {
                fadeDuration = defaultFadeDuration;
            }

            if (stopPlayback)
            {
                if (baseFadeRoutine != null)
                {
                    StopCoroutine(baseFadeRoutine);
                }

                baseFadeRoutine = StartCoroutine(FadeBaseOutAndStop(fadeDuration));
            }
            else
            {
                FadeSource(ref baseFadeRoutine, baseSource, 0f, fadeDuration);
            }

            bgmPlaying = false;
        }

        public void FadeInBase(float fadeDuration = -1f)
        {
            if (fadeDuration < 0f) fadeDuration = defaultFadeDuration;

            if (!baseSource.isPlaying && baseSource.clip != null)
                baseSource.Play();

            float target = currentOverrideClip == null ? 1f : 0f;
            FadeSource(ref baseFadeRoutine, baseSource, target, fadeDuration);
        }

        private IEnumerator FadeBaseOutAndStop(float duration)
        {
            float start = baseSource.volume;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                baseSource.volume = Mathf.Lerp(start, 0f, fadeCurve.Evaluate(t / duration));
                yield return null;
            }
            baseSource.volume = 0f;
            baseSource.Stop();
            baseFadeRoutine = null;
        }

        public void PlayOverride(AudioClip clip, float fadeDuration = -1f, float targetVolume = 1f)
        {
            if (fadeDuration < 0f) fadeDuration = defaultFadeDuration;
            if (clip == currentOverrideClip) return;

            currentOverrideClip = clip;

            FadeSource(ref baseFadeRoutine, baseSource, 0f, fadeDuration);

            AudioSource fadeOut = activeOverride;
            AudioSource fadeIn = inactiveOverride;

            fadeIn.clip = clip;
            fadeIn.volume = 0f;
            fadeIn.Play();

            if (overrideFadeRoutine != null) StopCoroutine(overrideFadeRoutine);
            overrideFadeRoutine = StartCoroutine(CrossfadeOverride(fadeOut, fadeIn, targetVolume, fadeDuration));

            activeOverride = fadeIn;
            inactiveOverride = fadeOut;
        }

        public void ClearOverride(float fadeDuration = -1f)
        {
            if (fadeDuration < 0f) fadeDuration = defaultFadeDuration;
            if (currentOverrideClip == null) return;

            currentOverrideClip = null;

            FadeSource(ref baseFadeRoutine, baseSource, 1f, fadeDuration);

            AudioSource toStop = activeOverride;

            if (overrideFadeRoutine != null)
            {
                StopCoroutine(overrideFadeRoutine);
            }
            overrideFadeRoutine = StartCoroutine(FadeOutAndStop(toStop, fadeDuration));
        }

        private IEnumerator CrossfadeOverride(AudioSource fadeOut, AudioSource fadeIn, float targetVolume, float duration)
        {
            float startOut = fadeOut.volume;
            float t = 0f;

            while (t < duration)
            {
                t += Time.deltaTime;
                float n = fadeCurve.Evaluate(t / duration);
                fadeOut.volume = Mathf.Lerp(startOut, 0f, n);
                fadeIn.volume = Mathf.Lerp(0f, targetVolume, n);
                yield return null;
            }

            fadeOut.volume = 0f;
            fadeOut.Stop();
            fadeIn.volume = targetVolume;
            overrideFadeRoutine = null;
        }

        private IEnumerator FadeOutAndStop(AudioSource src, float duration)
        {
            float start = src.volume;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                src.volume = Mathf.Lerp(start, 0f, fadeCurve.Evaluate(t / duration));
                yield return null;
            }
            src.volume = 0f;
            src.Stop();

            overrideFadeRoutine = null;
        }

        private void FadeSource(ref Coroutine routine, AudioSource src, float target, float duration)
        {
            if (routine != null)
            {
                StopCoroutine(routine);
            }

            routine = StartCoroutine(FadeVolume(src, target, duration));
        }

        private IEnumerator FadeVolume(AudioSource src, float target, float duration)
        {
            float start = src.volume;
            float t = 0f;

            while (t < duration)
            {
                t += Time.deltaTime;
                src.volume = Mathf.Lerp(start, target, fadeCurve.Evaluate(t / duration));
                yield return null;
            }

            src.volume = target;
        }
    }
}