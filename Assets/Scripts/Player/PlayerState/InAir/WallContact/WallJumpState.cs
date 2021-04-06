using UnityEngine;

namespace Assets.Scripts.Player
{
    public class WallJumpState : PlayerState
    {
        float animtionLength;
        bool hasAddForce = false;

        public WallJumpState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animtionLength);
        }

        public override void Enter()
        {
            newState = this;
            timer = 0f;
            hasAddForce = false;
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

            if (hasAddForce && pData.Rb.velocity.y < 0f)
            {
                pInput.JumpInputCounter = 1;
                newState = pController.StartFallingState;
            }
            else if (pInput.DashInput)
            {
                FacingDirectionUpdate();
                if (pData.DashCooldownTimer <= 0 && pData.canDash && PlayerData.HasDash)
                {
                    newState = pController.DashingState;
                    pData.canDash = false;
                }
            }
        }

        public override void PhysicUpdate()
        {
            if (timer >= animtionLength && !hasAddForce)
            {
                pData.Rb.velocity = pData.WallJumpDirection * pData.WallJumpVelocity;
                hasAddForce = true;
            }
            else if (timer <= animtionLength)
            {
                pData.Rb.velocity = Vector2.zero;
            }
        }
    }
}