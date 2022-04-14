using UnityEngine;

namespace Assets.Scripts.Player
{
    public class WallJumpState : PlayerState
    {
        private float animtionLength;

        public WallJumpState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animtionLength);
        }

        public override void Enter()
        {
            newState = this;
            timer = 0f;
            pInput.JumpInputCounter = 0;
            pData.Rb.AddForce(pData.WallJumpDirection * pData.WallJumpVelocity, ForceMode2D.Impulse);
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }

        public override void LogicUpdate()
        {
            timer += Time.deltaTime;
            pInput.InputUpdate();
            WallJumpDirectionUpdate();
            if (pData.DashCooldownTimer > 0)
            {
                pData.DashCooldownTimer -= Time.deltaTime;
            }

            if (pInput.DashInput)
            {
                FacingDirectionUpdate();
                if (pData.DashCooldownTimer <= 0 && pData.canDash && pData.HasDash)
                {
                    newState = pController.DashingState;
                    pData.canDash = false;
                }
            }
            else if (pData.Rb.velocity.y < 0)
            {
                newState = pController.StartFallingState;
                pInput.JumpInputCounter = 1;
            }
        }

        public override void PhysicUpdate()
        {
            //if (timer >= jumpTime && !hasAddForce)
            //{
            //    hasAddForce = true;
            //}
            //else if (timer <= jumpTime)
            //{
            //    pData.Rb.velocity = Vector2.zero;
            //}
        }
    }
}