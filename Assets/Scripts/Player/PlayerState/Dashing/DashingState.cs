using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class DashingState : PlayerState
    {
        private AudioClip sound;

        public DashingState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            sound = Resources.Load<AudioClip>(@"Sounds/hero_dash");
        }

        public override void Enter()
        {
            newState = this;
            pInput.JumpInputCounter = pData.MaxJumpCounter - 1;
            pData.currentMana -= pData.manaCost;
            pController.audioSource.PlayOneShot(sound);
            timer = 0;
            pInput.DashInput = false;
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }

        public override void LogicUpdate()
        {
            timer += Time.deltaTime;
            if (timer >= pData.DashDuration)
            {
                if (IsGrounded())
                {
                    pData.DashCooldownTimer = pData.DashCooldown;
                    newState = pController.IdleState;
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