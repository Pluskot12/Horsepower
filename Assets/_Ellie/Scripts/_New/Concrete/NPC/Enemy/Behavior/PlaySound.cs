using Demo.Perception;
using Ellie.Audio;
using Ellie.Core.Behavior;
using System;
using UnityEngine;

namespace Horsepower.NPC
{
    [Serializable]
    public class PlaySound : Behavior<EnemyContext>
    {
        [SerializeField] private AudioClip[] onEnterSound;
        [SerializeField] private AudioClip[] onExitSound;
        [SerializeField] private bool playOnce;

        private bool played;

        protected override void OnEnter(EnemyContext context)
        {
            if (playOnce && played)
            {
                return;
            }

            if (onEnterSound != null)
            {
                Debug.LogWarning("PLAYING");
                SoundManager.PlayRandomSFX(onEnterSound, context.Transform.position);
                played = true;
            }
        }

        protected override void OnExit(EnemyContext context)
        {
            if (playOnce && played)
            {
                return;
            }

            if (onExitSound != null)
            {
                SoundManager.PlayRandomSFX(onExitSound, context.Transform.position);
                played = true;
            }
        }
    }
}
