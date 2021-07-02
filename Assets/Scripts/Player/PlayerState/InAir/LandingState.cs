using UnityEngine;

namespace Assets.Scripts.Player
{
    public class LandingState : PlayerState
    {
        private AudioClip sound;
        private float animationLength;

        public LandingState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animationLength);
            sound = Resources.Load<AudioClip>(@"Sounds/hero_land_hard");
        }

        public override void Enter()
        {
            timer = 0;
            newState = this;
            pController.audioSource.Stop();
            pController.audioSource.PlayOneShot(sound);
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