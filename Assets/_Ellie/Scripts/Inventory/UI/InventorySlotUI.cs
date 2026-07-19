using Ellie.Audio;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static CarGame.InventoryUI;

namespace CarGame
{
    public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private Image itemParent;
        [SerializeField] private Image itemIcon;
        [SerializeField] private Image frame;
        [SerializeField] private Image cooldownSprite;
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private InventoryItem slottedItem;
        InventoryUI ui;

        private bool onCooldown;
        public bool OnCooldown => onCooldown;

        public GadgetBarUI gadgetBar;
        public int Index { get; private set; }

        public RectTransform RectTransform => rect;
        public CanvasGroup CanvasGroup => cg;
        public Image Frame => frame;
        public InventoryItem SlottedItem => slottedItem;
        public Transform ItemParent => itemParent.transform;

        public AllowedItem allowedItem = AllowedItem.All;

        public InventoryType ParentType => ui.Type;

        public enum SlotType
        {
            Inventory,
            ActionBar,
            Gadget,
            Chest
        }

        public enum AllowedItem
        {
            All,
            Bomb,
            GadgetBack,
            GadgetMid,
            GadgetFront
        }

        public SlotType Type = SlotType.Inventory;

        public void Setup(InventoryItem item)
        {
            slottedItem = item;

            if (item == null)
            {
                itemParent.gameObject.SetActive(false);
                itemIcon.sprite = null;
                quantityText.text = "";
            }
            else
            {
                itemIcon.sprite = item.ItemData.sprite;
                itemParent.gameObject.SetActive(true);

                if (item.Quantity > 1 || item.ItemData.GetType() == typeof(WeaponItemData))
                {
                    quantityText.text = item.Quantity.ToString();
                }
                else
                {
                    quantityText.text = "";
                }
            }
        }
        Coroutine cooldownCoroutine;
        public void StartCooldown(float time)
        {
            if (cooldownSprite != null)
            {
                if (cooldownCoroutine != null)
                {
                    return;
                }
                cooldownCoroutine = StartCoroutine(Cooldown(time));
            }
        }

        public void StopCooldown()
        {
            if (cooldownCoroutine != null)
            {
                StopCoroutine(cooldownCoroutine);
                cooldownCoroutine = null;
                cooldownSprite.fillAmount = 0;
                onCooldown = false;
            }


        }


        private IEnumerator Cooldown(float time)
        {
            onCooldown = true;
            float timer = 0;
            cooldownSprite.fillAmount = 1f;

            while (timer <= time)
            {
                cooldownSprite.fillAmount = 1 - (timer / time);
                timer += Time.deltaTime;
                yield return null;
            }


            cooldownSprite.fillAmount = 0;
            onCooldown = false;

            if (Type == SlotType.Gadget)
            {
                SoundManager.PlayUI(ui.GadgetRechargeSound);
            }

            cooldownCoroutine = null;
        }

        public void Refresh()
        {
            Setup(slottedItem);
        }

        public void Init(InventoryUI ui, int i)
        {
            this.ui = ui;
            Index = i;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (onCooldown)
            {
                return;
            }

            if (Type == SlotType.Gadget)
            {
                gadgetBar.OnSlotClicked(this, eventData);
            }
            else
            {
                ui?.OnSlotClicked(this, eventData);
            }

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ui.InventoryPanelU.OnItemSlotHover(this, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ui.InventoryPanelU.OnItemSlotHover(null, null);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            ui.InventoryPanelU.OnItemSlotHover(this, eventData);
        }

    }
}