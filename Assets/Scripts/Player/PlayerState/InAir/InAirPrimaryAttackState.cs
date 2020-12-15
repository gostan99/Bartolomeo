using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class InAirPrimaryAttackState : PlayerState
    {
        float animationLength;
        public InAirPrimaryAttackState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animationLength);
        }
        public override void Enter()
        {
            pInput.JumpInputCounter = 0;
            timer = 0F;
            newState = this;
            pInput.AttackInput = false;
      
        }

        public override void Exit()
        {
            throw new NotImplementedException();
        }
        public override void LogicUpdate()
        {
            pInput.InputUpdate();
            timer += Time.deltaTime;
            FacingDirectionUpdate();
            if (timer >= animationLength && pInput.xInput == 0 )
            {
                newState = pController.IdleState;
            }
            else if(timer>= animationLength && pInput.AttackInput)
            {
                newState = pController.InAirSecondaryAttackState;
            }
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = new Vector2(pData.Speed * pInput.xInput, pData.Rb.velocity.y);

        }

        

    }
}

