using UnityEngine;

namespace Assets.Scripts.Player
{
    public class StartRunState : PlayerState
    {
        float animationLength;

        public StartRunState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animationLength);
        }


        public override void Enter()
        {
            newState = this;
            timer = 0f;
            pData.canDash = true;
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }

        public override void LogicUpdate()
        {
            timer += Time.deltaTime;
            pInput.InputUpdate();
            FacingDirectionUpdate();
            if (pData.DashCooldownTimer > 0)
            {
                pData.DashCooldownTimer -= Time.deltaTime;
            }

            if (timer >= animationLength)
            {
                newState = pController.RunningState;
            }
            else if (pInput.xInput == 0)
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
                if (pData.DashCooldownTimer <= 0 && pData.HasDash)
                {
                    newState = pController.DashingState;
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
                else
                {
                    newState = pController.GroundedUpwardAttackState;
                }
            }
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = new Vector2(pData.Speed * pInput.xInput, pData.Rb.velocity.y);
            
        }
    }
}