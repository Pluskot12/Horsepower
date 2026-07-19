using System.Collections;
using UnityEngine;

namespace Ellie.Audio
{
    public class SoundEmitterPooled : SoundEmitter
    {
        private SoundPool pool;

        private Coroutine coroutine;

        public void SetPool(SoundPool soundPool)
        {
            pool = soundPool;
        }


        public override void Play()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            audioSource.Play();

            StartCoroutine(WaitForSound());
        }


        public override void Stop()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            audioSource.Stop();

            pool.ReturnToPool(this);
        }


        private IEnumerator WaitForSound()
        {
            yield return new WaitWhile(() => audioSource.isPlaying || isPaused);

            pool.ReturnToPool(this);
        }

    }
}