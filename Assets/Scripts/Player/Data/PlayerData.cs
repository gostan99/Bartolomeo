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
        public CapsuleCollider2D CapsuleCollider { get; private set; }

        //--LAYER------------------------------------------------------
        public LayerMask GroundMask { get; private set; }

        public LayerMask WallMask { get; private set; }
        public LayerMask EntityMask { get; private set; }
        public LayerMask NextLevelMask { get; private set; }

        //--HITBOX-----------------------------------------------------
        public List<Collider2D> CollidedObjects = new List<Collider2D>();

        //Primaray Attack
        public Transform PrimaryHitboxPos;

        public Vector2 PrimaryHitboxSize = new Vector2(103, 37);
        public CapsuleDirection2D PrimaryHitboxDirection = CapsuleDirection2D.Horizontal;

        //In Air Primaray Attack
        public Transform InAirPrimaryHitboxPos;

        public Vector2 InAirPrimaryHitboxSize = new Vector2(92, 38);
        public CapsuleDirection2D InAirPrimaryHitboxDirection = CapsuleDirection2D.Horizontal;

        //Grounded UpWard Attack
        public Transform GroundedUpwardHitboxPos;

        public Vector2 GroundedUpwardHitboxSize = new Vector2(79, 103);
        public CapsuleDirection2D GroundedUpwardHitboxDirection = CapsuleDirection2D.Vertical;

        //In Air UpWard Attack
        public Transform InAirUpwardHitboxPos;

        public Vector2 InAirUpwardHitboxSize = new Vector2(79, 102);
        public CapsuleDirection2D InAirUpwardHitboxDirection = CapsuleDirection2D.Vertical;
        //--------------------------------------------------------------

        //--PARAMETER--------------------------------------------------
        public PlayerSerializeData serializeData = new PlayerSerializeData();

        public ref bool HasCheckPoint { get { return ref serializeData.HasCheckPoint; } }
        public ref float PosX { get { return ref serializeData.PosX; } }
        public ref float PosY { get { return ref serializeData.PosY; } }
        public ref float AttackDamage { get { return ref serializeData.AttackDamage; } }
        public ref float maxMana { get { return ref serializeData.maxMana; } }
        public ref float currentMana { get { return ref serializeData.currentMana; } }
        public ref float manaCost { get { return ref serializeData.manaCost; } }

        public ref float manaGenerationTimer { get { return ref serializeData.manaGenerationTimer; } }

        public ref float currentHealth { get { return ref serializeData.currentHealth; } }
        public ref float maxHealth { get { return ref serializeData.maxHealth; } }

        public ref float Speed { get { return ref serializeData.Speed; } }

        public ref float DashCooldownTimer { get { return ref serializeData.DashCooldownTimer; } }

        public ref float DashCooldown { get { return ref serializeData.DashCooldown; } }

        public ref float DashVelocityX { get { return ref serializeData.DashVelocityX; } }

        public ref float DashDuration { get { return ref serializeData.DashDuration; } }

        public ref bool canDash { get { return ref serializeData.canDash; } }

        public ref float JumpVelocityY { get { return ref serializeData.JumpVelocityY; } }

        public ref float WallSlideVelocityY { get { return ref serializeData.WallSlideVelocityY; } }
        public ref float WallJumpVelocity { get { return ref serializeData.WallJumpVelocity; } }

        public Vector2 WallJumpDirection;

        public ref int MaxJumpCounter { get { return ref serializeData.MaxJumpCounter; } }
        public ref bool HasDash { get { return ref serializeData.HasDash; } }
        public ref int Money { get { return ref serializeData.Money; } }

        //--TAKEHIT----------------------------------------------------
        public float invulnerableTimer = 0f;

        public float invulnerableTime = 2f;

        //--GROUND DETECTOR--------------------------------------------
        public Transform GroundDetector { get; private set; }

        public Vector2 GroundDetectorSize = new Vector2(20, 11);

        //--WALL DETECTOR----------------------------------------------
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
            EntityMask = LayerMask.GetMask("Entity");
            NextLevelMask = LayerMask.GetMask("NextLevel");

            GroundDetector = transform.Find("GroundDetector");
            WallDetector = transform.Find("WallDetector");

            PrimaryHitboxPos = transform.Find("PrimaryHitbox");
            InAirPrimaryHitboxPos = transform.Find("InAirPrimaryHitbox");
            GroundedUpwardHitboxPos = transform.Find("GroundedUpwardHitbox");
            InAirUpwardHitboxPos = transform.Find("InAirUpwardHitbox");

            WallJumpDirection = new Vector2(1, 5);

            AnimationLength = new Dictionary<string, float>()
            {
                { "Start_Run_Animation", 0.4f },
                { "Start_Falling_Animation", 0.183f },
                { "Wall_contact", 0.44f },
                { "Wallclimb_unhanged", 0.6f },
                { "Wallclimbing_jump", 0.517f },
                { "Tiep_dat_Animation", 0.4f},
                { "Stop_Run_Animation", 0.175f },
                {"Downward",0.444f },
                { "Left_swing_attack", 0.3f },
                {"Right_swing_attack",0.233f },
                {"Right_swing_jump",0.263f },
                {"Left_swing_jump",0.294f },
                {"Upward_clamped",0.267f },
                {"Upward_jump",0.222f }
            };
        }

        public class PlayerSerializeData
        {
            public bool HasCheckPoint = false;
            public float PosX;
            public float PosY;
            public float AttackDamage = 20f;
            public float maxMana = 100;
            public float currentMana = 100;
            public float manaCost = 50;
            public float manaGenerationTimer = 0.25f;
            public float currentHealth = 100;
            public float maxHealth = 100;
            public float Speed = 178f;
            public float DashCooldownTimer = 0.0f;
            public float DashCooldown = 0.4f;
            public float DashVelocityX = 973;
            public float DashDuration = 0.625f / 4f;// = 0.15625f
            public bool canDash = true;
            public float JumpVelocityY = 260.0f;
            public float WallSlideVelocityY = 20.0f;
            public float WallJumpVelocity = 110;
            public int MaxJumpCounter = 2;
            public bool HasDash = true;
            public int Money = 0;
        }
    }
}