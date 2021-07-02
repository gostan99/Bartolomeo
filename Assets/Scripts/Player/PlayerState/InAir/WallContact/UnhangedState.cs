using UnityEngine;

namespace Assets.Scripts.Player
{
    public class UnhangedState : PlayerState
    {
        private float animationLength;
        private AudioClip sound;

        public UnhangedState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animationLength);
            sound = Resources.Load<AudioClip>(@"Sounds/garden_zombie_prepare - Reverse");
        }

        public override void Enter()
        {
            timer = 0;
            pController.audioSource.PlayOneShot(sound);
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
                if (pData.DashCooldownTimer <= 0 && pData.canDash && pData.HasDash)
                {
                    newState = pController.DashingState;
                    pData.canDash = false;
                }
            }
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = new Vector2(pData.Rb.velocity.x, 0);
        }
    }
}