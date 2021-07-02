using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class JumpState : PlayerState
    {
        private AudioClip sound;

        public JumpState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            sound = Resources.Load<AudioClip>(@"Sounds/hero_jump");
        }

        public override void LogicUpdate()
        {
            pInput.InputUpdate();
            FacingDirectionUpdate();
            if (pData.DashCooldownTimer > 0)
            {
                pData.DashCooldownTimer -= Time.deltaTime;
            }

            if (pData.Rb.velocity.y < 0f)
            {
                newState = pController.StartFallingState;
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
            if (pInput.JumpInput)
            {
                pData.Rb.velocity = new Vector2(pData.Rb.velocity.x, pData.JumpVelocityY);
            }
        }

        public override void Enter()
        {
            newState = this;
            pController.audioSource.Stop();
            pController.audioSource.PlayOneShot(sound);
        }

        public override void Exit()
        {
            throw new NotImplementedException();
        }
    }
}