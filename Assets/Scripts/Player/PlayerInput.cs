using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerInput
    {
        public float xInput { get; private set; }
        public float yInput { get; private set; }

        public bool DashInput { get; private set; }

        public bool LargePotionInput { get; private set; }
        public bool SmallPotionInput { get; private set; }
        //public bool HandelInput { get; private set; }
        //public int CheckInput = 0;

        private PlayerData pData;

        public bool JumpInput { get; private set; }
        public bool AttackInput { get; set; }
        public bool UpwardAttackInput { get; set; }
        public bool DownwardAttackInput { get; set; }
        private float jumpInputHoldTime;
        public int JumpInputCounter = 0;

        private const float MAX_JUMP_HOLD_TIME = 0.25f;

        public PlayerInput(PlayerData playerData)
        {
            pData = playerData;
            JumpInput = false;
            xInput = Input.GetAxis("Horizontal");
        }

        private void OnJumpInput()
        {
            if (Input.GetKeyDown(KeyCode.Space) && JumpInputCounter < pData.MaxJumpCounter)
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

        private void OnHealInput()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                LargePotionInput = true;
            }
            else
            {
                LargePotionInput = false;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                SmallPotionInput = true;
            }
            else
            {
                SmallPotionInput = false;
            }
        }

        //private void OnHandelInput()
        //{
        //    if (Input.GetKeyDown(KeyCode.Q) )
        //    {
        //        CheckInput = 1;
        //        HandelInput = true;
        //        
        //    }
        //    else if(Input.GetKeyDown(KeyCode.E))
        //    {
        //        CheckInput = 2;
        //        HandelInput = true;
        //        
        //    }
        //    else if(Input.GetKeyDown(KeyCode.R))
        //    {
        //        CheckInput = 3;
        //        HandelInput = true;
        //    }
        //    else
        //    {
        //        HandelInput = false;
        //        CheckInput = 0;
        //    }
        //}

        private void OnAttackInput()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
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
            OnHealInput();
        }
    }
}