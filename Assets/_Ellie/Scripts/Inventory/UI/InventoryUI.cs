using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CarGame
{
    public class InventoryUI : MonoBehaviour
    {
        public enum InventoryType
        {
            Player,
            Chest,
            Gadget
        }
        [SerializeField] private InventoryType inventoryType = InventoryType.Chest;
        [SerializeField] protected InventorySlotUI[] inventorySlots;

        public event Action<InventorySlotUI, InventorySlotUI> AnyValueChanged = delegate { };
        public event Action<InventorySlotUI, Vector3> ItemDropped = delegate { };
        public event Action<InventorySlotUI, int> ItemQuantityChanged = delegate { };
        public event Action<InventorySlotUI> OnItemChanged = delegate { };

        protected InventoryPanelUI inventoryPanelU;
        public AudioClip GadgetRechargeSound => inventoryPanelU.GadgetRechargeSound;

        public InventoryPanelUI InventoryPanelU => inventoryPanelU;

        public InventorySlotUI[] InventorySlots => inventorySlots;

        public InventoryType Type => inventoryType;

        public void Init(InventoryPanelUI inventoryPanelUI)
        {
            inventoryPanelU = inventoryPanelUI;

            InitInventorySlots();
        }

        private void InitInventorySlots()
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                inventorySlots[i].Init(this, i);
            }
        }


        public void Refresh(InventoryItem[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                inventorySlots[i].Setup(items[i]);
            }

            if (inventoryPanelU)
                inventoryPanelU.OnRefresh();
        }

        public void OnSlotClicked(InventorySlotUI clickedSlot, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                HandleLeftClick(clickedSlot, eventData);
            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                HandleRightClick(clickedSlot, eventData);
            }
        }
        protected virtual bool IsValidSlot(InventorySlotUI clickedSlot, InventorySlotUI clonedSlot)
        {
            if (clickedSlot.Type == InventorySlotUI.SlotType.Gadget)
            {
                var item = clonedSlot.SlottedItem;
                if (item.ItemData.GetType() == typeof(BombItem) && clickedSlot.allowedItem == InventorySlotUI.AllowedItem.Bomb)
                {
                    return true;
                }
                else if (item.ItemData is GadgetItem gadget)
                {
                    if (gadget.slot == GadgetItem.Slot.Back && clickedSlot.allowedItem == InventorySlotUI.AllowedItem.GadgetBack)
                    {
                        return true;
                    }
                    else if (gadget.slot == GadgetItem.Slot.Middle && clickedSlot.allowedItem == InventorySlotUI.AllowedItem.GadgetMid)
                    {
                        return true;
                    }
                    if (gadget.slot == GadgetItem.Slot.Front && clickedSlot.allowedItem == InventorySlotUI.AllowedItem.GadgetFront)
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        private void HandleLeftClick(InventorySlotUI clickedSlot, PointerEventData eventData)
        {
            if (inventoryPanelU.ClonedSlot.SlottedItem == null)
            {
                if (clickedSlot.SlottedItem == null)
                {
                    return;
                }

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    inventoryPanelU.HandleShiftShortcut(clickedSlot);
                }
                else
                {
                    inventoryPanelU.OnItemSelected();

                    inventoryPanelU.ShowClone(clickedSlot.SlottedItem);
                    OnItemChanged.Invoke(clickedSlot);
                }
            }
            else
            {
                if (!IsValidSlot(clickedSlot, inventoryPanelU.ClonedSlot))
                {
                    return;
                }

                inventoryPanelU.OnItemPlaced();

                if (clickedSlot.SlottedItem == inventoryPanelU.ClonedSlot.SlottedItem)
                {
                    AnyValueChanged.Invoke(inventoryPanelU.ClonedSlot, clickedSlot);
                    inventoryPanelU.RemoveClone();
                }
                else
                {

                    if (clickedSlot.allowedItem == InventorySlotUI.AllowedItem.Bomb)
                    {
                        inventoryPanelU.OnBombPlaced();

                        clickedSlot.StartCooldown(5f);
                    }
                    if (clickedSlot.allowedItem == InventorySlotUI.AllowedItem.GadgetBack ||
                        clickedSlot.allowedItem == InventorySlotUI.AllowedItem.GadgetMid ||
                        clickedSlot.allowedItem == InventorySlotUI.AllowedItem.GadgetFront)
                    {
                        inventoryPanelU.OnGadgetPlaced();
                    }

                    AnyValueChanged.Invoke(inventoryPanelU.ClonedSlot, clickedSlot);

                    // Memo: This removes leftover item
                    // inventoryPanelU.RemoveClone();
                }
            }
        }
        public void HandleLeftOver(InventoryItem item)
        {
            inventoryPanelU.ShowClone(item);
        }

        private void HandleRightClick(InventorySlotUI clickedSlot, PointerEventData eventData)
        {
            if (clickedSlot.SlottedItem == null)
            {
                return;
            }

            if (clickedSlot.SlottedItem.ItemData.GetType() == typeof(WeaponItemData))
            {
                return;
            }

            if (inventoryPanelU.ClonedSlot.SlottedItem == null)
            {
                if (clickedSlot.SlottedItem == null)
                    return;

                inventoryPanelU.ShowClone(new InventoryItem(clickedSlot.SlottedItem.ItemData, 1, clickedSlot.SlottedItem.Durability));

                inventoryPanelU.OnItemSplit();
                ItemQuantityChanged.Invoke(clickedSlot, 1);
            }
            else
            {
                if (!IsValidSlot(clickedSlot, inventoryPanelU.ClonedSlot))
                {
                    return;
                }

                if (clickedSlot.SlottedItem.ItemData == inventoryPanelU.ClonedSlot.SlottedItem.ItemData)
                {
                    ItemQuantityChanged.Invoke(clickedSlot, 1);
                    inventoryPanelU.OnItemSplit();
                    inventoryPanelU.ClonedSlot.SlottedItem.Quantity++;
                    inventoryPanelU.ClonedSlot.Refresh();
                }
            }
        }

        public void DropItemAtIndex(InventorySlotUI clickedSlot, Vector2 pos)
        {
            ItemDropped.Invoke(clickedSlot, pos);
        }

        public void OnItemPickup()
        {
            inventoryPanelU.OnItemPickup();
        }

        public void OnItemBreak()
        {
            inventoryPanelU.OnItemBreak();
        }

        public void ClearCooldowns()
        {
            foreach (var slot in inventorySlots)
            {
                slot.StopCooldown();
            }
        }
    }
}