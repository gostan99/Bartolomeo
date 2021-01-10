using UnityEngine;

namespace Assets.Scripts.Player
{
    public class UnhangedState : PlayerState
    {
        float animationLength;


        public UnhangedState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animationLength);
        }

        public override void Enter()
        {
            timer = 0;
            newState = this;

        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }

        public override void LogicUpdate()
        {
            timer += Time.deltaTime;
            
            if (timer >= animationLength)
            {
                newState = pController.FallingState;
            }
            else if (pInput.DashInput)
            {
                FacingDirectionUpdate();
                newState = pController.DashingState;
            }
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = new Vector2(pData.Rb.velocity.x, 0);
        }
    }
}