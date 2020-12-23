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
        float animationLength;
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
            //Debug.Log(pInput.AttackInput);
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = new Vector2(pData.Speed * pInput.xInput, pData.Rb.velocity.y);
        }
    }
}
