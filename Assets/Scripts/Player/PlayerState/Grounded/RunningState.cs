using UnityEngine;

namespace Assets.Scripts.Player
{
    public class RunningState : PlayerState
    {
        private AudioClip sound;

        public RunningState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pInput.JumpInputCounter = 0;
            sound = Resources.Load<AudioClip>(@"Sounds/hero_run_footsteps_stone");
        }

        public override void Enter()
        {
            pInput.JumpInputCounter = 0;
            newState = this;
            pData.canDash = true;
            pController.audioSource.clip = sound;
            pController.audioSource.loop = true;
            pController.audioSource.Play();
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
            else if (pInput.JumpInput)
            {
                newState = pController.JumpState;
            }
            else if (pData.Rb.velocity.y < -30f && !IsGrounded())
            {
                ++pInput.JumpInputCounter;
                newState = pController.StartFallingState;
            }
            else if (pInput.DashInput)
            {
                if (pData.DashCooldownTimer <= 0 && pData.HasDash && pData.canDash)
                {
                    newState = pController.DashingState;
                    pData.canDash = false;
                }
            }
            else if (pInput.AttackInput)
            {
                if (pInput.yInput == 0)
                {
                    newState = pController.PrimaryAttackState;
                }
                else if (pInput.yInput > 0)
                {
                    newState = pController.GroundedDownwardAttackState;
                }
                else if (pInput.yInput < 0)
                {
                    newState = pController.GroundedUpwardAttackState;
                }
            }
            if (newState != this)
            {
                pController.audioSource.Stop();
            }
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = new Vector2(pData.Speed * pInput.xInput, pData.Rb.velocity.y);
        }
    }
}