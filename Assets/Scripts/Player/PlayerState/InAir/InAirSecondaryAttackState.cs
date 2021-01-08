﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class InAirSecondaryAttackState : PlayerState
    {
        float animationLength;
        bool hasAttackTwice = false;
        bool hasAttackUp = false;
        public InAirSecondaryAttackState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animationLength);
        }
        public override void Enter()
        {
            pInput.JumpInputCounter = 0;
            timer = 0F;
            newState = this;
            pInput.AttackInput = false;
            hasAttackTwice = false;
            hasAttackUp = false;
            FacingDirectionUpdate();
        }

        public override void Exit()
        {
            throw new NotImplementedException();
        }
        public override void LogicUpdate()
        {
            pInput.InputUpdate();
            timer += Time.deltaTime;

            if (pData.CollidedObjects.Count != 0)
            {
                foreach (var collider in pData.CollidedObjects)
                {
                    object[] package = new object[2];
                    package[0] = pData.AttackDamage;
                    package[1] = pData.FacingDirection;
                    collider.SendMessage("TakeDamage", package);
                }
                pData.CollidedObjects.Clear();
            }

            if (timer <= animationLength && pInput.AttackInput && !IsGrounded())
            {
                _ = pInput.yInput < 0 ? hasAttackUp = true : hasAttackTwice = true;
                pController.StartCoroutine(WaitAfter(animationLength - timer));
            }
            else if (timer >= animationLength)
            {
                if (hasAttackTwice || hasAttackUp)
                {
                }
                else
                {
                    newState = pController.IdleState;
                }
            }
            else if (pInput.DashInput)
            {
                newState = pController.DashingState;
            }
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = new Vector2(pData.Speed * pInput.xInput, pData.Rb.velocity.y);

        }

        IEnumerator WaitAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (hasAttackTwice)
            {
                newState = pController.InAirPrimaryAttackState;
            }
            else if (hasAttackUp)
            {
                newState = pController.InAirUpwardAttackState;
            }
        }

    }
}
