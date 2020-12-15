using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerData : MonoBehaviour
    {
        public Rigidbody2D Rb { get; private set; }
        public Animator Animator { get; private set; }
        public CapsuleCollider2D CapsuleCollider{ get; private set; }

        public LayerMask GroundMask{ get; private set; }
        public LayerMask WallMask{ get; private set; }

        public float Speed = 20.0f;

        public float DashCooldownTimer = 0.0f;
        public float DashCooldown = 0.3f;
        public float DashVelocityX = 320f;
        public float DashDuration = 0.625f/4f;// = 0.15625f
        public bool canDash = true;

        public float JumpVelocityY = 347.0f;

        public float WallSlideVelocityY = 20.0f;
        public Vector2 WallJumpDirection;
        public float WallJumpVelocity = 347.0f;

        public Transform GroundDetector { get; private set; }
        public Vector2 GroundDetectorSize = new Vector2(20, 11);

        public Transform WallDetector { get; private set; }
        public float WallDetectorLength = 13f;
        public Vector2 WallDetectorDir = Vector2.right;

        public int FacingDirection = 1;

        public Dictionary<string, float> AnimationLength { get; private set; }

        private void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            CapsuleCollider = GetComponent<CapsuleCollider2D>();

            GroundMask = LayerMask.GetMask("Ground");
            WallMask = LayerMask.GetMask("Wall");

            GroundDetector = transform.Find("GroundDetector");
            WallDetector = transform.Find("WallDetector");

            WallJumpDirection = new Vector2(1, 5);

            AnimationLength = new Dictionary<string, float>()
            {
                { "Start_Run_Animation", 0.4f },
                { "Start_Falling_Animation", 0.183f },
                { "Wall_contact", 0.647f },
                { "Wallclimb_unhanged", 0.632f },
                { "Wallclimbing_jump", 0.544f },
                { "Tiep_dat_Animation", 0.571f },
                { "Stop_Run_Animation", 0.175f },
                { "Left_swing_attack", 0.5f },
                {"Right_swing_attack",0.389f },
                {"Upward_clamped",0.6f },
                {"Downward",0.444f },
                {"Right_swing_jump",0.385f },
                {"Left_swing_jump",0.417f }
            };
        }
    }
}
