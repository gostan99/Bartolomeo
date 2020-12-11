using UnityEngine;

namespace Assets.Scripts.Player
{
    public class WallSlideState : PlayerState
    {


        public WallSlideState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
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
            WallDetectorDirectionUpdate();

            if (!IsWallContacted() || IsGrounded() || pInput.xInput == 0)
            {
                newState = pController.UnhangedState; 
            }
            else if (pInput.JumpInput)
            {
                newState = pController.WallJumpState;
            }
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = new Vector2(pData.Rb.velocity.x, -pData.WallSlideVelocityY);
        }
    }
}