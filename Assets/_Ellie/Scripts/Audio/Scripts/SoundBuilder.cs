using UnityEngine;
namespace Ellie.Audio
{
    public class SoundBuilder
    {
        readonly SoundPool pool;
        private SoundData data;
        private Vector3 position;
        
        private bool randomPitch;
        private float pitch;
        private float pitchMin;
        private float pitchMax;

        private AudioClip clip;

        public SoundBuilder(SoundPool soundPool) 
        {
            pool = soundPool;
        }

        public SoundBuilder WithClip(AudioClip clip) 
        {
            this.clip = clip;

            return this;
        }

        public SoundBuilder WithSoundData(SoundData data) 
        {
            this.data = data;
            return this;
        }

        public SoundBuilder WithPosition(Vector3 position) 
        {
            this.position = position;
            return this;
        }

        public SoundBuilder WithRandomPitch(float min = 0.9f, float max = 1.1f) 
        {
            this.randomPitch = true;
            this.pitchMin = min;
            this.pitchMax = max;

            return this;
        }

        public void Play() 
        {
            SoundEmitterPooled emitter = pool.Get();
            
            emitter.Init(clip);
            //emitter.Init(data);

            emitter.transform.position = position;

            if (randomPitch) 
            {
                pitch = 1f * Random.Range(pitchMin, pitchMax);
                emitter.SetPitch(pitch);
            }

            emitter.Play();

        }

    }
}