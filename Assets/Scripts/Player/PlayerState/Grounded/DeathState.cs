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
        float animationLength;
        public DeathState(PlayerController playerController, PlayerInput playerInput, PlayerData playerData, string animation) : base(playerController, playerInput, playerData, animation)
        {
            pData.AnimationLength.TryGetValue(animation, out animationLength);
        }
        public override void Enter()
        {
            pInput.JumpInputCounter = 0;
            timer = 0F;
            newState = this;
            pInput.AttackInput = false;
        }

        public override void Exit()
        {
            throw new NotImplementedException();
        }

        public override void LogicUpdate()
        {
        }

        public override void PhysicUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
