using Ellie.Core.Perception;
using System;
using UnityEngine;

namespace Ellie.Core
{
    [Serializable]
    public class VisionCone : Vision
    {
        [SerializeField] private Transform eye;
        [SerializeField] private float minDetectionRadius = 1f;
        [SerializeField] private float frontVisionRange = 6f;
        [SerializeField] private float frontViewAngle = 90f;
        [SerializeField] private float backVisionRange = 3f;
        [SerializeField] private float backViewAngle = 3f;

        private float cornerBuffer = 1;
        private float suspicionPerSecond = 1;

        public override void Tick(IPerceptionContext context, float dt)
        {
            base.Tick(context, dt);

            context.IsTargetInView = false;

            foreach (var candidate in TargetRegistry.Instance.GetTargetsForFaction(context.Faction))
            {
                if (candidate == null || candidate.Transform == context.Transform)
                {
                    continue;
                }

                if (!candidate.IsVisible)
                {
                    continue;
                }

                if (CanSeeTarget(context, candidate))
                {
                    context.SuspicionStimuli.Add(new Stimulus(candidate.Transform, candidate.Transform.position, suspicionPerSecond * dt));

                    if (candidate.Transform == context.Target)
                    {
                        context.IsTargetInView = true;
                    }
                }
            }


        }

        public override bool CanSeeTarget(IPerceptionContext context, IVisionTarget target)
        {
            Vector2 origin = eye.position;
            Vector2 direction = (Vector2)target.Transform.position - origin;

            float maxPossibleRange = Mathf.Max(frontVisionRange, backVisionRange) + cornerBuffer;
            if (direction.sqrMagnitude > maxPossibleRange * maxPossibleRange)
            {
                return false;
            }

            Vector2 facing = eye.right * context.Transform.localScale.x;

            foreach (Vector2 point in GetTargetCheckPoints(target))
            {
                Vector2 toPoint = point - origin;
                Debug.DrawRay(origin, toPoint);
            }
            foreach (Vector2 point in GetTargetCheckPoints(target))
            {
                Vector2 toPoint = point - origin;
                float dist = toPoint.magnitude;

                bool withinMinRadius = dist <= minDetectionRadius;

                if (!withinMinRadius)
                {
                    bool isInFront = Vector2.Dot(facing, toPoint) >= 0f;

                    float allowedRange = isInFront ? frontVisionRange : backVisionRange;
                    float coneAngle = isInFront ? frontViewAngle : backViewAngle;

                    if (coneAngle <= 0f)
                    {
                        continue;
                    }

                    if (dist > allowedRange)
                    {
                        continue;
                    }

                    Vector2 referenceDir = isInFront ? facing : -facing;
                    float allowedHalfAngle = coneAngle / 2f;
                    float angleToPoint = Vector2.Angle(referenceDir, toPoint);

                    if (angleToPoint > allowedHalfAngle)
                    {
                        continue;
                    }
                }

                var hit = Physics2D.Raycast(origin, toPoint.normalized, dist, obstructionMask);

                if (hit.collider == null)
                {
                    return true;
                }

            }

            return false;
        }


        private Vector2[] GetTargetCheckPoints(IVisionTarget target)
        {
            Transform t = target.Transform;
            BoxCollider2D box = target.VisionCollider;

            Vector2 localExtents = box.size * 0.5f;
            Vector2 localOffset = box.offset;

            return new Vector2[]
            {
                t.position,
                t.TransformPoint(localOffset + new Vector2(-localExtents.x, -localExtents.y)),
                t.TransformPoint(localOffset + new Vector2(-localExtents.x,  localExtents.y)),
                t.TransformPoint(localOffset + new Vector2( localExtents.x, -localExtents.y)),
                t.TransformPoint(localOffset + new Vector2( localExtents.x,  localExtents.y)),
            };
        }

        #region Gizmos

        public override void DrawGizmos(IPerceptionContext context)
        {
            if (eye == null)
            {
                return;
            }

            Vector2 origin = eye.position;
            float facing = context != null ? context.Transform.localScale.x : 1f;
            Vector2 direction = eye.right * facing;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            DrawCone(origin, angle, frontViewAngle, frontVisionRange, Color.green);
            DrawCone(origin, angle + 180f, backViewAngle, backVisionRange, Color.cyan);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(origin, minDetectionRadius);
        }

        private void DrawCone(Vector3 origin, float centerAngle, float coneAngle, float range, Color color)
        {
            Gizmos.color = color;

            float halfAngle = coneAngle / 2f;
            int segments = 20;

            Vector3 prevPoint = origin + DirFromAngle(centerAngle - halfAngle) * range;
            Gizmos.DrawLine(origin, prevPoint);

            for (int i = 1; i <= segments; i++)
            {
                float angle = centerAngle - halfAngle + (coneAngle * i / segments);
                Vector3 point = origin + DirFromAngle(angle) * range;
                Gizmos.DrawLine(prevPoint, point);
                prevPoint = point;
            }

            Gizmos.DrawLine(origin, prevPoint);
        }

        private Vector3 DirFromAngle(float angleDegrees)
        {
            float rad = angleDegrees * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
        }

        #endregion
    }
}
