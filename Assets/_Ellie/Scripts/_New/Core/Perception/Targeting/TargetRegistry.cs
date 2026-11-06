using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ellie.Core
{
    public class TargetRegistry : MonoBehaviour
    {
        public static TargetRegistry Instance { get; private set; }

        public event Action<IVisionTarget> OnActorAdded;
        public event Action<IVisionTarget> OnActorRemoved;

        private List<IVisionTarget> targets = new List<IVisionTarget>();
        public IReadOnlyList<IVisionTarget> All => targets;

        private Dictionary<Faction, List<IVisionTarget>> excludingFaction = new Dictionary<Faction, List<IVisionTarget>>();

        private void Awake()
        {
            Instance = this;

            foreach (Faction faction in Enum.GetValues(typeof(Faction)))
            {
                excludingFaction[faction] = new List<IVisionTarget>();
            }
        }

        public void Register(IVisionTarget actor)
        {
            targets.Add(actor);

            foreach (var kvp in excludingFaction)
            {
                if (kvp.Key != actor.Faction)
                {
                    kvp.Value.Add(actor);
                }
            }

            OnActorAdded?.Invoke(actor);
        }

        public void Unregister(IVisionTarget actor)
        {
            targets.Remove(actor);

            foreach (var kvp in excludingFaction)
            {
                kvp.Value.Remove(actor);
            }

            OnActorRemoved?.Invoke(actor);
        }

        public IReadOnlyList<IVisionTarget> GetTargetsForFaction(Faction faction)
        {
            return excludingFaction.TryGetValue(faction, out var list) ? list : targets;
        }

    }
}
