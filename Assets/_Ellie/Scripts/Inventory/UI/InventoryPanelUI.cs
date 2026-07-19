using PrimeTween;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CarGame
{
    public class InventoryPanelUI : MonoBehaviour, IPointerMoveHandler
    {
        public static InventoryPanelUI Instance { get; private set; }

        [SerializeField] private Canvas canvas;
        [SerializeField] private GraphicRaycaster raycaster;

        [SerializeField] private InventoryUI inventory;
        [SerializeField] private ActionBarUI actionBar;
        [SerializeField] private InventoryUI gadgetBar;

        [SerializeField] private RectTransform inventoryRect;
        [SerializeField] private RectTransform inventoryParent;
        [SerializeField] private InventorySlotUI inventorySlotPrefab;
        [SerializeField] private Canvas rootCanvas;

        [Header("Crafting")]
        [SerializeField] private CraftingMenuUI craftingListPanel;
        [SerializeField] private CraftingRecipePanelUI craftingRecipePanel;

        [Header("Inventory Button")]
        [SerializeField] private Image inventoryButton;
        [SerializeField] private GameObject inventoryButtonOpen;
        //[SerializeField] private Sprite inventoryButtonOpen;
        [SerializeField] private GameObject inventoryButtonClose;

        [Header("Animation Settings")]
        [SerializeField] private float offPosition = -270f;
        [SerializeField] private float inDuration = 0.2f;

        [Header("Sounds")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip openAudio;
        [SerializeField] private AudioClip closeAudio;
        [SerializeField] private AudioClip selectAudio;
        [SerializeField] private AudioClip splitAudio;
        [SerializeField] private AudioClip placeAudio;
        [SerializeField] private AudioClip throwAudio;
        [SerializeField] private AudioClip bombPlacementSound;
        [SerializeField] private AudioClip gadgetPlacementSound;
        [SerializeField] private AudioClip gadgetCooldownSound;
        [SerializeField] private AudioClip itemBreakAudio;

        private Camera cam;

        private bool isShowing;

        public AudioClip GadgetRechargeSound => gadgetCooldownSound;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            cam = Camera.main;

            inventoryParent.anchoredPosition = new Vector2(0, offPosition);
            ShowInventory(isShowing, false);
            craftingListPanel.Show(isShowing, false);
            craftingRecipePanel.Show(isShowing, false);

            inventory.Init(this);
            gadgetBar.Init(this);
            //actionBar.Init(this);

            itemHoverText.gameObject.SetActive(false);

            CreateClone();
        }

        public void Show()
        {
            //isShowing = false;
            //ShowInventory(false, false);
            canvas.enabled = true;
        }

        public void Hide()
        {
            isShowing = false;
            ShowInventory(false, false);
            canvas.enabled = false;

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab) && !GameManager.Instance.Player.IsDead && !GameManager.GamePaused)
            {
                OnInventoryButton();
            }

            if (Input.GetKeyUp(KeyCode.T))
            {
                if (clonedSlot.SlottedItem != null)
                {
                    DropItem(clonedSlot, Input.mousePosition);
                    RemoveClone();
                }
                else
                {
                    var slotToDrop = actionBar.ActiveSlot();
                    if (slotToDrop.SlottedItem != null)
                    {
                        DropItem(slotToDrop, Input.mousePosition);
                        inventory.DropItemAtIndex(slotToDrop, Input.mousePosition);
                    }
                }

            }

            if (clonedSlot.SlottedItem != null)
            {
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rootCanvas.transform as RectTransform, Input.mousePosition, rootCanvas.worldCamera, out pos);
                clonedSlot.transform.localPosition = pos;

                if (Input.GetMouseButtonDown(0))
                {
                    bool isInside = IsInsideInventoryRect(Input.mousePosition);

                    if (!isInside)
                    {
                        DropItem(clonedSlot, Input.mousePosition);
                    }
                }
            }
        }

        private void UpdateButtonSprite(bool animate = true)
        {
            if (isShowing)
            {
                inventoryButtonOpen.SetActive(false);
                inventoryButtonClose.SetActive(true);
                //inventoryButton.sprite = inventoryButtonClose;
            }
            else
            {
                inventoryButtonOpen.SetActive(true);
                inventoryButtonClose.SetActive(false);
                //inventoryButton.sprite = inventoryButtonOpen;
            }

            if (animate)
            {
                Tween.PunchScale(inventoryButton.transform, strength: Vector3.one * 0.2f, duration: .3f, frequency: 7);
            }
        }

        public void OnInventoryButton()
        {
            isShowing = !isShowing;

            ShowInventory(isShowing);

            craftingListPanel.Show(isShowing);
            craftingRecipePanel.Show(isShowing);

            if (!isShowing)
            {
                SetSecondary(null);
            }
        }

        public void OnChestInteraction(bool open, IPanelUI chest)
        {
            isShowing = open;
            ShowInventory(open);

            if (open)
            {
                SetSecondary(chest);
            }
            else
            {
                SetSecondary(null);
            }

            if (open == false && craftingListPanel.IsShowing)
            {
                craftingListPanel.Show(false);
                craftingRecipePanel.Show(false);
            }
        }

        public bool TryClose()
        {
            if (isShowing)
            {
                isShowing = false;
                ShowInventory(false);
                SetSecondary(null);
                return true;
            }

            return false;
        }


        private void ShowInventory(bool show, bool animate = true)
        {
            UpdateButtonSprite(animate);
            if (animate)
            {
                audioSource.PlayOneShot(show ? openAudio : closeAudio);

                Animate();

                GameManager.Instance.Player.OnInventory(show);
            }
            else
            {
                Vector2 pos = inventoryParent.anchoredPosition;
                pos.y = isShowing ? 0 : offPosition;
                inventoryParent.anchoredPosition = pos;
            }
        }

        private void Animate()
        {
            Tween.UIAnchoredPositionY(inventoryParent, endValue: isShowing ? 0 : offPosition, duration: inDuration, ease: Ease.InOutQuart);
        }

        public bool IsInsideInventoryRect(PointerEventData eventData)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(
                inventoryRect,
                eventData.position,
                eventData.pressEventCamera
            );
        }

        IPanelUI secondaryInventory;

        private void SetSecondary(IPanelUI secondary)
        {
            if (secondaryInventory != null)
            {
                secondaryInventory.Hide(true);
            }

            secondaryInventory = secondary;
        }

        public int HandleShiftShortcut(InventorySlotUI clickedSlot)
        {
            if (secondaryInventory == null)
            {
                return -1; // ?
            }

            Inventory from;
            Inventory to;

            if (clickedSlot.ParentType == InventoryUI.InventoryType.Player)
            {
                from = PlayerInventory.Instance.InventoryController.Inventory;
                to = secondaryInventory.Inventory;
            }
            else if (clickedSlot.ParentType == InventoryUI.InventoryType.Chest)
            {
                from = secondaryInventory.Inventory;
                to = PlayerInventory.Instance.InventoryController.Inventory;
            }
            else
            {
                return -1;
            }

            int count = clickedSlot.SlottedItem.Quantity;
            int remaining = to.TryAdd(clickedSlot.SlottedItem);
            int remove = count - remaining;

            if (clickedSlot.SlottedItem.ItemData is WeaponItemData)
            {
                from.TryRemoveAtIndex(clickedSlot.Index);
            }
            else
            {
                from.TryRemoveQuantityAtIndex(clickedSlot.Index, remove);
            }
            //int remaining = secondaryInventory.Inventory.TryAdd(clickedSlot.SlottedItem);

            return 0;
        }

        private void HideSecondary(bool animate)
        {
            if (secondaryInventory == null)
            {
                return;
            }


            secondaryInventory.Hide(animate);


            secondaryInventory = null;
        }


        public bool IsInsideInventoryRect(Vector2 mousePosition)
        {
            if (secondaryInventory != null)
            {
                bool main = RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, mousePosition);
                bool secondary = RectTransformUtility.RectangleContainsScreenPoint(secondaryInventory.Rect, mousePosition);
                return main || secondary;
            }

            return RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, mousePosition);
        }

        public void DropItem(InventorySlotUI inventorySlotUI, Vector2 screenPos)
        {
            audioSource.PlayOneShot(throwAudio);

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(screenPos);
            mouseWorld.z = 0f;
            Vector2 direction = (mouseWorld - GameManager.Instance.Player.transform.position).normalized;

            OnItemDropped(inventorySlotUI, direction);

            RemoveClone();
        }


        public void OnItemDropped(InventorySlotUI item, Vector3 force)
        {
            Vector3 position = GameManager.Instance.Player.transform.position;
            ItemSpawner.Instance.DropItem(item.SlottedItem.ItemData, item.SlottedItem.Quantity, item.SlottedItem.Durability, position, force, true);
        }

        [SerializeField] private InventorySlotUI clonedSlot;
        public InventorySlotUI ClonedSlot => clonedSlot;



        private void CreateClone()
        {
            // clonedSlot = Instantiate(inventorySlotPrefab, transform.root);
            clonedSlot.Frame.enabled = false;
            clonedSlot.CanvasGroup.blocksRaycasts = false;
            clonedSlot.transform.localScale *= 1.15f;
            clonedSlot.Setup(null);
        }

        public void ShowClone(InventoryItem slot)
        {
            var slottedItem = new InventoryItem(slot.ItemData, slot.Quantity, slot.Durability);

            itemHoverText.gameObject.SetActive(false);
            clonedSlot.gameObject.SetActive(true);
            clonedSlot.transform.SetAsLastSibling();

            clonedSlot.Setup(slottedItem);

            UIMananger.IsHoldingItem = slottedItem != null;
            UIMananger.HeldItem = slottedItem;
        }

        public void RemoveClone()
        {
            clonedSlot.Setup(null);
            clonedSlot.gameObject.SetActive(false);
            UIMananger.HeldItem = null;

            itemHoverText.gameObject.SetActive(true);
            //OnItemSlotHover(lastHoverItem, null);

            StartCoroutine(RenableShooting());
        }

        public void OnDeath()
        {
            RemoveClone();
        }

        private IEnumerator RenableShooting()
        {
            yield return new WaitForSeconds(0.1f);

            UIMananger.IsHoldingItem = false;
        }


        public void OnItemPickup()
        {
            audioSource.PlayOneShot(placeAudio);
        }

        public void OnRefresh()
        {
            actionBar.OnInventoryRefreshed();
        }
        public void SetInteractable(bool enable)
        {
            raycaster.enabled = enable;
        }

        public void OnBombPlaced()
        {
            audioSource.PlayOneShot(bombPlacementSound);
        }

        public void OnGadgetPlaced()
        {
            audioSource.PlayOneShot(gadgetPlacementSound);
        }

        public void OnItemSelected()
        {
            audioSource.PlayOneShot(selectAudio);
        }

        public void OnItemPlaced()
        {
            audioSource.PlayOneShot(placeAudio);
        }

        public void OnItemSplit()
        {
            audioSource.PlayOneShot(splitAudio);
        }

        internal void OnItemBreak()
        {
            audioSource.PlayOneShot(itemBreakAudio);
        }

        [Header("Hovering Text")]
        [SerializeField] private TextMeshProUGUI itemHoverText;
        [SerializeField] private Vector2 itemOverTextOffset;

        private bool isHoveringItem;
        PointerEventData lastHoverPointerData;

        public void OnItemSlotHover(InventorySlotUI slot, PointerEventData eventData)
        {
            if (slot == null || slot.SlottedItem == null)
            {
                itemHoverText.text = "";
                itemHoverText.gameObject.SetActive(false);
                isHoveringItem = false;
            }
            else
            {
                Canvas rootCanvas = itemHoverText.canvas.rootCanvas;
                RectTransform canvasRect = rootCanvas.transform as RectTransform;
                Vector2 scaledOffset = itemOverTextOffset / rootCanvas.scaleFactor;

                if (eventData == null)
                {
                    eventData = lastHoverPointerData;
                }

                lastHoverPointerData = eventData;

                if (rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    itemHoverText.rectTransform.position = (Vector3)(eventData.position + itemOverTextOffset);
                }
                else
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvasRect,
                        eventData.position,
                        rootCanvas.worldCamera,
                        out Vector2 localPoint);

                    itemHoverText.rectTransform.anchoredPosition = localPoint + scaledOffset;
                }

                itemHoverText.text = slot.SlottedItem.ItemData.displayName;
                //itemHoverText.transform.position = eventData.position + itemOverTextOffset;
                itemHoverText.gameObject.SetActive(true);
                isHoveringItem = true;
            }
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (isHoveringItem)
            {
                Canvas rootCanvas = itemHoverText.canvas.rootCanvas;
                RectTransform canvasRect = rootCanvas.transform as RectTransform;
                Vector2 scaledOffset = itemOverTextOffset / rootCanvas.scaleFactor;

                if (rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    itemHoverText.rectTransform.position = (Vector3)(eventData.position + itemOverTextOffset);
                }
                else
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvasRect,
                        eventData.position,
                        rootCanvas.worldCamera,
                        out Vector2 localPoint);

                    itemHoverText.rectTransform.anchoredPosition = localPoint + scaledOffset;
                }
            }
        }
    }
}
