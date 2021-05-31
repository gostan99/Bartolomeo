using UnityEngine;

namespace Assets.Scripts.Player
{
    public class StartFallingState : PlayerState
    {
        private float animationLength;

        public StartFallingState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animationLength);
        }

        public override void Enter()
        {
            timer = 0f;
            newState = this;
        }

        public override void Exit()
        {
        }

        public override void LogicUpdate()
        {
            timer += Time.deltaTime;
            pInput.InputUpdate();
            WallDetectorDirectionUpdate();
            FacingDirectionUpdate();
            if (pData.DashCooldownTimer > 0)
            {
                pData.DashCooldownTimer -= Time.deltaTime;
            }

            if (timer >= animationLength)
            {
                newState = pController.FallingState;
            }
            else if (pInput.JumpInput)
            {
                newState = pController.JumpState;
            }
            else if (IsWallContacted() && pInput.xInput != 0)
            {
                newState = pController.WallContactState;
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
        }

        public override void PhysicUpdate()
        {
            //Movment
            pData.Rb.velocity = new Vector2(pData.Speed * pInput.xInput, pData.Rb.velocity.y);
        }
    }
}