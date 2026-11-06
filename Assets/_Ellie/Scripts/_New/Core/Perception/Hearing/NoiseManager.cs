using System;
using UnityEngine;

namespace Ellie.Core
{
    public static class NoiseManager
    {
        public static event Action<Vector3, float, Transform> OnNoiseEmitted;

        public static void EmitNoise(Vector3 position, float noiseValue, Transform owner)
        {
            OnNoiseEmitted?.Invoke(position, noiseValue, owner);
        }
    }
}
