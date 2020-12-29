
using System;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    class Skeleton : MonoBehaviour
    {
        public float Speed = 4;
        private int moveDirection = 1;          //hướng di chuyển
        public float MaxHealth = 100;
        public float CurrentHealth;

        public GameObject PatrolPoint;      // vị trí để đi tuần tra
        public float PatrolDistance = 50f;      // khoảng cách giới hạn để đi tuần tra

        Animator animator;
        string currentAnimation = "Patrol";

        Rigidbody2D rb;

        RaycastHit2D playerDetector;
        public float PlayerDetectRange = 26f; // tầm phát hiện player
        public Vector3 AttackOffSet = new Vector3(16, 12, 0); // vị trí đánh
        public Vector2 AttackRange = new Vector2(44, 56); // tầm đánh
        public float AttackDamage = 10;
        private LayerMask playerMask;

        private Transform groundDetector;     //Dùng để làm vị trí gốc cho Raycast
        private int groundMask;               //Ground layer

        public float idleTime = 2f; // thời gian ở trạng thái idle là 2s
        float idleTimer; // đếm thời gian ở trạng thái idle còn lại

        private enum State
        {
            Idle, Patrol, Attack, Dead,
            TakeHit, Chase
        }
        State state = State.Patrol;


        private void Start()
        {
            CurrentHealth = MaxHealth;

            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            playerMask = LayerMask.GetMask("Player");
            groundDetector = transform.Find("GroundDetector");
            groundMask = LayerMask.GetMask("Ground");

            //cho phép player và entity đi xuyên qua nhau
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Entity"));
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Entity"), LayerMask.NameToLayer("Entity"));
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log(state);
            if (idleTimer > 0)
            {
                // giảm thời gian ở trạng thái idle còn lại
                idleTimer -= Time.deltaTime;
            }
            LookingForPlayer();
            FacingDirectionUpdate();
            animator.Play(currentAnimation);
        }

        private void FixedUpdate()
        {
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
                case State.Dead:
                    currentAnimation = "Dead";
                    break;
            }
        }

        void Idle()
        {
            // nếu hết thời gian ở trạng thái idle thì chuyển sang trạng thái walk
            if (idleTimer <= 0)
            {
                state = State.Patrol;
                moveDirection = -moveDirection;
            }

            if (playerDetector && !IsDeadEnd())
            {
                state = State.Chase;
                return;
            }
        }

        void Patrol()
        {
            //nếu đi quá xa
            if (Vector2.Distance(transform.position, PatrolPoint.transform.position) >= PatrolDistance)
            {
                if (!IsDirectToPatrolPoint())
                {
                    idleTimer = idleTime;
                    state = State.Idle;
                    return;
                }
            }

            //nếu phát hiện player 
            if (playerDetector)
            {
                state = State.Chase;
                return;
            }

            //Movement
            if (!IsDeadEnd())
            {
                rb.velocity = new Vector2(Speed * moveDirection, rb.velocity.y);
            }
        }

        void Chase()
        {
            //nếu không phát hiện player nữa hoặc cụt đường thì chuyển sang trạng thái idle
            if (!playerDetector)
            {
                state = State.Idle;
                idleTimer = idleTime;
                return;
            }

            //cập nhật hướng di chuyển tới nhân vật
            _ = (playerDetector.transform.position.x - transform.position.x) > 0 ? moveDirection = 1 : moveDirection = -1;

            //Movement
            if (!IsDeadEnd())
            {
                rb.velocity = new Vector2(Speed * moveDirection, rb.velocity.y);
            }
        }

        //Được gọi bởi Player
        //Packege[0] là lượng dame, Package[1] là hướng bị đánh
        public void TakeDamage(object[] package)
        {
            if (state == State.Dead)
            {
                return;
            }

            state = State.TakeHit;
            if (!playerDetector)
            {
                moveDirection = -moveDirection;
            }

            // trừ máu
            CurrentHealth -= Convert.ToSingle(package[0]);
            if (CurrentHealth <= 0)
            {
                state = State.Dead;
            }
        }

        // được gọi bởi Attack animation và TakeHit animaiton sau khi nó kết thúc hoạt ảnh
        void BackToChase()
        {
            state = State.Chase;
        }

        void LookingForPlayer()
        {
            playerDetector = Physics2D.Raycast(transform.position, Vector2.right * moveDirection, PlayerDetectRange, playerMask);
        }

        //cập nhật hướng quay mặt
        void FacingDirectionUpdate()
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * moveDirection, transform.localScale.y, transform.localScale.z);
        }

        bool IsDirectToPatrolPoint()
        {
            if (transform.position.x < PatrolPoint.transform.position.x)
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

        // kiểm  tra có tới đường cụt?
        // đường cụt là khi phía trước gặp vực
        bool IsDeadEnd()
        {
            //Physics2D.Raycast() sẽ trả giá trị true nếu nó va chạm với groundMask
            //groundDetector.transform.position là gốc của tia Raycast
            //Vector2.down là hướng bắn của tia
            //56.0f là độ dài của tia
            var _collider = Physics2D.Raycast(groundDetector.transform.position, Vector2.down, 56f, groundMask);
            return !_collider;
        }

        //private void OnDrawGizmos()
        //{
        //    Gizmos.DrawLine(groundDetector.transform.position, groundDetector.transform.position + Vector3.down * 56f);
        //    var temp = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        //    Gizmos.DrawLine(temp, temp + Vector3.right * PlayerDetectRange * moveDirection);
        //}
    }
}
