using UnityEngine;

namespace Assets.Scripts.Player
{
    public class WallSlideState : PlayerState
    {
        private AudioClip sound;

        public WallSlideState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            sound = Resources.Load<AudioClip>(@"Sounds/hero_wall_slide");
        }

        public override void Enter()
        {
            newState = this;
            pController.audioSource.Stop();
            pController.audioSource.clip = sound;
            pController.audioSource.loop = true;
            pController.audioSource.Play();
            pInput.JumpInputCounter = 0;
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }

        public override void LogicUpdate()
        {
            pInput.InputUpdate();
            WallDetectorDirectionUpdate();
            if (pInput.JumpInput)
            {
                WallJumpDirectionUpdate();
                newState = pController.WallJumpState;
            }
            else if (pInput.DashInput)
            {
                if (pData.DashCooldownTimer <= 0 && pData.canDash && pData.HasDash)
                {
                    newState = pController.DashingState;
                    pData.canDash = false;
                }
            }
            else if (!IsWallContacted() || IsGrounded() || pInput.xInput == 0)
            {
                newState = pController.StartFallingState;
            }
            if (newState != this)
            {
                pController.audioSource.Stop();
            }
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = new Vector2(pData.Rb.velocity.x, -pData.WallSlideVelocityY);
        }
    }
}