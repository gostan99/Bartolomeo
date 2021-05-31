using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class InAirUpwardAttackState : PlayerState
    {
        private float animationLength;

        public InAirUpwardAttackState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animationLength);
        }

        public override void Enter()
        {
            timer = 0F;
            newState = this;
            pInput.AttackInput = false;
            FacingDirectionUpdate();
        }

        public override void Exit()
        {
            throw new NotImplementedException();
        }

        public override void LogicUpdate()
        {
            pInput.InputUpdate();
            timer += Time.deltaTime;

            if (pData.CollidedObjects.Count != 0)
            {
                foreach (var collider in pData.CollidedObjects)
                {
                    object[] package = new object[3];
                    package[0] = pData.AttackDamage;
                    package[1] = pData.FacingDirection;
                    package[2] = pData;
                    collider.SendMessage("TakeDamage", package);
                }
                pData.CollidedObjects.Clear();
            }

            if (timer >= animationLength)
            {
                if (!IsGrounded())
                {
                    newState = pController.StartFallingState;
                }
                else
                {
                    newState = pController.IdleState;
                }
            }
            else if (pInput.DashInput)
            {
                if (pData.DashCooldownTimer <= 0 && pData.canDash && pData.HasDash)
                {
                    newState = pController.DashingState;
                    pData.canDash = false;
                }
            }
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = new Vector2(pData.Speed * pInput.xInput, pData.Rb.velocity.y);
        }
    }
}