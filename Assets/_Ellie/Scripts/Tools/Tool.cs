using Ellie.Audio;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace CarGame
{
    public class Tool : MonoBehaviour
    {
        private const float INITIAL_COOLDOWN = 0.75f;

        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject explodingParts;

        [Header("Tool Settings")]
        [SerializeField] private float swingCooldown = 0.75f;

        [Header("Audio")]
        [SerializeField] private AudioClip useClip;
        [SerializeField] private AudioClip equipClip;


        [Header("Events"), Space(2)]
        public UnityEvent<Tool> OnToolAction;

        private int slot;
        private ToolItemData data;
        private bool onCooldown;


        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                TryUse();
            }
        }

        public void Setup(Transform t, int slot, ToolItemData data, bool playSound)
        {
            this.slot = slot;
            this.data = data;

            onCooldown = true;

            if (playSound)
            {
                SoundManager.PlaySFX(equipClip, transform.position);
            }

            StartCoroutine(CooldownCoroutine(INITIAL_COOLDOWN));
        }

        public void OnDeselect()
        {
            StopAllCoroutines();

            Destroy(gameObject);
        }

        private void TryUse()
        {
            if (CanSwing())
            {
                SoundManager.PlaySFX(useClip, transform.position);

                if (animator)
                {
                    animator.SetBool("CanSwing", false);
                    animator.Play("Swing");
                }

                StartCoroutine(CooldownCoroutine(swingCooldown));
            }
        }

        private IEnumerator CooldownCoroutine(float duration)
        {
            onCooldown = true;

            yield return new WaitForSeconds(duration);

            onCooldown = false;
        }

        private bool CanSwing()
        {
            if (UIMananger.IsPointerOverUIObject())
            {
                return false;
            }

            if (UIMananger.IsHoldingItem)
            {
                return false;
            }

            if (animator && animator.GetBool("CanSwing") == false)
            {
                return false;
            }

            if (onCooldown)
            {
                return false;
            }


            return true;
        }


        public void OnItemBreak()
        {
            if (explodingParts == null)
            {
                return;
            }

            explodingParts.transform.SetParent(null);
            explodingParts.SetActive(true);
            Destroy(explodingParts, 5f);
        }

        public void UpdateDurability()
        {
            PlayerInventory.Instance.InventoryController.DamageItem(slot);
        }

        #region Animation Events

        private void OnAnimationHit()
        {
            OnToolAction.Invoke(this);
        }

        #endregion

    }
}
