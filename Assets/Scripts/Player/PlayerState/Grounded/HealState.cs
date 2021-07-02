using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class HealState : PlayerState
    {
        private float animationLength;
        private AudioClip sound;

        public HealState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animationLength);
            sound = Resources.Load<AudioClip>(@"Sounds/focus_health_heal");
        }

        public override void Enter()
        {
            pInput.JumpInputCounter = 0;
            timer = 0f;
            newState = this;
            pInput.UpwardAttackInput = false;
            //fullHeath(); // không gọi phương thức này vì đã có item hồi máu lo nhiệm vụ thêm máu cho player
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
            FacingDirectionUpdate();

            if (pData.CollidedObjects.Count != 0)
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

            if (timer >= animationLength)
            {
                if (pInput.xInput == 0)
                {
                    newState = pController.IdleState;
                }
                else
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

        public void fullHeath()
        {
            if (pData.currentHealth == pData.maxHealth)
            {
                pData.currentHealth += 0;
            }
            else
            {
                pData.currentHealth += 20;
            }
        }

        public override void PhysicUpdate()
        {
            pData.Rb.velocity = new Vector2(0, pData.Rb.velocity.y);
        }
    }
}