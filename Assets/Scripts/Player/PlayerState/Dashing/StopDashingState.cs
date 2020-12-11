using UnityEngine;

namespace Assets.Scripts.Player
{
    public class StopDashingState : PlayerState
    {
        float animtionLength;


        public StopDashingState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animtionLength);
        }

        public override void Enter()
        {
            timer = 0f;
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
            if (timer>=animtionLength)
            {
                newState = pController.IdleState;
            }
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = Vector2.zero;
        }
    }
}