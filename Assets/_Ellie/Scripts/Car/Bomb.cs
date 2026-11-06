using UnityEngine;

namespace CarGame
{
    public class Bomb : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private float blastRadius = 2f;
        [SerializeField] private BombExplosion explosion;
        [SerializeField] private NoiseGenerator noiseGenerator;

        private BombItem data;
        private IAttacker attacker;
        public void Setup(BombItem data, IAttacker attacker, Vector3 direction, float force, Vector2 linearVelocity)
        {
            this.data = data;
            this.attacker = attacker;
            body.AddForce(direction * force, ForceMode2D.Impulse);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            explosion.Explode(attacker, data.damage, blastRadius);
            noiseGenerator.GenerateNoise(1f, null);

            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, blastRadius);
        }
    }

}
