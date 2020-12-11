using UnityEngine;

namespace Assets.Scripts.Player
{
    public class LandingState : PlayerState
    {
        float animationLength;


        public LandingState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
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
            pInput.InputUpdate();
            if (timer >= animationLength)
            {
                newState = pController.IdleState;
            }
        }

        public override void PhysicUpdate()
        {
        }
    }
}