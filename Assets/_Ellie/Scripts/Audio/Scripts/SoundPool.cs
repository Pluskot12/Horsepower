using Ellie.Audio;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SoundPool : MonoBehaviour
{
    [SerializeField] private SoundEmitterPooled prefab;
    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxPoolSize = 100;
    [SerializeField] private bool collectionCheck = true;

    private IObjectPool<SoundEmitterPooled> pool;
    public readonly List<SoundEmitterPooled> activeSoundEmitters = new List<SoundEmitterPooled>();

    public void InitializePool()
    {
        pool = new ObjectPool<SoundEmitterPooled>(
            CreateSoundEmitter,
            OnTakeFromPool,
            OnReturnedToPool,
            OnPoolDestroyed,
            collectionCheck,
            defaultCapacity,
            maxPoolSize);
    }

    public SoundEmitterPooled Get()
    {
        return pool.Get();
    }

    public void ReturnToPool(SoundEmitterPooled emitter)
    {
        pool.Release(emitter);
    }

    private SoundEmitterPooled CreateSoundEmitter()
    {
        var soundEmitter = Instantiate(prefab, transform);
        soundEmitter.SetPool(this);
        soundEmitter.gameObject.SetActive(false);

        return soundEmitter;
    }

    private void OnTakeFromPool(SoundEmitterPooled emitter)
    {
        emitter.gameObject.SetActive(true);
        activeSoundEmitters.Add(emitter);
    }

    private void OnReturnedToPool(SoundEmitterPooled emitter)
    {
        emitter.gameObject.SetActive(false);
        activeSoundEmitters.Remove(emitter);
    }

    private void OnPoolDestroyed(SoundEmitterPooled emitter)
    {
        Destroy(emitter.gameObject);
    }
}
