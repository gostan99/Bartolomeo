using UnityEngine;

namespace Assets.Scripts.Player
{
    public class FallingState : PlayerState
    {


        public FallingState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
        }


        public override void Enter()
        {
            newState = this;
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

            if (IsGrounded() && pInput.xInput == 0)
            {
                newState = pController.IdleState;
            }
            else if (IsGrounded() && pInput.xInput != 0)
            {
                newState = pController.RunningState;
            }
            else if (pInput.JumpInput)
            {
                newState = pController.JumpState;
            }
            else if (IsWallContacted() && pInput.xInput != 0)
            {
                newState = pController.WallContactState;
            }
            if (pData.Rb.velocity.y < -750.0f && IsNearGround())
            {
                newState = pController.LandingState;
            }
            else if (pInput.DashInput && pData.DashCooldownTimer <= 0 && pData.canDash)
            {
                newState = pController.DashingState;
                pData.canDash = false;
            }
        }

        public override void PhysicUpdate()
        {
            //Movment
            pData.Rb.velocity = new Vector2(pData.Speed * pInput.xInput, pData.Rb.velocity.y);
        }

        bool IsNearGround()
        {
            var nearGroundDetectorSize = new Vector2(pData.GroundDetectorSize.x, pData.GroundDetectorSize.y + 100);
            var isNearGround = Physics2D.OverlapBox(pData.GroundDetector.transform.position, nearGroundDetectorSize, 0f, pData.GroundMask, 0f, 0f);
            return isNearGround;
        }
    }
}