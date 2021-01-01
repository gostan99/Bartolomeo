
using System;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    class BossGoat : MonoBehaviour
    {
        public float Speed = 115;
        private int moveDirection = 1;        //hướng di chuyển
        public float MaxHealth = 500;
        public float CurrentHealth;

        Animator animator;
        string currentAnimation;

        Rigidbody2D rb;
        private Transform slashPos;                     // vị trí đánh
        public Vector2 slashRange = new Vector2(200, 54);     // tầm đánh: dài = x, rộng = y
        public float slashAngle = -40;
        public float slashDamage = 10;

        public Transform Player;
        private LayerMask playerMask;

        private enum State
        {
            Idle, Slash, Dead,
            Chase, TurnAround
        }
        State state = State.Chase;


        private void Start()
        {
            CurrentHealth = MaxHealth;

            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            playerMask = LayerMask.GetMask("Player");
            slashPos = transform.Find("SlashPos");

            //cho phép player và entity đi xuyên qua nhau
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Entity"));
            //cho phép entity đi xuyên qua nhau
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Entity"), LayerMask.NameToLayer("Entity"));
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case State.Idle:
                    currentAnimation = "Idle_Animation";
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
                case State.Dead:
                    //currentAnimation = "Dead";
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

        void Chase()
        {
            //nếu hướng về phía player
            if (!IsDirectToPlayer())
            {
                state = State.TurnAround;
                return;
            }

            //Nếu player nằm trong khoảng đánh được
            if (PlayerIsInAttackRange())
            {
                state = State.Slash;
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

        //Player có nằm trong tầm đánh?
        bool PlayerIsInAttackRange()
        {
            Collider2D _collider = Physics2D.OverlapCapsule(slashPos.position, slashRange, CapsuleDirection2D.Horizontal, slashAngle, playerMask);
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
