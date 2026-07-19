using Ellie.Audio;
using PrimeTween;
using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class GadgetForceShield : GadgetAbility
    {
        [SerializeField] private SpriteRenderer forceShield;
        [SerializeField] private GameObject explosionParts;
        //[SerializeField] private int shield = 100;
        [SerializeField] private float duration = 10;

        [Header("Audio")]
        [SerializeField] private AudioClip[] hitSounds;
        [SerializeField] private AudioClip activateSound;
        [SerializeField] private AudioClip destroyedSound;

        public override void OnEquip(Player player)
        {
            //player.SetShield(shield);
            forceShield.transform.localScale = Vector3.zero;

            player.OnAttacked += OnHit;
        }

        public override void OnUnequip(Player player)
        {
            //player.SetShield(0);
            player.OnAttacked -= OnHit;

            StopAllCoroutines();
        }

        public override void OnActivate(Player player)
        {
            StartCoroutine(ShieldCoroutine());
        }
        bool shieldActive;
        private IEnumerator ShieldCoroutine() 
        {
            SoundManager.PlaySFX(activateSound, transform.position);
            shieldActive = true;
            player.SetShield(true);

            Tween.Alpha(forceShield, 0, 1f, 0.25f);
            Tween.Scale(forceShield.transform, 0, 1f, 0.25f, ease: Ease.OutBack);

            yield return new WaitForSeconds(duration);

            SoundManager.PlaySFX(destroyedSound, transform.position);

            Tween.Scale(forceShield.transform, 0f, 0.1f);
            Tween.Alpha(forceShield, 0, 0.075f);

            var instance = Instantiate(explosionParts, transform.position, transform.rotation);
            instance.SetActive(true);
            Destroy(instance, 10f);
            shieldActive = false;
            player.SetShield(false);
        }

        public void OnHit(int damage) 
        {
            if (shieldActive) 
            { 
                SoundManager.PlayRandomSFX(hitSounds, transform.position);
            }
        }

    }
}
