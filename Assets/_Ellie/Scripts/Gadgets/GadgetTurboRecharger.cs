using Ellie.Audio;
using PrimeTween;
using System;
using UnityEngine;

namespace CarGame
{
    public class GadgetTurboRecharger : GadgetAbility
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField, Range(0, 1f)] private float activationChance = 0.5f;
        //[SerializeField] private float rechargeRate = 5f;
        [SerializeField] private float amount = 1;
        [SerializeField] private AudioClip activateSound;
        [SerializeField] private float fadeTime;

        private void Start()
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        }

        float eventTimer;
        private void Update()
        {
            /*
            if (!player.Turbo.IsActive || gadget.OnCooldown)
            {
                eventTimer = 0f;
                return;
            }*/

            eventTimer += Time.deltaTime;
            if (eventTimer >= 1f)
            {
                eventTimer = 0f;
                TryActivate();
            }
        }

        public void TryActivate()
        {
            if (UnityEngine.Random.value < activationChance)
            {
                GameManager.Instance.Player.AddTurbo(amount);

                Tween.StopAll(this);
                Tween.Alpha(spriteRenderer, 0f, 1f, fadeTime);
                Tween.Alpha(spriteRenderer, 1f, 0f, fadeTime, startDelay: fadeTime + 0.1f);
                //StartCoroutine(MaterialAnimation());

                SoundManager.PlaySFX(activateSound, transform.position);

                gadget.StartCooldown(Cooldown);
            }


        }

        public override void OnEquip(Player player)
        {
            StopAllCoroutines();
        }


        /*
               private IEnumerator MaterialAnimation()
               {
                   Tween.Alpha(spriteRenderer, 1, fadeTime);
                   //yield



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
    }
}
