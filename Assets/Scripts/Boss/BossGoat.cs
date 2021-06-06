using Assets.Scripts.Player;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Entities
{
    internal class BossGoat : MonoBehaviour
    {
        private PlayerData pData;

        public float Speed = 115;
        private int moveDirection = 1;        //hướng di chuyển
        public EnemyData eData;

        private Animator animator;
        private string currentAnimation;

        private Rigidbody2D rb;
        public float DamageSlash = 10f;
        public float DamageStomp = 20f;
        public float DamageSmash = 30f;
        public float slashAngle = -20;
        public float stompAngle = -74;
        private Transform slashPos;                     // vị trí đánh
        private Transform smashPos;                     // vị trí đánh
        private Transform stompPos;                     // vị trí đánh
        private Transform smashDetectorPos;                            // vị trí kiểm tra tầm đánh
        private Transform slashOrStompDetectorPos;                     // vị trí kiểm tra tầm đánh
        private Vector3 attackDetectorRange = new Vector3(90, 109, 0);  // tầm để kiểm tra tầm đánh
        public Vector2 slashRange = new Vector2(200, 54);     // tầm đánh: dài = x, rộng = y
        public Vector3 smashRange = new Vector3(200, 174, 0);
        public Vector3 stompRange = new Vector3(149, 94, 0);

        private void OnDrawGizmos()
        {
            if (smashPos == null)
            {
                return;
            }
            Gizmos.DrawCube(smashPos.position, smashRange);
            //Gizmos.DrawCube(smashDetectorPos.position, attackDetectorRange);
            //Gizmos.DrawCube(slashOrStompDetectorPos.position, attackDetectorRange);
        }

        private int SlashOrStomp = 0;

        public Transform Player;
        private LayerMask playerMask;

        private float IdleTimer;

        private enum State
        {
            Idle, Slash, Smash, Stomp, Dead,
            Chase, TurnAround, SmashToIdle
        }

        private State state = State.Chase;

        private void Start()
        {
            eData = GetComponent<EnemyData>();

            pData = Player.GetComponent<PlayerData>();
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            playerMask = LayerMask.GetMask("Player");
            slashPos = transform.Find("SlashPos");
            smashPos = transform.Find("SmashPos");
            stompPos = transform.Find("StompPos");
            smashDetectorPos = transform.Find("SmashDetectorPos");
            slashOrStompDetectorPos = transform.Find("SlashOrStompDetectorPos");

            ////cho phép player và entity đi xuyên qua nhau
            //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Entity"));
            ////cho phép entity đi xuyên qua nhau
            //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Entity"), LayerMask.NameToLayer("Entity"));
        }

        // Update is called once per frame
        private void Update()
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

        private void Idle()
        {
            IdleTimer -= Time.deltaTime;
            if (IdleTimer <= 0)
            {
                //Nếu player nằm trong khoảng đánh được
                if (PlayerIsInSmashRange())
                {
                    state = State.Smash;
                    return;
                }

                if (PlayerIsInSlashOrStompRange())
                {
                    if (SlashOrStomp == 0)
                    {
                        state = State.Slash;
                        SlashOrStomp = 1;
                    }
                    else if (SlashOrStomp == 1)
                    {
                        state = State.Stomp;
                        SlashOrStomp = 0;
                    }
                    return;
                }
                state = State.Chase;
            }
        }

        private void Chase()
        {
            //nếu hướng về phía player
            if (!IsDirectToPlayer())
            {
                state = State.TurnAround;
                return;
            }

            //Nếu player nằm trong khoảng đánh được
            if (PlayerIsInSmashRange())
            {
                state = State.Smash;
                return;
            }

            if (PlayerIsInSlashOrStompRange())
            {
                if (SlashOrStomp == 0)
                {
                    state = State.Slash;
                    SlashOrStomp = 1;
                }
                else if (SlashOrStomp == 1)
                {
                    state = State.Stomp;
                    SlashOrStomp = 0;
                }
                return;
            }

            //cập nhật hướng di chuyển tới nhân vật
            _ = (Player.position.x - transform.position.x) > 0 ? moveDirection = 1 : moveDirection = -1;
        }

        //được gọi sau khi hoạt TurnAround kết thúc
        private void TurnAroundFinished()
        {
            moveDirection = -moveDirection;
            state = State.Chase;
        }

        //được gọi sau khi hoạt Attack kết thúc
        private void SlashFinished()
        {
            if (PlayerIsInSlashOrStompRange())
            {
                if (SlashOrStomp == 0)
                {
                    state = State.Slash;
                    SlashOrStomp = 1;
                }
                else if (SlashOrStomp == 1)
                {
                    state = State.Stomp;
                    SlashOrStomp = 0;
                }
                return;
            }
            state = State.Chase;
        }

        private void SmashFinished()
        {
            state = State.SmashToIdle;
        }

        private void SmashToIdleFinished()
        {
            if (PlayerIsInSmashRange())
            {
                state = State.Smash;
                return;
            }
            state = State.Chase;
        }

        private void StompFinished()
        {
            if (PlayerIsInSlashOrStompRange())
            {
                if (SlashOrStomp == 0)
                {
                    state = State.Slash;
                    SlashOrStomp = 1;
                }
                else if (SlashOrStomp == 1)
                {
                    state = State.Stomp;
                    SlashOrStomp = 0;
                }
                return;
            }
            state = State.Chase;
        }

        public void TakeDamage(object[] package)
        {
            // trừ máu
            eData.CurrentHealth -= Convert.ToSingle(package[0]);
            if (eData.CurrentHealth <= 0)
            {
                Die();
            }
        }

        private void ApplyMovement()
        {
            //Movement
            rb.velocity = new Vector2(Speed * moveDirection, rb.velocity.y);
        }

        //cập nhật hướng quay mặt
        private void FacingDirectionUpdate()
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * moveDirection, transform.localScale.y, transform.localScale.z);
        }

        private void AttackSlash()
        {
            Collider2D hit = Physics2D.OverlapCapsule(slashPos.position, slashRange, CapsuleDirection2D.Horizontal, slashAngle, playerMask);
            if (hit)
            {
                object[] package = new object[1];
                package[0] = DamageSlash;
                hit.SendMessage("TakeDamage", package);
            }
        }

        private void AttackStomp()
        {
            Collider2D hit = Physics2D.OverlapBox(stompPos.position, stompRange, stompAngle, playerMask);
            if (hit)
            {
                object[] package = new object[1];
                package[0] = DamageStomp;
                hit.SendMessage("TakeDamage", package);
            }
        }

        private void AttackSmash()
        {
            Collider2D hit = Physics2D.OverlapBox(smashPos.position, smashRange, 0, playerMask);
            if (hit)
            {
                object[] package = new object[1];
                package[0] = DamageSmash;
                hit.SendMessage("TakeDamage", package);
            }
        }

        private void Die()
        {
            GetComponent<SpriteRenderer>().enabled = false;
            Destroy(this.gameObject, 5);
        }

        //Player có nằm trong tầm đánh?
        private bool PlayerIsInSlashOrStompRange()
        {
            var _collider = Physics2D.OverlapBox(slashOrStompDetectorPos.position, attackDetectorRange, 0, playerMask);
            return _collider;
        }

        private bool PlayerIsInSmashRange()
        {
            var _collider = Physics2D.OverlapBox(smashDetectorPos.position, attackDetectorRange, 0, playerMask);
            return _collider;
        }

        //faceDiection có đang hướng về Player?
        private bool IsDirectToPlayer()
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