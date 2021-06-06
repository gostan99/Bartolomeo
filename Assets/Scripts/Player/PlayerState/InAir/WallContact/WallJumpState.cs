using UnityEngine;

namespace Assets.Scripts.Player
{
    public class WallJumpState : PlayerState
    {
        private float animtionLength;
        private bool hasAddForce = false;

        public WallJumpState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animtionLength);
        }

        public override void Enter()
        {
            newState = this;
            timer = 0f;
            hasAddForce = false;
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
            WallJumpDirectionUpdate();
            if (pData.DashCooldownTimer > 0)
            {
                pData.DashCooldownTimer -= Time.deltaTime;
            }

            if (timer >= animtionLength)
            {
                Debug.Log(pInput.JumpInput);
                if (pInput.JumpInput)
                {
                    newState = pController.JumpState;
                }
                else if (pInput.DashInput)
                {
                    FacingDirectionUpdate();
                    if (pData.DashCooldownTimer <= 0 && pData.canDash && pData.HasDash)
                    {
                        newState = pController.DashingState;
                        pData.canDash = false;
                    }
                }
                else if (pData.Rb.velocity.y < -23f)
                {
                    pInput.JumpInputCounter = 1;
                    newState = pController.StartFallingState;
                }
            }
        }

        public override void PhysicUpdate()
        {
            if (timer >= animtionLength && !hasAddForce)
            {
                pData.Rb.AddForce(pData.WallJumpDirection * pData.WallJumpVelocity, ForceMode2D.Impulse);
                hasAddForce = true;
            }
            else if (timer <= animtionLength)
            {
                pData.Rb.velocity = Vector2.zero;
            }
        }
    }
}