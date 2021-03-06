﻿using Assets.Scripts.Player;
using System;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    internal class Skeleton : MonoBehaviour
    {
        public EnemyData eData;

        public float Speed = 4;
        private int facingDirection = 1;        //hướng quay mặt
        //public float MaxHealth = 100;
        //public float CurrentHealth;

        public GameObject PatrolPoint;          // vị trí để đi tuần tra
        public float PatrolDistance = 50f;      // khoảng cách giới hạn để đi tuần tra

        private Animator animator;
        private string currentAnimation = "Patrol";

        private Rigidbody2D rb;

        private RaycastHit2D playerDetector;
        public float PlayerDetectRange = 125f;                // tầm phát hiện player
        private Transform attackPos;                          // vị trí đánh
        public Vector2 AttackRange = new Vector2(75, 19);     // tầm đánh: dài = x, rộng = y
        public float AttackDamage = 10;

        private LayerMask playerMask;
        private Transform groundDetector;        //Dùng để làm vị trí gốc cho Raycast kiểm tra va chạm với mặt đất
        public float groundDetectorLength = 56f; //Độ dài tia Raycast
        private int groundMask;                  //Ground layer
        private int wallMask;                    //Wall layer
        public float wallDetectorLength = 16f;   //Độ dài tia Raycast

        public float idleTime = 2f;              // thời gian ở trạng thái idle là 2s
        private float idleTimer;                         // đếm thời gian ở trạng thái idle còn lại

        private Canvas healthCanvas;

        private enum State
        {
            Idle, Patrol, Attack, Dead,
            TakeHit, Chase
        }

        private State state = State.Patrol;

        private void Start()
        {
            eData = GetComponent<EnemyData>();
            //CurrentHealth = MaxHealth;

            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            playerMask = LayerMask.GetMask("Player");
            attackPos = transform.Find("AttackPos");
            groundDetector = transform.Find("GroundDetector");
            groundMask = LayerMask.GetMask("Ground");
            wallMask = LayerMask.GetMask("Wall");

            //cho phép player và entity đi xuyên qua nhau
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Entity"));
            //cho phép entity đi xuyên qua nhau
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Entity"), LayerMask.NameToLayer("Entity"));

            healthCanvas = transform.GetComponentInChildren<Canvas>();
            healthCanvas.enabled = false;
        }

        // Update is called once per frame
        private void Update()
        {
            //cập nhật biến playerDetector
            LookingForPlayer();

            switch (state)
            {
                case State.Idle:
                    currentAnimation = "Idle";
                    Idle();
                    break;

                case State.Patrol:
                    currentAnimation = "Walk";
                    Patrol();
                    break;

                case State.Chase:
                    currentAnimation = "Walk";
                    Chase();
                    break;

                case State.TakeHit:
                    currentAnimation = "TakeHit";
                    break;

                case State.Attack:
                    currentAnimation = "Attack";
                    break;

                case State.Dead:
                    currentAnimation = "Dead";
                    break;
            }

            FacingDirectionUpdate();
            animator.Play(currentAnimation);

            if (eData.CurrentHealth <= 0)
            {
                healthCanvas.enabled = false;
            }
        }

        private void FixedUpdate()
        {
            // di chuyển
            if ((state == State.Chase || state == State.Patrol) && !IsDeadEnd())
            {
                ApplyMovement();
            }
        }

        private void Idle()
        {
            // giảm thời gian ở trạng thái idle còn lại
            idleTimer -= Time.deltaTime;

            // nếu hết thời gian ở trạng thái idle thì chuyển sang trạng thái Patrol
            if (idleTimer <= 0)
            {
                state = State.Patrol;
                facingDirection = -facingDirection;
            }

            //nếu phát hiện player
            if (playerDetector && !IsDeadEnd())
            {
                state = State.Chase;
                return;
            }
        }

        private void Patrol()
        {
            //nếu đi quá xa
            if (Vector2.Distance(transform.position, PatrolPoint.transform.position) >= PatrolDistance || IsDeadEnd())
            {
                if (!IsDirectToPatrolPoint())
                {
                    idleTimer = idleTime;
                    state = State.Idle;
                    return;
                }
            }

            //nếu gặp đường cụt
            if (IsDeadEnd())
            {
                idleTimer = idleTime;
                state = State.Idle;
                return;
            }

            //nếu phát hiện player
            if (playerDetector)
            {
                state = State.Chase;
                return;
            }
        }

        private void Chase()
        {
            //nếu không phát hiện player nữa hoặc cụt đường thì chuyển sang trạng thái Idle
            if (!playerDetector || IsDeadEnd())
            {
                state = State.Idle;
                idleTimer = idleTime;
                return;
            }

            //Nếu player nằm trong attack range
            if (PlayerIsInAttackRange())
            {
                state = State.Attack;
                return;
            }

            //cập nhật hướng di chuyển tới nhân vật
            _ = (playerDetector.transform.position.x - transform.position.x) > 0 ? facingDirection = 1 : facingDirection = -1;
        }

        // được gọi bơi Attack animation
        private void Attack()
        {
            if (eData.CurrentHealth <= 0)
            {
                return;
            }
            Collider2D hit = Physics2D.OverlapCapsule(attackPos.transform.position, new Vector2(AttackRange.x * facingDirection, AttackRange.y), CapsuleDirection2D.Horizontal, 0, playerMask);
            if (hit)
            {
                object[] package = new object[2];
                package[0] = AttackDamage;
                package[1] = facingDirection;
                hit.SendMessage("TakeDamage", package);
            }
        }

        // được gọi bởi Attack animation và TakeHit animaiton sau khi nó kết thúc hoạt ảnh
        private void BackToChase()
        {
            if (PlayerIsInAttackRange())
            {
                state = State.Attack;
            }
            else
            {
                state = State.Chase;
            }
        }

        //Được gọi bởi Player
        //Packege[0] là lượng dame, Package[1] là hướng bị đánh
        public void TakeDamage(object[] package)
        {
            if (eData.CurrentHealth <= 0)
            {
                return;
            }
            if (!healthCanvas.isActiveAndEnabled)
            {
                healthCanvas.enabled = true;
            }

            if (state == State.Dead)
            {
                return;
            }

            state = State.TakeHit;

            // trừ máu
            eData.CurrentHealth -= Convert.ToSingle(package[0]);
            if (eData.CurrentHealth <= 0)
            {
                if (package.Length == 3)
                {
                    PlayerData playerData = (PlayerData)package[2];
                    if (playerData.currentMana == playerData.maxMana)
                    {
                        playerData.currentMana += 0;
                    }
                    else
                    {
                        playerData.currentMana += 10;
                    }
                }
                state = State.Dead;
                Destroy(this.gameObject, 5);
            }
        }

        private void ApplyMovement()
        {
            //Movement
            rb.velocity = new Vector2(Speed * facingDirection, rb.velocity.y);
        }

        private void LookingForPlayer()
        {
            var tempPos = new Vector3(transform.position.x, transform.position.y - 30, transform.position.z);
            playerDetector = Physics2D.Raycast(tempPos, Vector2.right, PlayerDetectRange, playerMask);

            if (!playerDetector)
            {
                playerDetector = Physics2D.Raycast(tempPos, Vector2.right * -1, PlayerDetectRange, playerMask);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * PlayerDetectRange);
        }

        //cập nhật hướng quay mặt
        private void FacingDirectionUpdate()
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * facingDirection, transform.localScale.y, transform.localScale.z);
        }

        //moveDiection có đang hướng về PatrolPoint?
        private bool IsDirectToPatrolPoint()
        {
            if (transform.position.x < PatrolPoint.transform.position.x)
            {
                if (facingDirection < 0)
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
                if (facingDirection > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        // kiểm  tra có tới đường cụt?
        // đường cụt là khi phía trước gặp vực hoặc tường
        private bool IsDeadEnd()
        {
            //Physics2D.Raycast() sẽ trả giá trị true nếu nó va chạm với groundMask
            //groundDetector.transform.position là gốc của tia Raycast
            //Vector2.down là hướng bắn của tia
            //56.0f là độ dài của tia
            var groundCollided = Physics2D.Raycast(groundDetector.transform.position, Vector2.down, groundDetectorLength, groundMask);
            var wallCollided = Physics2D.Raycast(transform.position, Vector2.right * facingDirection, wallDetectorLength, wallMask);
            if (!groundCollided || wallCollided)
            {
                return true;
            }
            return false;
        }

        //Player có nằm trong tầm đánh?
        private bool PlayerIsInAttackRange()
        {
            Collider2D _collider = Physics2D.OverlapCapsule(attackPos.transform.position, AttackRange, CapsuleDirection2D.Horizontal, 0, playerMask);
            return _collider;
        }
    }
}