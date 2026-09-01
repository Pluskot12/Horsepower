using Ellie.Audio;
using PrimeTween;
using UnityEngine;

namespace CarGame
{
    public class GadgetMooseHoof : GadgetAbility
    {
        [SerializeField] private float duration = 10;
        [SerializeField] private float onRotation = 0;
        [SerializeField] private float offRotation = 0;
        [SerializeField] private float animationDuration = 0.35f;
        [SerializeField] private Transform rotationPivot;
        [SerializeField] private Collider2D hitCollider;
        [SerializeField] private AudioClip useSound;
        [SerializeField] private AudioClip[] hitSounds;
        [SerializeField] private int damage;

        [Header("Knockback Settings")]
        [SerializeField] float knockbackForce = 1f;
        [SerializeField] float verticalKnockbackFactor = 1f;


        private bool active;

        public override bool TryActivate(Player player)
        {
            /*
            if (projectileReady)
            {
                Fire();

                return true;
            }
            */

            Activate();

            return true;
        }

        private void Activate()
        {

            SoundManager.PlaySFX(useSound, transform.position);

            Sequence.Create()
                .Group(Tween.LocalRotation(rotationPivot, new Vector3(0, 0, onRotation), animationDuration, ease: Ease.OutBack))
                .ChainCallback(() => { hitCollider.enabled = true; })
                .ChainDelay(duration)
                .Chain(Tween.LocalRotation(rotationPivot, new Vector3(0, 0, offRotation), animationDuration, ease: Ease.OutBack))
                .ChainCallback(() => { hitCollider.enabled = false; });
        }

        public override void OnEquip(Player player)
        {
            rotationPivot.localRotation = Quaternion.Euler(0, 0, offRotation);
            hitCollider.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.attachedRigidbody)
            {
                if (collision.attachedRigidbody.TryGetComponent<CarController>(out CarController car))
                {
                    Vector2 knockbackDir = new Vector2(car.transform.position.x - player.CarController.transform.position.x, verticalKnockbackFactor).normalized;
                    Knockback(car, knockbackDir);

                    if (car.TryGetComponent<IDamageable>(out IDamageable c))
                    {
                        c.TryDamage(damage, player.gameObject);
                    }

                    SoundManager.PlayRandomSFX(hitSounds, transform.position);

                    gadget.StartCooldown(Cooldown);

                    CameraManager.Instance.Shake(1f);
                }
            }
        }

        private void Knockback(CarController car, Vector2 dir)
        {
            float finalForce = knockbackForce;
            car.Knockback(dir * finalForce);
        }

    }
}
