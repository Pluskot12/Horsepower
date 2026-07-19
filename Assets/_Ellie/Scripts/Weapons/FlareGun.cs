using Ellie.Audio;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace CarGame
{
    public class FlareGun : MonoBehaviour
    {
        [SerializeField] private Transform visuals;
        [SerializeField] private FlareExplosion flareExplosion;
        [SerializeField] private Tool toolbase;

        [Header("Settings")]
        [SerializeField] private float bulletSpeed;
        [SerializeField] private float bulletLifetime;
        [SerializeField] private float noise;
        [SerializeField] private float noiseTickrate;
        [SerializeField] private float explosionLifetime;

        [SerializeField] private Transform bulletSpawn;
        [SerializeField] private Projectile bullet;

        [SerializeField] private AudioClip shootClip;
        [SerializeField] private AudioClip explosionClip;

        bool isActive;

        private void OnEnable()
        {
            CinemachineCore.CameraUpdatedEvent.AddListener(OnCinemachineUpdated);
        }

        private void OnDisable()
        {
            CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCinemachineUpdated);
        }

        private void OnCinemachineUpdated(CinemachineBrain brain)
        {
            if (isActive)
            {
                //return;
            }

            if (GameManager.GamePaused)
            {
                return;
            }

            visuals.right = Vector2.right;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isActive = true;

                // Tween.LocalRotation(visuals, new Vector3(0, 0, 75), 0.35f, ease: Ease.OutElastic);
            }
        }

        public void OnShoot()
        {
            float flip = Mathf.Sign(transform.lossyScale.x);
            Vector3 shootDirection = bulletSpawn.right * flip;

            var instance = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
            instance.Setup(bulletSpawn.position, (shootDirection * bulletSpeed) + GameManager.Instance.Player.Velocity, 0, 5f, 0);

            SoundManager.PlaySFX(shootClip, transform.position);

            StartCoroutine(Explode(instance));
        }

        IEnumerator Explode(Projectile p)
        {
            yield return new WaitForSeconds(bulletLifetime);
            var instance = Instantiate(flareExplosion, p.transform.position, Quaternion.identity);
            instance.Setup(noise, noiseTickrate, explosionLifetime);

            SoundManager.PlaySFX(explosionClip, p.transform.position);

            Destroy(p.gameObject);

            toolbase.UpdateDurability();
        }
    }
}
