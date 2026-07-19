using UnityEngine;

namespace Ellie.Audio
{
    public class SoundEmitter : MonoBehaviour
    {
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected Channel channel;

        protected bool isPaused = false;

        private void OnEnable()
        {
            SoundManager.PauseChannel += OnPause;
        }
        private void OnDisable()
        {
            SoundManager.PauseChannel -= OnPause;
        }

        private void OnPause(Channel channel, bool pause)
        {
            if (this.channel != channel)
            {
                return;
            }

            if (pause)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }

        public void Init(SoundData data)
        {
            audioSource.clip = data.clip;
            audioSource.volume = 1f;
            audioSource.pitch = 1f;
        }

        public void Init(AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.volume = 1f;
            audioSource.pitch = 1f;
        }

        public virtual void Play()
        {
            audioSource.Play();
        }

        public virtual void Stop()
        {
            audioSource.Stop();
        }

        public void Pause()
        {
            if (!audioSource)
            {
                Debug.LogWarning("No source");
                return;
            }

            if (audioSource.isPlaying)
            {
                audioSource.Pause();
                isPaused = true;
            }
        }

        public void Resume()
        {
            if (!audioSource)
            {
                Debug.LogWarning("No source");
                return;
            }

            if (audioSource && isPaused)
            {
                audioSource.UnPause();
                isPaused = false;
            }
        }


        public void SetPitch(float pitch)
        {
            audioSource.pitch = pitch;
        }

    }
}