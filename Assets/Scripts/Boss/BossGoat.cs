
using Assets.Scripts.Player;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Entities
{
    class BossGoat : MonoBehaviour
    {
        private PlayerData pData;

        public float Speed = 115;
        private int moveDirection = 1;        //hướng di chuyển
        public float MaxHealth = 3000;
        public float CurrentHealth;

        Animator animator;
        string currentAnimation;

        Rigidbody2D rb;
        private Transform slashPos;                     // vị trí đánh
        public Vector2 slashRange = new Vector2(200, 54 );     // tầm đánh: dài = x, rộng = y
        public float slashAngle = -40;
        public float slashDamage = 10;
        public Vector3 smashOffSet = new Vector3(-5, 45,0);
        public Vector3 smashSize = new Vector3(5, 80,0);
        public Vector3 stompOffSet = new Vector3(-5, 45,0);
        public Vector3 stompSize = new Vector3(175, 80,0);

        int SlashOrStomp = 0;

        public Transform Player;
        private LayerMask playerMask;

        private float IdleTimer;


        private enum State
        {
            Idle, Slash,Smash,Stomp, Dead,
            Chase, TurnAround,SmashToIdle
        }
        State state = State.Chase;


        private void Start()
        {
            CurrentHealth = MaxHealth;

            pData = Player.GetComponent<PlayerData>();
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            playerMask = LayerMask.GetMask("Player");
            slashPos = transform.Find("SlashPos");


            ////cho phép player và entity đi xuyên qua nhau
            //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Entity"));
            ////cho phép entity đi xuyên qua nhau
            //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Entity"), LayerMask.NameToLayer("Entity"));
        }

        // Update is called once per frame
        void Update()
        {

            switch (state)
            {
                case State.Idle:
                    currentAnimation = "Idle_Animation";
                    Idle();
                    break;
                case State.Chase:
                    currentAnimation = "Walk_Animation";
                    Chase();
                    break;
                case State.Slash:
                    currentAnimation = "Slash_Animation";
                    break;
                case State.TurnAround:
                    currentAnimation = "Tunrnaround_Animation";
                    break;
                case State.Smash:
                    currentAnimation = "Smash_Animation";
                    break;
                case State.SmashToIdle:
                    currentAnimation = "Smash_to_Idle_Animation";
                    break;
                case State.Stomp:
                    currentAnimation = "Stomp_Animation";
                    break;
                case State.Dead:
                    currentAnimation = "Dead";
                    break;
            }

            FacingDirectionUpdate();
            animator.Play(currentAnimation);
        }

        private void FixedUpdate()
        {
            // di chuyển
            if (state == State.Chase)
            {
                ApplyMovement();
            }
        }

        void Idle()
        {
            IdleTimer -= Time.deltaTime;
            if (IdleTimer <= 0)
            {
                //Nếu player nằm trong khoảng đánh được
                if (PlayerIsInSlashRange())
                {
                    state = State.Slash;
                    return;
                }

                if (PlayerIsInStompRange())
                {
                    if (SlashOrStomp == 0)
                    {
                        state = State.Smash;
                        SlashOrStomp = 1;
                    }

                    else if (SlashOrStomp == 1)
                    {
                        state = State.Stomp;
                        SlashOrStomp = 0;
                    }
                    //return;
                }
                else if (PlayerIsInSmashRange())
                {
                    state = State.Smash;
                    return;
                }
                state = State.Chase;
            }

        }

        void Chase()
        {
            //nếu hướng về phía player
            if (!IsDirectToPlayer())
            {
                state = State.TurnAround;
                return;
            }

            //Nếu player nằm trong khoảng đánh được
            if (PlayerIsInSlashRange())
            {
                state = State.Slash;
                return;
            }

            if (PlayerIsInStompRange())
            {
                if (SlashOrStomp == 0)
                {
                    state = State.Smash;
                    SlashOrStomp = 1;
                }

                else if (SlashOrStomp == 1)
                {
                    state = State.Stomp;
                    SlashOrStomp = 0;
                }
                return;
            }
            
            if (PlayerIsInSmashRange())
            {
                state = State.Smash;
                return;
            }


            //cập nhật hướng di chuyển tới nhân vật
            _ = (Player.position.x - transform.position.x) > 0 ? moveDirection = 1 : moveDirection = -1;
        }

        //được gọi sau khi hoạt TurnAround kết thúc
        void TurnAroundFinished()
        {
            moveDirection = -moveDirection;
            state = State.Chase;
        }

        //được gọi sau khi hoạt Attack kết thúc
        void SlashFinished()
        {
            state = State.Chase;
        }

        void SmashFinished()
        {
            state = State.SmashToIdle;
        }

        void SmashToIdleFinished()
        {
            IdleTimer = 1;
            state = State.Idle;
        }

        void StompFinished()
        {
            IdleTimer = 1;
            state = State.Idle;
        }

        public void TakeDamage(object[] package)
        {
            // trừ máu
            CurrentHealth -= Convert.ToSingle(package[0]);
            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        void ApplyMovement()
        {
            //Movement
            rb.velocity = new Vector2(Speed * moveDirection, rb.velocity.y);
        }

        //cập nhật hướng quay mặt
        void FacingDirectionUpdate()
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * moveDirection, transform.localScale.y, transform.localScale.z);
        }

        void AttackSlash()
        {
            Collider2D hit = Physics2D.OverlapCapsule(slashPos.position, slashRange, CapsuleDirection2D.Horizontal, slashAngle, playerMask);
            if (hit)
            {
                object[] package = new object[2];
                package[0] = pData.DamageSlash;
                package[1] = pData.FacingDirection;
                hit.SendMessage("TakeDamage", package);
            }
        }

        void AttackStomp()
        {
            Collider2D hit = Physics2D.OverlapBox(stompOffSet + transform.position, stompSize, 0, playerMask);
            if (hit)
            {
                object[] package = new object[2];
                package[0] = pData.DamageStomp;
                package[1] = pData.FacingDirection;
                hit.SendMessage("TakeDamage", package);
            }
        }

        void AttackSmash()
        {
            Collider2D hit = Physics2D.OverlapBox(smashOffSet + transform.position, smashSize, 0, playerMask);
            if (hit)
            {
                object[] package = new object[2];
                package[0] = pData.DamageSmash;
                package[1] = pData.FacingDirection;
                hit.SendMessage("TakeDamage", package);
            }
        }



        void Die()
        {
            gameObject.SetActive(false);
        }

        //Player có nằm trong tầm đánh?
        bool PlayerIsInSlashRange()
        {
            Collider2D _collider = Physics2D.OverlapCapsule(slashPos.position, slashRange, CapsuleDirection2D.Horizontal, slashAngle, playerMask);
            return _collider;
        }

        bool PlayerIsInSmashRange()
        {
            Collider2D _collider = Physics2D.OverlapBox(smashOffSet + transform.position, smashSize, 0,playerMask);
            return _collider;
        }  
        bool PlayerIsInStompRange()
        {
            Collider2D _collider = Physics2D.OverlapBox(stompOffSet + transform.position, stompSize, 0,playerMask);
            return _collider;
        }
        //faceDiection có đang hướng về Player?
        bool IsDirectToPlayer()
        {
            if (transform.position.x < Player.position.x)
            {
                if (moveDirection < 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (moveDirection > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
