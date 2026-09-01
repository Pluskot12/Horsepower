using Ellie.Audio;
using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class ParachuteCrate : MonoBehaviour
    {
        [SerializeField] private Building building;
        [SerializeField] private Chest chest;
        [SerializeField] private GameObject parachute;
        [SerializeField] private Collider2D groundCollider;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator animator;

        [SerializeField] private AudioClip spawnSound;
        [SerializeField] private AudioClip landSound;

        bool grounded;

        private void Awake()
        {
            building.gameObject.SetActive(false);
        }

        private void Start()
        {
            SoundManager.PlaySFX(spawnSound, transform.position);

            parachute.SetActive(false);
            rb.bodyType = RigidbodyType2D.Kinematic;

            StartCoroutine(Show());

            chest.OnDestroyed += Chest_OnDestroyed;
        }

        private IEnumerator Show()
        {
            yield return new WaitForSeconds(10f);

            parachute.SetActive(true);

            rb.bodyType = RigidbodyType2D.Dynamic;

            rb.AddForceX(30);

        }

        private void Chest_OnDestroyed()
        {
            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!grounded)
            {
                grounded = true;
                building.gameObject.SetActive(true);
                parachute.SetActive(false);
                animator.enabled = false;
                // groundCollider.enabled = false;
                // rb.bodyType = RigidbodyType2D.Static;

                //animator.speed = 0;
                chest.OnPlace(null);

                SoundManager.PlaySFX(landSound, transform.position);
            }
        }
    }
}
