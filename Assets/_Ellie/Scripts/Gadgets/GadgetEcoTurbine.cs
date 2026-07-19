using System.Collections;
using UnityEngine;
namespace CarGame
{
    public class GadgetEcoTurbine : GadgetAbility
    {
        [SerializeField] private float speedIncrease = 10f;
        [SerializeField] private float horsePowerIncrease = 100f;
        [SerializeField] private float duration = 10f;

        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip activationSound;

        bool activated;

        public override void OnActivate(Player player) 
        {
            source.clip = activationSound;
            source.volume = player.EngineVolume;
            source.Play();

            player.SetHorsepowerBoost(horsePowerIncrease, duration);
            player.SetGadgetTopSpeed(speedIncrease, duration);

            StartCoroutine(Deactivate());

            activated = true;
        }

        IEnumerator Deactivate()
        {
            yield return new WaitForSeconds(duration);

            activated = false;
        }


        private void Update()
        {
            if (!activated)
            {
                return;
            }

            source.volume = player.EngineVolume;
        }

        public override void OnEquip(Player player) 
        {
            
        }
        
        public override void OnUnequip(Player player) 
        {
            StopAllCoroutines();

            player.SetHorsepowerBoost(0, 0);
            player.SetGadgetTopSpeed(0, 0);
        }
    }
}
