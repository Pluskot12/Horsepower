using Ellie.Audio;
using System.Collections.Generic;
using UnityEngine;

namespace CarGame
{
    public class BombExplosion : MonoBehaviour
    {
        [SerializeField] private AudioClip explosionSound;
        [SerializeField] private LayerMask damageableLayers;

        [Header("Settings")]
        [SerializeField] protected float blastRadius = 1;
        [SerializeField] protected int explosionDamage = 10;

        private HashSet<IDamageable> targets;

        private void Awake()
        {
            Debug.Log("C");
            //gameObject.SetActive(false);
        }

        public void Explode()
        {
            Explode(explosionDamage, blastRadius);
        }

        public void Explode(int damage, float blastRadius)
        {
            transform.SetParent(null);
            gameObject.SetActive(true);

            CameraManager.Instance.Shake(2);

            SoundManager.PlaySFX(explosionSound, transform.position);

            //audioSource.transform.SetParent(null);
            //Destroy(audioSource.gameObject, audioSource.clip.length * 2f);

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, blastRadius, damageableLayers);

            targets = new HashSet<IDamageable>();

            foreach (Collider2D hit in hits)
            {
                if (hit.attachedRigidbody == null)
                    continue;

                IDamageable target = hit.attachedRigidbody.GetComponent<IDamageable>();

                if (targets.Contains(target))
                {
                    continue;
                }

                if (target != null)
                {
                    targets.Add(target);
                    ApplyEffect(hit, target, damage);
                }
            }

            OnExplode();
        }

        protected virtual void OnExplode()
        {

        }

        protected virtual void ApplyEffect(Collider2D hit, IDamageable target, int damage)
        {
            target.TryDamage(damage, gameObject);
        }


    }
}
