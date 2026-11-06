using Ellie.Audio;
using PrimeTween;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CarGame
{
    public class TrashCanSlot : InventorySlotUI
    {
        [SerializeField] private float destroyDelay = 1f;
        [SerializeField] private AudioClip destroySound;
        public override void OnPointerClick(PointerEventData eventData)
        {
            //Debug.Log("Slotted item " + ui.InventoryPanelU.he);
            if (UIMananger.HeldItem != null && SlottedItem != null)
            {
                PlayerGadgets.Instance.GadgetController.RemoveAtIndex(Index);
                SoundManager.PlayUI(destroySound);
            }

            ui?.OnSlotClicked(this, eventData);
            /*
   
            else
            {
                Cancel();
            }*/
        }

        private void DestroyItem()
        {
            cooldownCoroutine = StartCoroutine(Cooldown(0));
        }

        protected override IEnumerator Cooldown(float time)
        {
            onCooldown = true;
            float timer = 0;
            cooldownSprite.color = Color.white;
            cooldownSprite.fillAmount = 0f;

            while (timer <= time)
            {
                cooldownSprite.fillAmount =/* 1 -*/ (timer / time);
                timer += Time.deltaTime;
                yield return null;
            }

            Tween.Alpha(cooldownSprite, 0f, 0.25f);
            //cooldownSprite.fillAmount = 0;
            onCooldown = false;



            cooldownCoroutine = null;


        }

        private void Cancel()
        {
            StopCooldown();
        }
    }
}
