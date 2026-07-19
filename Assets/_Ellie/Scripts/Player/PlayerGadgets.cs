using System.Collections.Generic;
using UnityEngine;

namespace CarGame
{
    public class PlayerGadgets : MonoBehaviour
    {
        public static PlayerGadgets Instance { get; private set; }

        public const int BOMB_INDEX = 0;

        [SerializeField] private GadgetController gadgetController;

        [Header("Settings")]
        [SerializeField] private float gadgetEquipCooldown = 5f;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip gadgetCooldownSound;

        public GadgetController GadgetController => gadgetController;

        private Gadget backGadget;
        private Gadget midGadget;
        private Gadget frontGadget;

        private Dictionary<GadgetItem.Slot, InventoryItem> slots;

        private Transform GadgetSlot => GameManager.Instance.Player.Attachments.GadgetSlot;

        private void Awake()
        {
            Instance = this;

            slots = new Dictionary<GadgetItem.Slot, InventoryItem>
            {
                { GadgetItem.Slot.Back, null },
                { GadgetItem.Slot.Middle, null },
                { GadgetItem.Slot.Front, null }
            };

            gadgetController.Inventory.AnyValueChanged += Inventory_AnyValueChanged;
        }

        private void Inventory_AnyValueChanged(InventoryItem[] obj)
        {
            if (SlotChanged(GadgetItem.Slot.Back, obj[1]))
            {
                UpdateSlot(GadgetItem.Slot.Back, obj[1]);
            }
            if (SlotChanged(GadgetItem.Slot.Middle, obj[2]))
            {
                UpdateSlot(GadgetItem.Slot.Middle, obj[2]);
            }
            if (SlotChanged(GadgetItem.Slot.Front, obj[3]))
            {
                UpdateSlot(GadgetItem.Slot.Front, obj[3]);
            }
        }

        private void UpdateSlot(GadgetItem.Slot slot, InventoryItem item)
        {
            slots[slot] = item;

            if (item == null)
            {
                RemoveGadget(slot);
            }
            else
            {
                AddGadget(slot, item);
            }
        }

        private bool SlotChanged(GadgetItem.Slot slot, InventoryItem item)
        {
            if (slots[slot] != item)
            {
                return true;
            }

            return false;
        }

        private void AddGadget(GadgetItem.Slot slot, InventoryItem item)
        {
            GadgetItem gadget = (GadgetItem)item.ItemData;

            if (gadget == null)
            {
                Debug.LogWarning("No gadget item found for " + item.ItemData);
                return;
            }

            RemoveGadget(slot);

            //gadgetController.Slots[(int)slot + 1].StartCooldown(gadgetEquipCooldown);

            Gadget instance = Instantiate(gadget.prefab, GadgetSlot);
            instance.OnCooldownStarted += StartCooldown;
            instance.Setup(GameManager.Instance.Player, gadget, gadgetEquipCooldown);



            if (slot == GadgetItem.Slot.Back)
            {
                backGadget = instance;
            }
            else if (slot == GadgetItem.Slot.Middle)
            {
                midGadget = instance;
            }
            else if (slot == GadgetItem.Slot.Front)
            {
                frontGadget = instance;
            }
        }

        private void RemoveGadget(GadgetItem.Slot slot)
        {
            Gadget gadget = GetGadgetBySlot(slot);

            if (gadget == null)
            {
                return;
            }

            gadget.OnCooldownStarted -= StartCooldown;
            gadget.Unequip();

            Destroy(gadget.gameObject);
        }



        private Gadget GetGadgetBySlot(GadgetItem.Slot slot)
        {
            switch (slot)
            {
                case GadgetItem.Slot.Back:
                    return backGadget;
                case GadgetItem.Slot.Middle:
                    return midGadget;
                case GadgetItem.Slot.Front:
                    return frontGadget;
                default:
                    return null;
            }
        }



        public BombItem TryUseBomb()
        {
            if (gadgetController.Slots[0].OnCooldown)
                return null;

            var bomb = gadgetController.Inventory.Items[BOMB_INDEX];
            if (bomb == null)
                return null;

            BombItem bombData = (BombItem)bomb.ItemData;


            gadgetController.Slots[0].StartCooldown(bombData.cooldown);

            gadgetController.OnItemUse(BOMB_INDEX);
            return bombData;
        }


        public void DropAllItems()
        {
            Vector3 position = GameManager.Instance.Player.transform.position;
            gadgetController.DropAllItems(position);
        }

        public void TryActivate(GadgetItem.Slot slot)
        {
            if (CanActivate(slot))
            {
                Gadget gadget = GetGadgetBySlot(slot);
                gadget.Activate();

                //StartCooldown(gadget, slot, gadget.AbilityCooldown);
            }
        }

        public void StartCooldown(Gadget gadget, GadgetItem.Slot slot, float cooldown)
        {
            gadgetController.Slots[(int)slot + 1].StartCooldown(cooldown);
        }

        private bool CanActivate(GadgetItem.Slot slot)
        {
            Gadget gadget = GetGadgetBySlot(slot);

            if (gadget == null)
            {
                return false;
            }

            if (!gadget.Activateable)
            {
                return false;
            }

            if (gadget.OnCooldown)
            {
                return false;
            }

            return true;
        }

        public GadgetSaveData GetSaveData()
        {
            var saveData = new GadgetSaveData();
            saveData.Inventory = gadgetController.Inventory.Items;


            return saveData;
        }

        public void LoadData(GadgetSaveData data)
        {
            gadgetController.Inventory.Set(data.Inventory);
        }
    }
}
