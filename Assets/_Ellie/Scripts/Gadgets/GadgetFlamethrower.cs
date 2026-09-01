using PrimeTween;
using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class GadgetFlamethrower : GadgetAbility
    {
        [SerializeField] private float duration;
        [SerializeField] private float fireRate;
        [SerializeField] private float speed;
        [SerializeField] private int damage;
        [SerializeField] private LayerMask hitMask;
        [SerializeField] private Projectile prefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private AudioClip activationSound;
        [SerializeField] private AudioSource source;

        private Material mat;

        private int drawOrder;

        private bool equipped;

        public override bool TryActivate(Player player)
        {
            drawOrder = 10000;
            source.clip = activationSound;
            source.Play();
            //SoundManager.PlaySFX(activationSound, transform.position);

            StartCoroutine(Fire());
            Tween.Alpha(spriteRenderer, 0f, 1f, duration);
            Tween.Alpha(spriteRenderer, 1f, 0f, 5f, startDelay: duration + 3f);

            return true;
        }

        public override void OnEquip(Player player)
        {
            equipped = true;

            mat = spriteRenderer.material;
        }

        public override void OnUnequip(Player player)
        {
            equipped = false;
            StopAllCoroutines();
        }
        [SerializeField] private float spread = 5;
        /*
               private IEnumerator MaterialAnimation()
               {

                   yield return new WaitForSeconds(1f);
                   Tween.Alpha(spriteRenderer, 0f, 5f);

                   float progress = 0f;

                   while (progress < 1f)
                   {
                       progress += Time.deltaTime / duration;
                       mat.SetFloat("_Progress", Mathf.Clamp01(progress));
                       yield return null;
                   }

                   mat.SetFloat("_Progress", 1f);

                   yield return new WaitForSeconds(1f);

                   while (progress > 0f)
                   {
                       progress -= Time.deltaTime / 5f;
                       mat.SetFloat("_Progress", Mathf.Clamp01(progress));
                       yield return null;
                   }

                   mat.SetFloat("_Progress", 0f);
               }*/
        private IEnumerator Fire()
        {
            float elapsed = 0f;

            while (equipped && elapsed < duration)
            {
                float baseAngle = firePoint.eulerAngles.z;
                float spreadAngle = baseAngle + UnityEngine.Random.Range(-spread, spread);

                if (firePoint.lossyScale.x < 0)
                {
                    spreadAngle = baseAngle - UnityEngine.Random.Range(-spread, spread) + 180f;
                }

                Quaternion rotation = Quaternion.Euler(0, 0, spreadAngle);
                var instance = Instantiate(prefab, firePoint.position, rotation);
                instance.Setup(firePoint.position, (instance.transform.right * speed) + player.Velocity, damage, 1f, hitMask);
                //instance.Setup(firePoint.position, instance.transform.right * speed, damage, 1f, hitMask);

                instance.SetDrawOrder(drawOrder);
                drawOrder++;

                yield return new WaitForSeconds(fireRate);
                elapsed += fireRate;
            }
        }
    }
}
