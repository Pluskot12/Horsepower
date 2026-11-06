using Demo.Perception;
using Ellie.Audio;
using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class Infection : MonoBehaviour
    {
        [SerializeField] private EnemyData infectedData;
        [SerializeField] private Enemy self;
        [SerializeField] private Enemy enemyTest;
        [SerializeField] private float infectionChance = 1f;
        [SerializeField] private float infectionTime = 30f;
        [SerializeField] private GameObject explosionParts;
        [SerializeField] private AudioClip[] infectionAlertSound;
        [SerializeField] private AudioClip infectionTransformSound;

        bool infected;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                OnBite(self, enemyTest);
            }
        }

        private void OnEnable()
        {
            self.OnHitEvent += OnBite;
            self.OnIdleSoundEvent += OnIdleSound;
        }

        private void OnIdleSound()
        {
            SoundManager.PlayRandomSFX(infectionAlertSound, self.transform.position);
        }

        private void OnDisable()
        {
            self.OnHitEvent -= OnBite;
            self.OnIdleSoundEvent -= OnIdleSound;
        }

        private void OnBite(Enemy self, IAttacker attacker)
        {
            if (infected)
            {
                return;
            }
            /*
            if (attacker == null)
            {
                return;
            }

            if (attacker.Faction != Ellie.Core.Faction.Infected)
            {
                return;
            }
            */
            float roll = Random.Range(0f, 100f);

            if (roll <= infectionChance)
            {
                StartCoroutine(Infect());
            }

            Debug.LogFormat("Infection Roll: {0} ({1}%)", roll, infectionChance);
        }

        private IEnumerator Infect()
        {
            yield return new WaitForSeconds(infectionTime);

            if (!self.IsDead)
            {
                self.SetInfected(true);
                self.OnDeath();

                explosionParts.SetActive(true);

                SoundManager.PlaySFX(infectionTransformSound, self.transform.position);

                var instance = EnemySpawnManager.Instance.SpawnEnemy(infectedData, self.transform.position, self.transform.rotation);
                instance.SetEnraged(GameManager.Instance.Player, 10f);
                //instance.SetSortingOrder(10);
            }
        }
    }
}
