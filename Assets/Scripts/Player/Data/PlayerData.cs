using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerData : MonoBehaviour
    {
        //-- COMPONENT-------------------------------------------------
        public Rigidbody2D Rb { get; private set; }
        public Animator Animator { get; private set; }
        public CapsuleCollider2D CapsuleCollider{ get; private set; }

        //--LAYER------------------------------------------------------
        public LayerMask GroundMask{ get; private set; }
        public LayerMask WallMask{ get; private set; }
        public LayerMask EntityMask{ get; private set; }

        //--HITBOX-----------------------------------------------------
        public Transform HitboxPos;
        public Vector2 HitboxSize = new Vector2(103,37);
        public CapsuleDirection2D CapsuleHitboxDirection = CapsuleDirection2D.Horizontal;
        public float AttackDamage = 20f; 
        public List<Collider2D> CollidedObjects = new List<Collider2D>();

        //--PARAMETER--------------------------------------------------
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

        //--SERIALIZE DATA---------------------------------------------
        public static int MaxJumpCounter = 2;
        public static bool HasDash = true;


        //--GROUND DETECTOR--------------------------------------------
        public Transform GroundDetector { get; private set; }
        public Vector2 GroundDetectorSize = new Vector2(20, 11);

        //--WALL DETECTOR----------------------------------------------
        public Transform WallDetector { get; private set; }
        public float WallDetectorLength = 13f;
        public Vector2 WallDetectorDir = Vector2.right;

        public int FacingDirection = 1;

        public Dictionary<string, float> AnimationLength { get; private set; }

        public int OurHealth;
        public int MaxHealth = 10;

        private void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            CapsuleCollider = GetComponent<CapsuleCollider2D>();

            GroundMask = LayerMask.GetMask("Ground");
            WallMask = LayerMask.GetMask("Wall");
            EntityMask = LayerMask.GetMask("Entity");

            GroundDetector = transform.Find("GroundDetector");
            WallDetector = transform.Find("WallDetector");
            HitboxPos = transform.Find("Hitbox");

            WallJumpDirection = new Vector2(1, 5);

            OurHealth = MaxHealth;

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
                {"Left_swing_jump",0.417f },
                {"Upward_jump",0.526f }
            };
        }
    }
}
