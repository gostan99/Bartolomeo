using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Player
{
    public class DeathState : PlayerState
    {
        private float animationLength;
        private AudioClip sound;

        public bool AnimationIsFinished { get; private set; }

        public DeathState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animationLength);
            sound = Resources.Load<AudioClip>(@"Sounds/health_cocoon_break");
        }

        public override void Enter()
        {
            timer = 0f;
            newState = this;
            pController.audioSource.Stop();
            pController.audioSource.PlayOneShot(sound);
        }

        public override void Exit()
        {
            throw new NotImplementedException();
        }

        public override void LogicUpdate()
        {
            pInput.InputUpdate();
            timer += Time.deltaTime;
            if (timer >= animationLength && IsGrounded())
            {
                AnimationIsFinished = true;
                // newState = pController.IdleState;
            }
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = new Vector2(0, pData.Rb.velocity.y);
        }
    }
}