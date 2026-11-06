using UnityEngine;

namespace CarGame
{
    public class EnemyEyeTester : MonoBehaviour
    {

        [Header("Eye Tracking")]
        [SerializeField] private Transform eyeParent;
        [SerializeField] private Transform eyeCenter;
        [SerializeField] private Transform target;
        [SerializeField] private float maxDistance = 0.065f;

        private void Update()
        {
            UpdateEyePosition();
        }

        private void UpdateEyePosition()
        {

            Vector3 direction = target.position - eyeCenter.position;

            if (direction.magnitude > maxDistance)
            {
                direction = direction.normalized * maxDistance;
            }

            Vector3 targetWorldPos = eyeCenter.position + direction;

            eyeParent.localPosition = transform.InverseTransformPoint(targetWorldPos);


        }
    }
}
