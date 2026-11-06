using Demo.Perception;
using Ellie.Core;
using System.Collections.Generic;
using UnityEngine;

namespace CarGame
{
    public class TrailerSpawner : MonoBehaviour
    {
        [SerializeField] private Enemy enemy;
        [SerializeField] private Vector3 trailerSpawnPosition;
        [SerializeField] private Transform hitchPoint;
        [SerializeField] private List<Trailer> variants;

        private Trailer trailer;

        private void Start()
        {
            var variant = variants[Random.Range(0, variants.Count)];

            trailer = Instantiate(variant, enemy.transform.position + trailerSpawnPosition, Quaternion.identity);
            trailer.Setup(enemy.Controller, hitchPoint);

            enemy.OnDespawnEvent += Enemy_OnDespawnEvent;
        }

        private void Enemy_OnDespawnEvent(Actor<EnemyContext> obj)
        {
            trailer.OnDeath();
        }
    }
}
