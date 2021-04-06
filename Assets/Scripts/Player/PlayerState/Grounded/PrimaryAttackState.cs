using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PrimaryAttackState : PlayerState
    {

        float animationLength;
        bool hasAttackTwice = false;
        private bool hasAttackUp = false;

        public PrimaryAttackState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animationLength);
        }
        public override void Enter()
        {
            pInput.JumpInputCounter = 0;
            timer = 0f;
            newState = this;
            pInput.AttackInput = false;
            hasAttackTwice = false;
            hasAttackUp = false;
            pInput.UpwardAttackInput = false;
            pInput.DownwardAttackInput = false;
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

            if (pData.CollidedObjects.Count !=0 )
            {
                foreach (var collider in pData.CollidedObjects)
                {
                    object[] package = new object[3];
                    package[0] = pData.AttackDamage;
                    package[1] = pData.FacingDirection;
                    package[2] = pData;
                    collider.SendMessage("TakeDamage", package);
                }
                pData.CollidedObjects.Clear();
            }

            if (timer <= animationLength && pInput.AttackInput)
            {
                _ = pInput.yInput < 0 ? hasAttackUp = true : hasAttackTwice = true;
                pController.StartCoroutine(WaitAfter(animationLength - timer));
            }
            else if (timer >= animationLength)
            {
                if (hasAttackTwice || hasAttackTwice)
                {

                }
                else if (pInput.xInput == 0)
                {
                    newState = pController.IdleState;
                }
                else if (pInput.xInput != 0)
                {
                    newState = pController.StartRunState;
                }
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
            pData.Rb.velocity = new Vector2(0, pData.Rb.velocity.y);
        }

        IEnumerator WaitAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (hasAttackTwice)
            {
                newState = pController.GroundedSecondaryAttackState;
            }
            else if (hasAttackUp)
            {
                newState = pController.GroundedUpwardAttackState;
            }
        }
    }
}
