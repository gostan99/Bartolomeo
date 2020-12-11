using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public abstract class PlayerState
    {
        protected PlayerController pController;
        protected PlayerInput pInput;
        protected PlayerData pData;

        protected string animation;
        protected PlayerState newState;
        protected float timer;

        private PlayerState()
        {

        }

        public PlayerState( PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation )
        {
            pController = playerController;
            pInput = playerInput;
            pData = playerData;

            this.animation = animation;

            timer = 0f;
            newState = this;
        }

        public PlayerState GetChangedState()
        {
            return newState;
        }

        public abstract void LogicUpdate();

        public abstract void PhysicUpdate();

        public abstract void Enter();

        public abstract void Exit();

        public void PlayAnimation()
        {
            pData.Animator.Play(animation);
        }

        protected void FacingDirectionUpdate()
        {
            if (pInput.xInput > 0)
            {
                pData.FacingDirection = 1;
            }
            if (pInput.xInput < 0)
            {
                pData.FacingDirection = -1;
            }
            pData.transform.localScale = new Vector3(Mathf.Abs(pData.transform.localScale.x) * pData.FacingDirection, pData.transform.localScale.y, pData.transform.localScale.z);
        }

        protected bool IsGrounded()
        {
            var collided = Physics2D.OverlapBox(pData.GroundDetector.transform.position, pData.GroundDetectorSize, 0f, pData.GroundMask, 0f, 0f);
            return collided;
        }

        protected bool IsWallContacted()
        {
            var collided = Physics2D.Raycast(pData.WallDetector.transform.position, pData.WallDetectorDir, pData.WallDetectorLength, pData.WallMask);
            return collided;
        }

        protected void WallDetectorDirectionUpdate()
        {
            if (pInput.xInput > 0)
            {
                pData.WallDetectorDir = Vector2.right;
            }
            if (pInput.xInput < 0)
            {
                pData.WallDetectorDir = Vector2.right * -1;
            }
        }

        protected void WallJumpDirectionUpdate()
        {
            if (pData.FacingDirection < 0 )
            {
                pData.WallJumpDirection = new Vector2(Mathf.Abs(pData.WallJumpDirection.x), pData.WallJumpDirection.y);
            }
            else if (pData.FacingDirection > 0)
            {
                pData.WallJumpDirection = new Vector2(Mathf.Abs(pData.WallJumpDirection.x) * -1, pData.WallJumpDirection.y);
            }
        }
    }
}
