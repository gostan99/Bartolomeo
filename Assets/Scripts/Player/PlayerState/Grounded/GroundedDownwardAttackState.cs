﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class GroundedDownwardAttackState : PlayerState
    {
        private float animationLength;
        private AudioClip sound;

        public GroundedDownwardAttackState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animationLength);
            sound = Resources.Load<AudioClip>(@"Sounds/mage_knight_sword");
        }

        public override void Enter()
        {
            pInput.JumpInputCounter = 0;
            timer = 0f;
            newState = this;
            //pInput.DownwardAttackInput = false;
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
            FacingDirectionUpdate();
            if (timer >= animationLength)
            {
                newState = pController.IdleState;
            }
            else if (pInput.DashInput)
            {
                if (pData.canDash)
                {
                    newState = pController.DashingState;
                    pData.canDash = false;
                }
            }
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = new Vector2(pData.Speed * pInput.xInput, pData.Rb.velocity.y);
        }
    }
}