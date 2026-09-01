using Ellie.Audio;
using PrimeTween;
using UnityEngine;

namespace CarGame
{
    public class GadgetFireworksLauncher : GadgetAbility
    {
        [SerializeField] private Projectile fireworkPrefab;
        [SerializeField] private Transform gear;
        [SerializeField] private Transform rotationPivot;
        [SerializeField] private Transform movePivot;
        [SerializeField] private AudioClip onUseSound;

        [SerializeField] private float rotationTime;
        [SerializeField] private Vector3 launchRotation;
        [SerializeField] private Vector3 launchPosition;

        [Header("Settings")]
        [SerializeField] private float bulletSpeed = 10;
        [SerializeField] private int damage = 10;
        [SerializeField] private LayerMask hitMask;

        private bool projectileReady;

        public override bool TryActivate(Player player)
        {
            if (projectileReady)
            {
                Fire();

                return true;
            }

            return false;
        }

        private void Fire()
        {
            var instance = Instantiate(fireworkPrefab, fireworkPrefab.transform.position, fireworkPrefab.transform.rotation);
            instance.enabled = true;
            instance.Setup(fireworkPrefab.transform.position, -90, rotationPivot.up * bulletSpeed, damage, 10f, hitMask);

            //Tween.ShakeLocalRotation(instance.transform, Vector3.forward * 5, 0.5f);

            rotationPivot.localRotation = Quaternion.Euler(Vector3.zero);
            movePivot.localPosition = Vector3.zero;

            projectileReady = false;

            SoundManager.PlaySFX(onUseSound, transform.position);
        }

        private void Reload()
        {
            Tween.LocalRotation(rotationPivot, Vector3.zero, launchRotation, rotationTime/*, ease: Ease.OutBack*/);
            Tween.LocalPosition(movePivot, Vector3.zero, launchPosition, rotationTime/*, ease: Ease.OutBack*/);

            Tween.LocalRotation(gear, gear.localRotation.eulerAngles + launchRotation, rotationTime);

            Tween.Delay(rotationTime, () =>
            {
                projectileReady = true;
            });
        }

        public override void OnEquip(Player player)
        {
            gadget.OnCooldownEnded += Gadget_OnCooldownEnded;
        }

        private void Gadget_OnCooldownEnded(Gadget gadget, GadgetItem.Slot slot)
        {
            Reload();
        }

        public override void OnUnequip(Player player)
        {
            // StopAllCoroutines();
        }
    }
}
