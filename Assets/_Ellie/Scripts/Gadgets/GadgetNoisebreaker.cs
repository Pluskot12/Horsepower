using Ellie.Audio;
using PrimeTween;
using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class GadgetNoisebreaker : GadgetAbility
    {
        [SerializeField] private GameObject antenna;
        [SerializeField] private SpriteRenderer wave;
        //[SerializeField] private GameObject explosionParts;
        //[SerializeField] private int shield = 100;
        [SerializeField] private float duration = 10;
        [SerializeField] private float radius = 10;
        [SerializeField] private float knockbackForce = 10;

        [Header("Audio")]
        [SerializeField] private AudioClip[] hitSounds;
        [SerializeField] private AudioClip activateSound;
        [SerializeField] private AudioClip waveSound;

        Vector3 antennaStartPos;

        public override void OnEquip(Player player)
        {
            //player.SetShield(shield);
            antennaStartPos = antenna.transform.localPosition;
            wave.transform.localScale = Vector3.zero;

            player.OnAttacked += OnHit;
        }

        public override void OnUnequip(Player player)
        {
            //player.SetShield(0);
            player.OnAttacked -= OnHit;

            StopAllCoroutines();
        }

        public override bool TryActivate(Player player)
        {
            StartCoroutine(ShieldCoroutine());

            return true;
        }

        bool shieldActive;
        private IEnumerator ShieldCoroutine()
        {
            SoundManager.PlaySFX(activateSound, transform.position);
            // shieldActive = true;
            // player.SetShield(true);

            Sequence.Create()
                .Chain(Tween.LocalPositionY(antenna.transform, 0.369f, 0.2f))
                .ChainCallback(() => Zap())
                .Chain(Tween.Alpha(wave, 0, 1f, 1))
                .Group(Tween.Scale(wave.transform, 0, 1f, 1f, ease: Ease.OutBack))

                .Chain(Tween.Alpha(wave, 0, 1))
                .Chain(Tween.LocalPositionY(antenna.transform, antennaStartPos.y, 0.2f));


            yield return new WaitForSeconds(duration);



            //SoundManager.PlaySFX(destroyedSound, transform.position);

            //Tween.Scale(forceShield.transform, 0f, 0.1f);


            // var instance = Instantiate(explosionParts, transform.position, transform.rotation);
            //   instance.SetActive(true);
            // Destroy(instance, 10f);
            // shieldActive = false;
            // player.SetShield(false);
        }

        private void Zap()
        {
            SoundManager.PlaySFX(waveSound, transform.position);

            var enemies = EnemySpawnManager.Instance.GetEnemiesWithinRadius(transform, radius);

            foreach (var enemy in enemies)
            {
                enemy.DeAggro();
                Vector2 knockbackDir = new Vector2(enemy.transform.position.x - transform.position.x, 0).normalized;
                enemy.Knockback(knockbackDir * knockbackForce);
            }
        }

        public void OnHit(int damage)
        {/*
            if (shieldActive)
            {
                SoundManager.PlayRandomSFX(hitSounds, transform.position);
            }*/
        }

    }
}
