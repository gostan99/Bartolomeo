using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerInput
    {
        public float xInput { get; private set; }
        public float yInput { get; private set; }
        

        public bool DashInput { get; private set; }

        public bool JumpInput { get; private set; }
        public bool AttackInput { get; set; }
        public bool UpwardAttackInput { get; set; }
        public bool DownwardAttackInput { get; set; }
        float jumpInputHoldTime;
        public int JumpInputCounter = 0;
        
        const float MAX_JUMP_HOLD_TIME = 0.25f;

        public PlayerInput()
        {
            JumpInput = false;
            xInput = Input.GetAxis("Horizontal");
        }

        private void OnJumpInput()
        {
            if (Input.GetButtonDown("Jump") && JumpInputCounter < PlayerData.MaxJumpCounter )
            {
                JumpInput = true;
                jumpInputHoldTime = 0;
                ++JumpInputCounter;
            }
            if (Input.GetButton("Jump") && JumpInput)
            {
                jumpInputHoldTime += Time.deltaTime;
                if (jumpInputHoldTime >= MAX_JUMP_HOLD_TIME)
                {
                    JumpInput = false;
                }
            }
            else if (Input.GetButtonUp("Jump"))
            {
                JumpInput = false;
            }
        }

        private void OnRunInput()
        {
            xInput = Input.GetAxis("Horizontal");
        }

        private void OnDashInput()
        {
            if (Input.GetButtonDown("Dash"))
            {
                DashInput = true;
            }
            else if (Input.GetButton("Dash"))
            {
                DashInput = false;
            }
        }
        private void OnAttackInput()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) )
            {
                AttackInput = true;
            }
            else 
            {
                AttackInput = false;
            } 
        }

        private void OnVerticalInput()
        {
            yInput = Input.GetAxis("Vertical");
        }
        

        public void InputUpdate()
        {
            OnJumpInput();
            OnRunInput();
            OnDashInput();
            OnAttackInput();          
            OnVerticalInput();
        }
    }
}
