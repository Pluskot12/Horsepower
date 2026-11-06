using Ellie.Audio;
using Horsepower.NPC;
using PrimeTween;
using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class TurretWeaponController : WeaponController
    {
        [Header("Turret")]
        [SerializeField] private Transform turretBase;
        [SerializeField] private Transform gunPivot;
        [SerializeField] private Transform gunVisual;
        [SerializeField] private float aimSpeed = 20;
        [SerializeField] private float returnSpeed = 15;

        [Header("Gun")]
        [SerializeField] private Projectile bulletPrefab;

        [SerializeField] private Transform firePoint;
        [SerializeField] private int rounds = 5;
        [SerializeField] private float fireRate = 0.2f;
        [SerializeField] private float bulletSpeed = 100f;

        [Header("Effects")]
        [SerializeField] private Animator muzzleFlash;
        [SerializeField] private AudioClip fireSound;

        private Coroutine attackCoroutine;

        public override void Tick(IWeaponContext context, float dt)
        {
            // Move turret
            RotateTurret();


            base.Tick(context, dt);
        }

        protected override void Attack()
        {
            attackCoroutine = StartCoroutine(AttackCoroutine());
        }

        private IEnumerator AttackCoroutine()
        {
            int bulletsRemaining = rounds;

            while (bulletsRemaining > 0)
            {
                SpawnBullet();
                Recoil();
                MuzzleFlash();

                EnemySpawnManager.Instance.OnNoiseGenerated(0.1f);
                SoundManager.PlaySFX(fireSound, transform.position);

                bulletsRemaining--;

                yield return new WaitForSeconds(fireRate);
            }

        }

        private void SpawnBullet()
        {
            Vector3 dir = firePoint.localToWorldMatrix.MultiplyVector(Vector3.right).normalized;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);

            var instance = Instantiate(bulletPrefab, firePoint.position, rot);
            instance.Setup(attacker, firePoint.position, dir * bulletSpeed, damage, 10, hitLayer);
        }

        private void Recoil()
        {
            Tween.PunchLocalPosition(gunVisual, new Vector3(-0.1f * gunVisual.localScale.x, 0, 0f), 0.15f);
        }

        private void MuzzleFlash()
        {
            muzzleFlash.Play("Shoot", 0, 0);
        }

        private void RotateTurret()
        {
            if (context.Target)
            {

                float dirToTarget = context.Target.transform.position.x - context.Transform.position.x;

                float direction = Mathf.Sign(dirToTarget);

                turretBase.localScale = new Vector3(direction * context.Transform.localScale.x, 1f, 1f);

                Vector2 worldDirection = context.Target.transform.position - gunPivot.position;
                Vector2 localDirection = gunPivot.parent.InverseTransformDirection(worldDirection);

                float angle = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;
                angle = Mathf.Clamp(angle, -10f, 40f);

                gunPivot.localRotation = Quaternion.RotateTowards(gunPivot.localRotation, Quaternion.Euler(0f, 0f, angle), Time.deltaTime * aimSpeed);
                bool flippedY = angle > 90f || angle < -90f;

                gunVisual.localScale = new Vector3(1f, flippedY ? -1f : 1f, 1f);
            }
            else
            {
                gunPivot.localRotation = Quaternion.RotateTowards(gunPivot.localRotation, Quaternion.identity, Time.deltaTime * returnSpeed);
            }
        }
    }
}
