using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ellie.Core
{
    public static class AggroTracker
    {
        private static readonly Dictionary<Transform, Transform> aggro = new();
        public static IReadOnlyDictionary<Transform, Transform> Aggro => aggro;

        public static event Action<Transform, Transform> OnAggroChanged;

        public static void SetAggroTarget(Transform enemy, Transform target)
        {
            bool previousTarget = aggro.TryGetValue(enemy, out var previous);

            if (previousTarget && previous == target)
            {
                return;
            }

            if (target == null)
            {
                aggro.Remove(enemy);
            }
            else
            {
                aggro[enemy] = target;
            }

            OnAggroChanged?.Invoke(enemy, target);
        }

        public static Transform GetAggroTarget(Transform enemy)
        {
            return aggro.TryGetValue(enemy, out var t) ? t : null;
        }
    }
}
