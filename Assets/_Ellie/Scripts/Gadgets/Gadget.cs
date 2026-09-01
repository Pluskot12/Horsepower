using System;
using UnityEngine;

namespace CarGame
{
    public class Gadget : MonoBehaviour
    {
        [SerializeField] private GadgetAbility ability;
        [SerializeField] private bool activateable = true;

        public event Action<Gadget, GadgetItem.Slot, float> OnCooldownStarted;
        public event Action<Gadget, GadgetItem.Slot> OnCooldownEnded;

        public bool Activateable => activateable;

        public float AbilityCooldown => ability.Cooldown;

        private GadgetItem gadgetData;

        private bool onCooldown;
        public bool OnCooldown => onCooldown;

        float timer;

        Player player;

        private float currentCooldown;

        private void Start()
        {

        }

        private void Update()
        {
            if (onCooldown)
            {
                timer += Time.deltaTime;

                if (timer >= currentCooldown)
                {
                    onCooldown = false;
                    OnCooldownEnded?.Invoke(this, gadgetData.slot);
                }
            }
        }


        public void Setup(Player player, GadgetItem data, float equipCooldown)
        {
            this.player = player;
            gadgetData = data;

            ability.Init(this, player);
            ability.OnEquip(player);

            StartCooldown(equipCooldown);
        }

        public void Activate(bool activatedByAbility = false)
        {
            if (!activatedByAbility)
            {
                if (ability.TryActivate(player) == false)
                {
                    return;
                }
            }

            StartCooldown(AbilityCooldown);
        }

        public void StartCooldown(float cd)
        {
            currentCooldown = cd;

            if (currentCooldown > 0)
            {
                OnCooldownStarted?.Invoke(this, gadgetData.slot, currentCooldown);

                onCooldown = true;
                timer = 0;
            }
        }

        public void Unequip()
        {
            ability.OnUnequip(player);
        }
    }
}
