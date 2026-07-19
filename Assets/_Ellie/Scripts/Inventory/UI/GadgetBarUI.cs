using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CarGame
{
    public class GadgetBarUI : InventoryUI
    {
        public InventoryItem BombSlot => inventorySlots[0].SlottedItem;
        public InventoryItem BackSlot => inventorySlots[1].SlottedItem;
        public InventoryItem MidSlot => inventorySlots[2].SlottedItem;
        public InventoryItem FrontSlot => inventorySlots[3].SlottedItem;
    }
}
