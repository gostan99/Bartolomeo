using UnityEngine;

namespace Assets.Scripts.Player
{
    public class RunningState : PlayerState
    {
        public RunningState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pInput.JumpInputCounter = 0;
        }

        public override void Enter()
        {
            pInput.JumpInputCounter = 0;
            newState = this;
            pData.canDash = true;
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }

        public override void LogicUpdate()
        {
            pInput.InputUpdate();
            FacingDirectionUpdate();
            if (pData.DashCooldownTimer > 0)
            {
                pData.DashCooldownTimer -= Time.deltaTime;
            }

            if (pInput.xInput == 0)
            {
                newState = pController.IdleState;
            }
            if (pInput.JumpInput)
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
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = new Vector2(pData.Speed * pInput.xInput, pData.Rb.velocity.y);

        }
    }
}