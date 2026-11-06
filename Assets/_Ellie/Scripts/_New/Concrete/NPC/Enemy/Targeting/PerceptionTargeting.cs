using Ellie.Core;
using Ellie.Core.Perception;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Demo.Perception
{
    [Serializable]
    public class PerceptionTargeting : EnemyTargeting
    {
        [SerializeField] private float increaseRate = 1f;
        [SerializeField] private float decreateRate = 1f;



        private float maxSuspicion = 100;
        private float staggerTime = 0.5f;

        // Investigation
        private float investigationSuspicion;
        private float investigationStaggerTimer;

        private Transform lockedTarget;



        public override void Tick(IPerceptionContext context, float dt)
        {
            // Apply Stimuli
            ApplyStimuli(context, dt);

            // Apply Investigation
            ApplyInvestigation(context, dt);

            // Decay 
            DecayUnstimulated(context, dt);

            // Update Target
            UpdateTarget(context);

            UpdateAlertMeter(context);

            UpdateLastKnownPosition(context);

            context.SuspicionStimuli.Clear();
            context.InvestigationPoints.Clear();
        }

        private void UpdateLastKnownPosition(IPerceptionContext context)
        {
            if (lockedTarget != null)
            {
                context.LastKnownPosition = TargetData[lockedTarget].lastKnownPosition;
            }
            else
            {
                if (context.InvestigationPoints.Count > 0)
                {
                    var best = context.InvestigationPoints[0];

                    foreach (var p in context.InvestigationPoints)
                    {
                        if (p.Priority > best.Priority)
                        {
                            best = p;
                        }
                    }

                    context.LastKnownPosition = best.Position;
                }

                if (TargetData.Count > 0)
                {
                    TargetInfo highest = default;
                    foreach (var stim in TargetData)
                    {
                        if (stim.Value.suspicion > highest.suspicion)
                        {
                            highest = stim.Value;
                        }
                    }

                    context.LastKnownPosition = highest.lastKnownPosition;
                }
            }

        }

        private void UpdateAlertMeter(IPerceptionContext context)
        {
            float highestTargetSuspicion = 0f;

            foreach (var info in TargetData.Values)
            {
                if (info.suspicion > highestTargetSuspicion)
                {
                    highestTargetSuspicion = info.suspicion;
                }
            }

            context.AlertMeter = Mathf.Clamp(highestTargetSuspicion, 0f, 100f);
            //context.AlertMeter2 = Mathf.Clamp(highestTargetSuspicion, 0f, 100f);
            context.AlertMeter2 = Mathf.Clamp(investigationSuspicion, 0f, 100f);
        }

        private void ApplyStimuli(IPerceptionContext context, float dt)
        {
            foreach (var stim in context.SuspicionStimuli)
            {
                TargetInfo info = TargetData.TryGetValue(stim.Source, out TargetInfo target) ? target : default;
                info.suspicion = Mathf.Clamp(info.suspicion + (stim.Amount * increaseRate), 0, maxSuspicion);
                info.lastStimulusTime = staggerTime;
                info.lastKnownPosition = stim.Position;
                TargetData[stim.Source] = info;

                UpdateAlertedState(stim.Source, info.suspicion);
            }
        }

        private void UpdateAlertedState(Transform candidate, float suspicion)
        {
            bool isAlerted = AlertedTargets.Contains(candidate);

            if (suspicion >= maxSuspicion && !isAlerted)
            {
                AlertedTargets.Add(candidate);
            }
            else if (suspicion <= 0 && isAlerted)
            {
                AlertedTargets.Remove(candidate);
            }
        }

        private void ApplyInvestigation(IPerceptionContext context, float dt)
        {
            if (context.InvestigationPoints.Count == 0)
            {
                return;
            }

            float highest = 0f;

            foreach (var point in context.InvestigationPoints)
            {
                if (point.Priority > highest)
                {
                    highest = point.Priority;
                }
            }

            investigationSuspicion = Mathf.Clamp(investigationSuspicion + highest, 0, maxSuspicion);
            investigationStaggerTimer = staggerTime;

        }

        private void DecayUnstimulated(IPerceptionContext context, float dt)
        {
            if (context.IsAggro) { return; }

            foreach (var candidate in new List<Transform>(TargetData.Keys))
            {
                TargetInfo info = TargetData[candidate];

                if (info.lastStimulusTime > 0)
                {
                    info.lastStimulusTime -= dt;
                    TargetData[candidate] = info;
                }
                else
                {
                    info.suspicion = Mathf.Clamp(info.suspicion - decreateRate * dt, 0, maxSuspicion);
                }

                TargetData[candidate] = info;


                if (info.suspicion <= 0)
                {
                    TargetData.Remove(candidate);
                }

                UpdateAlertedState(candidate, info.suspicion);
            }

            if (investigationStaggerTimer > 0)
            {
                investigationStaggerTimer -= dt;
            }
            else
            {
                investigationSuspicion = Mathf.Clamp(investigationSuspicion - decreateRate * dt, 0, maxSuspicion);
            }
        }

        private void UpdateTarget(IPerceptionContext context)
        {
            if (lockedTarget != null)
            {
                if (!AlertedTargets.Contains(lockedTarget))
                {
                    lockedTarget = null;
                }
            }
            else if (AlertedTargets.Count > 0)
            {
                lockedTarget = AlertedTargets[UnityEngine.Random.Range(0, AlertedTargets.Count)];
            }

            if (context.Target != lockedTarget)
            {
                AggroTracker.SetAggroTarget(context.Transform, lockedTarget);
            }

            if (context.Target != lockedTarget)
            {
                Debug.Log("Changing target: " + lockedTarget);
            }

            context.Target = lockedTarget;
        }

        public override void Dispose(IPerceptionContext context)
        {
            AggroTracker.SetAggroTarget(context.Transform, null);
        }

    }
}
