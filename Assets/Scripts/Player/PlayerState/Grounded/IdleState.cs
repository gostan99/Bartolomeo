using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Player
{
    public class IdleState : PlayerState
    {
        public IdleState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
        }

        public override void LogicUpdate()
        {
            if (pData.DashCooldownTimer > 0)
            {
                pData.DashCooldownTimer -= Time.deltaTime;
            }

            pInput.InputUpdate();
            if (pInput.xInput != 0)
            {
                newState = pController.StartRunState;
            }
            else if (pInput.JumpInput)
            {
                newState = pController.JumpState;
            }
            else if (pData.Rb.velocity.y < -30f && !IsGrounded())
            {
                ++pInput.JumpInputCounter;
                newState = pController.StartFallingState;
            }
            else if (pInput.DashInput && pData.DashCooldownTimer <= 0)
            {
                newState = pController.DashingState;

            }
            else if(pInput.AttackInput && pInput.yInput == 0)
            {
                newState = pController.PrimaryAttackState;
            }
            else if (pInput.yInput<0 && pInput.AttackInput)
            {
                newState = pController.GroundedUpwardAttackState;
            }
            else if (pInput.yInput>0 && pInput.AttackInput)
            {
                newState = pController.GroundedDownwardAttackState;
            }
            else if (pData.OurHealth<0)
            {
                SceneManager.LoadScene(0);
            }
            //Debug.Log(pInput.JumpInput && pInput.AttackInput);

        }

        public override void PhysicUpdate()
        {
        }

        public override void Enter()
        {
            pInput.JumpInputCounter = 0;
            newState = this;
            pData.canDash = true;
            pInput.DownwardAttackInput = false;
            
        }

        public override void Exit()
        {
            throw new NotImplementedException();
        }
    }
}
