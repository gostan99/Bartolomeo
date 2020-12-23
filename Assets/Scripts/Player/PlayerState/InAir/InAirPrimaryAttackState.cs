using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class InAirPrimaryAttackState : PlayerState
    {
        float animationLength;
        bool hasAttackTwice = false;
        private bool hasAttackUp = false;

        public InAirPrimaryAttackState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
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
                newState = pController.InAirSecondaryAttackState;
            }
            else if (hasAttackUp)
            {
               newState = pController.InAirUpwardAttackState;
            }
        }
    }
}

