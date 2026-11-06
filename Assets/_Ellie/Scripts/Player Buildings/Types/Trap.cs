using UnityEngine;

namespace CarGame
{
    public class Trap : MonoBehaviour
    {
        [SerializeField] private int reflectDamage;

        public void OnDamageTaken(int damage, GameObject attacker)
        {
            if (attacker.TryGetComponent<IDamageable>(out IDamageable target))
            {
                Debug.Log("TODO: Set attacker source");
                target.TryDamage(reflectDamage, null);
            }
        }

    }
}
