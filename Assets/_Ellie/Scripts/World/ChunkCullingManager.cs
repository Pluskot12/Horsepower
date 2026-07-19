using UnityEngine;

namespace CarGame
{
    using UnityEngine;
    using System.Collections.Generic;

    public class ChunkCullingManager : MonoBehaviour
    {
        public static ChunkCullingManager Instance { get; private set; }

        [Header("Settings")]
        public float cullDistance = 35f;
        public float checkInterval = 0.25f;

        private readonly List<TerrainChunk> _chunks = new();
        private Transform _player;
        private float _timer;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            _player = GameManager.Instance.Player.transform;
            foreach (var chunk in GetComponentsInChildren<TerrainChunk>())
                _chunks.Add(chunk);
        }
        public bool Active = false;
        public void Register(TerrainChunk chunk) => _chunks.Add(chunk);
        public void Unregister(TerrainChunk chunk) => _chunks.Remove(chunk);

        void Update()
        {
            if (!Active) { return; }

            _timer += Time.deltaTime;
            if (_timer < checkInterval) return;
            _timer = 0f;

            Vector2 playerPos = _player.position;
            float cullSq = cullDistance * cullDistance; // Avoid sqrt per chunk
            
            foreach (var chunk in _chunks)
            {
                bool visible = (chunk.Position - playerPos).sqrMagnitude < cullSq;
                chunk.SetVisible(visible);
            }
        }

        public void SetEnabled(bool enabled) 
        {
            Active = enabled;

            if (!enabled) 
            {
                EnableAll();
            }
        }

        private void EnableAll()
        {
            foreach (var chunk in _chunks)
            {
                chunk.SetVisible(true);
            }
        }
    }
}
