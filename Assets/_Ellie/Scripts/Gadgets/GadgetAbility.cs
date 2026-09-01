using UnityEngine;

namespace CarGame
{
    public abstract class GadgetAbility : MonoBehaviour
    {
        [SerializeField] private float cooldown;

        public float Cooldown => cooldown;

        protected Player player;
        protected Gadget gadget;

        public void Init(Gadget gadget, Player player)
        {
            this.gadget = gadget;
            this.player = player;
        }

        public virtual void OnEquip(Player player) { }

        public virtual bool TryActivate(Player player) { return true; }

        public virtual void OnUnequip(Player player) { }
    }
}
