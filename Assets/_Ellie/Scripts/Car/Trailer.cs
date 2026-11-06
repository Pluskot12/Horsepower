using Ellie.Audio;
using System.Collections.Generic;
using UnityEngine;

namespace CarGame
{
    public class Trailer : MonoBehaviour
    {
        [System.Serializable]
        public struct TrailerVariant
        {
            public Trailer Prefab;
            public DropTable DropTable;
        }

        [Header("References")]
        [SerializeField] private AudioClip[] deathSounds;
        [SerializeField] private Transform trailerAnchor;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private GameObject explosions;
        [SerializeField] private DropTable dropTable;

        [Header("Settings")]
        [SerializeField] private bool useLimits = true;
        [SerializeField, Range(0f, 180f)] private float maxAngle = 55f;
        [SerializeField] private float linearDrag = 0.05f;
        [SerializeField] private float angularDrag = 2f;

        private CarController car;
        private HingeJoint2D hinge;
        private Transform truckHitchPoint;


        public void Setup(CarController car, Transform hitchPoint)
        {
            this.car = car;

            Connect(car.GetRigidbody(CarController.PhysicsPart.Body), hitchPoint);

            car.ConnectTrailer(transform, trailerAnchor);
        }


        private void Connect(Rigidbody2D truckBody, Transform truckHitchPoint)
        {
            this.truckHitchPoint = truckHitchPoint;

            rb.linearDamping = linearDrag;
            rb.angularDamping = angularDrag;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.rotation = truckBody.rotation;
            transform.rotation = truckBody.transform.rotation;
            Vector3 delta = truckHitchPoint.position - trailerAnchor.position;
            transform.position += delta;
            rb.position = transform.position;

            hinge = gameObject.AddComponent<HingeJoint2D>();
            hinge.connectedBody = truckBody;
            hinge.autoConfigureConnectedAnchor = false;
            hinge.anchor = transform.InverseTransformPoint(trailerAnchor.position);
            hinge.connectedAnchor = truckBody.transform.InverseTransformPoint(truckHitchPoint.position);
            hinge.enableCollision = false;
            hinge.useMotor = false;

            if (useLimits)
            {
                hinge.useLimits = true;
                hinge.limits = new JointAngleLimits2D { min = -maxAngle, max = maxAngle };
            }
        }


        public void OnDeath()
        {
            ItemSpawner.Instance.SpawnLoot(transform, new List<DropTable>() { dropTable });

            SoundManager.PlayRandomSFX(deathSounds, transform.position);

            explosions.transform.SetParent(null);
            explosions.SetActive(true);

            gameObject.SetActive(false);
            Destroy(gameObject, 5f);
        }


        void OnDrawGizmosSelected()
        {
            if (truckHitchPoint)
            {
                Gizmos.color = Color.green; Gizmos.DrawWireSphere(truckHitchPoint.position, 0.1f);
            }
            if (trailerAnchor)
            {
                Gizmos.color = Color.red; Gizmos.DrawWireSphere(trailerAnchor.position, 0.15f);
            }
        }
    }
}
