using Ellie.Audio;
using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class EnemyRamAttack : MonoBehaviour
    {
        [SerializeField] private int damage = 10;
        [SerializeField] private float camereShake = 1f;
        [SerializeField] private AudioClip[] hitSounds;
        [SerializeField] private EnemyController enemy;
        [SerializeField] float verticalKnockbackFactor = 0f;
        [SerializeField] float cooldown = 5f;
        [Header("Knockback Settings")]
        [SerializeField] float knockbackForce = 1f;
        [SerializeField] float speedNeeded = 0.5f;

        private bool onCooldown;

        private bool CanRam()
        {
            if (onCooldown)
            {
                return false;
            }

            if (enemy.MaxSpeed == enemy.IdleSpeed)
            {
                return false;
            }

            return true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Debug.Log("Trigger " + enemy.Velocity.x + " " + enemy.MaxSpeed);


            if (!CanRam())
            {
                return;
            }

            float p = Mathf.Abs(enemy.Velocity.x) / enemy.MaxSpeed;
            //Debug.Log("Trigger " + p);

            if (p < speedNeeded)
            {
                return;
            }

            if (collision.attachedRigidbody)
            {
                if (collision.attachedRigidbody.TryGetComponent<CarController>(out CarController car))
                {
                    Vector2 knockbackDir = new Vector2(car.transform.position.x - enemy.transform.position.x, verticalKnockbackFactor * knockbackForce).normalized;
                    Knockback(car, knockbackDir);

                    if (car.TryGetComponent<IDamageable>(out IDamageable c))
                    {
                        c.TryDamage(damage, enemy.gameObject);
                    }

                    SoundManager.PlayRandomSFX(hitSounds, transform.position);
                    CameraManager.Instance.Shake(camereShake);

                    StartCoroutine(Cooldown());
                }
            }
        }

        private IEnumerator Cooldown()
        {
            onCooldown = true;

            yield return new WaitForSeconds(cooldown);

            onCooldown = false;
        }

        private void Knockback(CarController car, Vector2 dir)
        {
            float finalForce = knockbackForce;
            car.Knockback(dir * finalForce);
        }
    }
}
