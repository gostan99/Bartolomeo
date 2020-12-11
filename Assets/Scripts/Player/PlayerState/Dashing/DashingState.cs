using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class DashingState : PlayerState
    {
        public DashingState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
        }

        public override void Enter()
        {
            timer = 0f;
            newState = this;
            pInput.JumpInputCounter = 1;
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }

        public override void LogicUpdate()
        {
            timer += Time.deltaTime;
            pInput.InputUpdate();
            if (timer >= pData.DashDuration && pInput.xInput == 0 && IsGrounded())
            {
                pData.DashCooldownTimer = pData.DashCooldown;
                newState = pController.StopDashingState;
            }
            else if (timer >= pData.DashDuration && pInput.xInput != 0 && IsGrounded())
            {
                pData.DashCooldownTimer = pData.DashCooldown;
                newState = pController.RunningState;
            }
            else if (timer >= pData.DashDuration && !IsGrounded())
            {
                pData.DashCooldownTimer = pData.DashCooldown;
                newState = pController.StartFallingState;
            }
            else if (timer >= pData.DashDuration && pInput.JumpInput)
            {
                pData.DashCooldownTimer = pData.DashCooldown;
                newState = pController.JumpState;
            }
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = Vector2.right * pData.FacingDirection * pData.DashVelocityX;
        }

    }
}