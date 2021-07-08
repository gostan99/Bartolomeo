using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class DashingState : PlayerState
    {
        private float startPosX;
        private AudioClip sound;

        public DashingState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            sound = Resources.Load<AudioClip>(@"Sounds/hero_dash");
        }

        public override void Enter()
        {
            startPosX = pController.transform.position.x;
            newState = this;
            pInput.JumpInputCounter = pData.MaxJumpCounter - 1;
            pData.currentMana -= pData.manaCost;
            pController.audioSource.PlayOneShot(sound);
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }

        public override void LogicUpdate()
        {
            float distanceBeenDashed = Mathf.Abs(pController.transform.position.x - startPosX);
            pInput.InputUpdate();
            if (distanceBeenDashed >= pData.DashDistance || pData.Rb.velocity.x == 0)
            {
                if (IsGrounded())
                {
                    if (pInput.xInput == 0)
                    {
                        pData.DashCooldownTimer = pData.DashCooldown;
                        newState = pController.StopDashingState;
                    }
                    else
                    {
                        pData.DashCooldownTimer = pData.DashCooldown;
                        newState = pController.RunningState;
                    }
                }
                else
                {
                    pData.DashCooldownTimer = pData.DashCooldown;
                    newState = pController.StartFallingState;
                }
            }
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = pData.DashVelocityX * pData.FacingDirection * Vector2.right;
        }
    }
}