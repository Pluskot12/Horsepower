using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Ellie.Audio
{
    public enum Channel
    {
        Effects,
        UI,
        Ambience
    }

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        public static event Action<Channel, bool> PauseChannel;

        [SerializeField] private AudioMixer mixer;
        [SerializeField] private SoundPool sfxSoundPool;
        [SerializeField] private SoundPool uiSoundPool;

        private SoundBuilder sfxSoundBuilder;
        private SoundBuilder uiSoundBuilder;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Multiple copies of SoundManager");

                Destroy(gameObject);

                return;
            }

            Instance = this;

            sfxSoundBuilder = new SoundBuilder(sfxSoundPool);
            uiSoundBuilder = new SoundBuilder(uiSoundPool);
        }

        private void Start()
        {
            sfxSoundPool.InitializePool();
            uiSoundPool.InitializePool();
        }

        public static void PlaySFX(AudioClip clip, Vector3 position)
        {
            if (Instance == null)
            {
                Debug.LogWarning("No instance of SoundManager found.");

                return;
            }

            Instance.sfxSoundBuilder.WithClip(clip).WithPosition(position).WithRandomPitch().Play();
        }

        public static void PlayRandomSFX(AudioClip[] clips, Vector3 position, float pitch = 1f)
        {
            if (Instance == null)
            {
                Debug.LogWarning("No instance of SoundManager found.");

                return;
            }

            PlaySFX(clips[Random.Range(0, clips.Length)], position);
        }

        public static void PlayUI(AudioClip clip)
        {
            if (Instance == null)
            {
                Debug.LogWarning("No instance of SoundManager found.");

                return;
            }

            if (clip == null)
            {
                return;
            }

            //Instance.uiSoundBuilder.WithClip(clip).WithPosition(CameraManager.Instance.AudioPosition).Play();
            Instance.uiSoundBuilder.WithClip(clip).Play();
        }

        public static void MuteEffects(bool mute)
        {
            MuteChannel(Channel.Effects, mute);
            //float volume = mute ? -80 : 0;
            //Instance.mixer.SetFloat("Effects", volume);
        }

        private static void MuteChannel(Channel channel, bool mute)
        {
            PauseChannel?.Invoke(channel, mute);
        }

    }
}