using UnityEngine;

namespace Assets.Scripts.Player
{
    public class FallingState : PlayerState
    {
        private AudioClip sound;

        public FallingState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            sound = Resources.Load<AudioClip>(@"Sounds/hero_falling");
        }

        public override void Enter()
        {
            newState = this;
            pController.audioSource.clip = sound;
            pController.audioSource.loop = true;
            pController.audioSource.PlayDelayed(0.3f);
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }

        public override void LogicUpdate()
        {
            pInput.InputUpdate();
            FacingDirectionUpdate();
            WallDetectorDirectionUpdate();
            if (pData.DashCooldownTimer > 0)
            {
                pData.DashCooldownTimer -= Time.deltaTime;
            }

            if (IsGrounded())
            {
                if (pInput.xInput == 0)
                {
                    newState = pController.IdleState;
                }
                else
                {
                    newState = pController.RunningState;
                }
            }
            else if (pInput.JumpInput)
            {
                newState = pController.JumpState;
            }
            else if (IsWallContacted() && pInput.xInput != 0)
            {
                newState = pController.WallContactState;
            }
            if (pData.Rb.velocity.y < -600.0f && IsNearGround())
            {
                newState = pController.LandingState;
            }
            else if (pInput.DashInput)
            {
                if (pData.DashCooldownTimer <= 0 && pData.canDash && pData.HasDash)
                {
                    newState = pController.DashingState;
                    pData.canDash = false;
                }
            }
            else if (pInput.AttackInput)
            {
                if (pInput.yInput < 0)
                {
                    newState = pController.InAirUpwardAttackState;
                }
                else
                {
                    newState = pController.InAirPrimaryAttackState;
                }
            }
            if (newState != this)
            {
                pController.audioSource.Stop();
            }
        }

        public override void PhysicUpdate()
        {
            //Movment
            pData.Rb.velocity = new Vector2(pData.Speed * pInput.xInput, pData.Rb.velocity.y);
        }

        private bool IsNearGround()
        {
            var nearGroundDetectorSize = new Vector2(pData.GroundDetectorSize.x, pData.GroundDetectorSize.y + 100);
            var isNearGround = Physics2D.OverlapBox(pData.GroundDetector.transform.position, nearGroundDetectorSize, 0f, pData.GroundMask, 0f, 0f);
            return isNearGround;
        }
    }
}