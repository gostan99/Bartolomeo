using UnityEngine;

namespace Assets.Scripts.Player
{
    public class WallContactState : PlayerState
    {
        private float animationLength;
        private AudioClip sound;

        public WallContactState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animationLength);
            sound = Resources.Load<AudioClip>(@"Sounds/garden_zombie_prepare");
        }

        public override void Enter()
        {
            timer = 0;
            newState = this;
            pInput.JumpInputCounter = 1;
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
            this.WallDetectorDirectionUpdate();

            if (timer >= animationLength && IsWallContacted() && pInput.xInput != 0)
            {
                newState = pController.WallSlideState;
            }
            else if (timer >= animationLength && pInput.xInput == 0 || timer >= animationLength && !IsWallContacted())
            {
                newState = pController.UnhangedState;
            }
            else if (pInput.JumpInput)
            {
                newState = pController.WallJumpState;
            }
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = new Vector2(pData.Rb.velocity.x, -pData.WallSlideVelocityY);
        }
    }
}