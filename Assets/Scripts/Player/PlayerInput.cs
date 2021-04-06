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
            if (Input.GetKeyDown(KeyCode.Space) && JumpInputCounter < PlayerData.MaxJumpCounter )
            {
                JumpInput = true;
                jumpInputHoldTime = 0;
                ++JumpInputCounter;
            }
            if (Input.GetKey(KeyCode.Space) && JumpInput)
            {
                
                jumpInputHoldTime += Time.deltaTime;
                if (jumpInputHoldTime >= MAX_JUMP_HOLD_TIME)
                {
                    JumpInput = false;
                }
            }
            else if (Input.GetKeyUp(KeyCode.Space))
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
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                DashInput = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
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
