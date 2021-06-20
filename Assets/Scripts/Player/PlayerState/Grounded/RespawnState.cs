using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class RespawnState : PlayerState
    {
        float animationLength;
        public RespawnState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animationLength);
        }
        public override void Enter()
        {
            pInput.JumpInputCounter = 0;
            timer = 0f;
            newState = this;
            pInput.UpwardAttackInput = false;

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

            if (timer >= animationLength && pInput.xInput == 0)
            {
                newState = pController.IdleState;              
            }
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = new Vector2(0, pData.Rb.velocity.y);
        }



    }
}
