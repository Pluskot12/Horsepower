using PrimeTween;
using UnityEngine;

namespace CarGame
{
    public class HitEffect : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform parent;

        [SerializeField] private Rigidbody2D body;
        [SerializeField] private float force = 10;
        [SerializeField] private float randomForce = 0.1f;
        [SerializeField] private float decayTime = 0.3f;
        [SerializeField] private float decayDelay = 1.5f;
        [SerializeField] private float minAngle = 25;
        [SerializeField] private float maxAngle = 155f;

        [SerializeField] private float angleOffset = 0f;
        [SerializeField] private float torque = 20;
        [SerializeField] private bool activateOnStart = true;


        [Header("Debug")]
        [SerializeField] private bool showDebugAngles = true;
        [SerializeField] private float debugLength = 1.5f;
        [SerializeField] private Color debugColor = Color.red;

        private void Start()
        {
            if (activateOnStart)
                Setup();
        }

        public void Setup()
        {
            Vector3 dir = GetDirection(Random.Range(minAngle, maxAngle));

            body.AddForce(dir * (force + Random.Range(0, randomForce)), ForceMode2D.Impulse);
            body.AddTorque(Random.Range(-torque, torque));
            Tween.Alpha(spriteRenderer, 0f, decayTime, startDelay: decayDelay);

            Destroy(gameObject, decayTime + decayDelay);
        }

        private Vector3 GetDirection(float angle)
        {
            Transform reference = parent != null ? parent : transform;
            float total = angle + angleOffset;

            if (parent != null)
            {
                Vector3 scale = parent.lossyScale;

                if (scale.y < 0)
                {
                    total = -total;
                }
                if (scale.x < 0)
                {
                    total = 180f - total;
                }
            }

            return Quaternion.Euler(0, 0, total) * reference.right;
        }
        private void OnDrawGizmos()
        {
            if (!showDebugAngles)
            {
                return;
            }

            Vector3 origin = transform.position;
            Gizmos.color = debugColor;

            Vector3 minDir = GetDirection(minAngle);
            Vector3 maxDir = GetDirection(maxAngle);
            Gizmos.DrawLine(origin, origin + minDir * debugLength);
            Gizmos.DrawLine(origin, origin + maxDir * debugLength);

            const int segments = 20;
            Vector3 previous = origin + minDir * debugLength;
            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector3 next = origin + GetDirection(Mathf.Lerp(minAngle, maxAngle, t)) * debugLength;
                Gizmos.DrawLine(previous, next);
                previous = next;
            }

            Gizmos.color = new Color(debugColor.r, debugColor.g, debugColor.b, 0.4f);
            Gizmos.DrawLine(origin, origin + GetDirection((minAngle + maxAngle) * 0.5f) * debugLength);
        }

    }
}
