using CarGame;
using Ellie.Core;
using System;
using UnityEngine;

namespace Horsepower.NPC
{
    [Serializable]
    public class CarMover : MovementController
    {
        [SerializeField] private CarController carController;

        private float speedMulti = 7.5f;

        public override void Tick(IMovementContext context, float dt)
        {
            carController.SetMaxSpeed(context.MoveSpeed / speedMulti);
            carController.SetMoveInput(context.MoveTarget.x);

            if (context.StopMove)
            {
                carController.Break();

                context.StopMove = false;
            }
        }
    }
}
