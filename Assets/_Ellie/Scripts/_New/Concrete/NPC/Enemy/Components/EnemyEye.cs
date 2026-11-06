using Demo.Perception;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Horsepower.NPC
{
    public class EnemyEye : EnemyComponent
    {
        [Header("Eye Tracking")]
        [SerializeField] private Transform eyeParent;
        [SerializeField] private Transform eyeCenter;
        [SerializeField] private float maxDistance = 0.1f;
        [SerializeField] private float speed = 1.25f;

        [Header("Lights")]
        [SerializeField] private Light2D eyeLight;
        [SerializeField] private float minIntensity = 0.5f;
        [SerializeField] private float maxIntensity = 3f;

        [Header("Eye Lid")]
        [SerializeField] private Animator eyeLid;
        [SerializeField] private float minBlink = 1;
        [SerializeField] private float maxBlink = 3;

        private Vector3 startPosition;

        public override void Init(EnemyContext context)
        {
            startPosition = eyeParent.localPosition;

            if (eyeLid != null)
            {
                StartCoroutine(Blink());
            }
        }

        public override void Tick(EnemyContext context, float dt)
        {
            UpdateEyePosition(context);

            float t = context.AlertMeter / 100f;
            eyeLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        }

        private void UpdateEyePosition(EnemyContext context)
        {
            if (context.IsIdle && context.AlertMeter == 0)
            {

                Move(startPosition);
            }
            else
            {
                Vector3 direction = context.LastKnownPosition - eyeCenter.position;

                if (direction.magnitude > maxDistance)
                {
                    direction = direction.normalized * maxDistance;
                }

                Vector3 targetWorldPos = eyeCenter.position + direction;
                Move(transform.InverseTransformPoint(targetWorldPos));
            }

        }

        private void Move(Vector3 target)
        {
            eyeParent.localPosition = Vector3.MoveTowards(eyeParent.localPosition, target, Time.deltaTime * speed);

        }

        private IEnumerator Blink()
        {
            while (true)
            {
                float random = Random.Range(minBlink, maxBlink);

                yield return new WaitForSeconds(random);

                eyeLid.Play("Blink");
            }
        }
    }
}
