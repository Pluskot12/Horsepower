using Ellie.Audio;
using PrimeTween;
using UnityEngine;

namespace CarGame
{
    public class GadgetTitaniumBumper : GadgetAbility
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D damageCollider;
        [SerializeField] private int chargedDamage = 20;
        [SerializeField] private int superDamage = 50;
        [SerializeField] private float critChance = 0.1f;

        [Header("Animation")]
        [SerializeField] private float fadeTime;

        [Header("Sounds")]
        [SerializeField] private AudioClip normalSound;
        [SerializeField] private AudioClip critSound;
        [SerializeField] private AudioClip[] hitSounds;

        [Header("Knockback Settings")]
        [SerializeField] float knockbackForce = 1f;
        [SerializeField] float verticalKnockbackFactor = 1f;
        [SerializeField] float playerKnockbackMultiplier = 0.2f;

        private Material mat;

        private enum Charge
        {
            None,
            Charged,
            Super
        }

        private Charge currentCharge;

        public override void OnEquip(Player player)
        {
            this.player = player;

            mat = spriteRenderer.material;

            damageCollider.enabled = false;
        }

        private void Awake()
        {

        }

        bool wasActive;

        private void Update()
        {
            bool isActive = player.Dash.IsDashing && !gadget.OnCooldown;
            damageCollider.enabled = isActive;

            if (isActive && !wasActive)
            {
                RollCharge();
                if (currentCharge == Charge.Charged)
                {
                    SoundManager.PlaySFX(normalSound, transform.position);
                }
                else if (currentCharge == Charge.Super)
                {
                    SoundManager.PlaySFX(critSound, transform.position);
                }

                Tween.StopAll(this);
                Tween.Alpha(spriteRenderer, 1, fadeTime);
            }
            else if (!isActive && wasActive)
            {
                Tween.StopAll(this);
                Tween.Alpha(spriteRenderer, 0, fadeTime);
            }

            wasActive = isActive;

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.attachedRigidbody)
            {
                if (collision.attachedRigidbody.TryGetComponent<CarController>(out CarController car))
                {
                    Vector2 knockbackDir = new Vector2(car.transform.position.x - player.CarController.transform.position.x, verticalKnockbackFactor * KnockbackChargeMulti()).normalized;
                    Knockback(player.CarController, new Vector2(-knockbackDir.x * playerKnockbackMultiplier, Mathf.Abs(knockbackDir.y)));
                    Knockback(car, knockbackDir);

                    float multiplier = GetDamage();

                    if (car.TryGetComponent<IDamageable>(out IDamageable c))
                    {
                        c.TryDamage(GetDamage(), player.gameObject);
                    }

                    SoundManager.PlayRandomSFX(hitSounds, transform.position);

                    gadget.StartCooldown(Cooldown);

                    CameraManager.Instance.Shake(1f * KnockbackChargeMulti());
                }
            }
        }

        private void RollCharge()
        {
            if (UnityEngine.Random.value < critChance)
            {
                currentCharge = Charge.Super;
            }
            else
            {
                currentCharge = Charge.Charged;
            }
        }

        private void Knockback(CarController car, Vector2 dir)
        {
            float finalForce = knockbackForce * KnockbackChargeMulti();
            car.Knockback(dir * finalForce);
        }

        private int GetDamage()
        {
            switch (currentCharge)
            {
                case Charge.Charged: return chargedDamage;
                case Charge.Super: return superDamage;
                default: return 0;
            }
        }
        /*
        private IEnumerator MaterialAnimation()
        {
            float progress = 0f;

            while (progress < 1f)
            {
                progress += Time.deltaTime / fadeTime;
                mat.SetFloat("_Progress", Mathf.Clamp01(progress));
                yield return null;
            }

            mat.SetFloat("_Progress", 1f);

            yield return null;

            while (progress > 0f)
            {
                progress -= Time.deltaTime / fadeTime;
                mat.SetFloat("_Progress", Mathf.Clamp01(progress));
                yield return null;
            }

            mat.SetFloat("_Progress", 0f);
        }*/

        private float KnockbackChargeMulti()
        {
            switch (currentCharge)
            {
                case Charge.None: return 1f;
                case Charge.Charged: return 1f;
                case Charge.Super: return 2f;
                default: return 1f;
            }
        }
    }
}
