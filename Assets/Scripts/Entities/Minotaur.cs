using Assets.Scripts.Entities;
using Assets.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minotaur : MonoBehaviour
{
    EnemyData eData;

    private enum State { Idle, Patrol, Attack, Dead,
        TakeHit
    }
    State state = State.Patrol;

    public float Speed = 150f;
    private int moveDirection = 1;          //hướng di chuyển
    public int KnockBackForce = 150;        //lực bị đẩy lùi
    public float AttackDamage = 3;

    private Transform groundDetector;           //Dùng để làm vị trí gốc cho Raycast kiểm tra va chạm với mặt đất
    public float groundDetectorLength = 22f;    //Độ dài tia Raycast
    private int groundMask;                     //Ground layer
    private int wallMask;                       //Wall layer
    public float wallDetectorLength = 16f;      //Độ dài tia Raycast

    public GameObject PatrolPoint;              // vị trí để đi tuần tra
    public float PatrolDistance = 50f;          // khoảng cách giới hạn để đi tuần tra

    public float PlayerDetectRange = 26f;       // tầm phát hiện player
    public Vector3 AttackOffSet = new Vector3(16, 12, 0); // vị trí đánh
    public Vector2 AttackRange = new Vector2(44, 56);     // tầm đánh

    Animator animator;
    string currentAnimation = "Patrol";

    Rigidbody2D rb;

    LayerMask playerMask;

    public float idleTime = 2f; // thời gian ở trạng thái idle là 2s
    float idleTimer;            // đếm thời gian ở trạng thái idle còn lại

    private Canvas healthCanvas;

    // Start is called before the first frame update
    void Start()
    {
        eData = GetComponent<EnemyData>();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerMask = LayerMask.GetMask("Player");
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
    void Update()
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
            case State.Attack:
                currentAnimation = "Attack";
                break;
            case State.TakeHit:
                currentAnimation = "TakeHit";
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
        if (state == State.Patrol && !IsInAir())
        {
            ApplyMovement();
        }
    }

    void Idle()
    {
        // giảm thời gian ở trạng thái idle còn lại
        idleTimer -= Time.deltaTime;

        // nếu hết thời gian ở trạng thái idle thì chuyển sang trạng thái walk
        if (idleTimer <= 0)
        {
            state = State.Patrol;
            moveDirection = -moveDirection;
        }

        if (PlayerIsInAttackRange())
        {
            state = State.Attack;
            return;
        }
    }

    void Patrol()
    {
        if (Vector2.Distance(transform.position, PatrolPoint.transform.position) >= PatrolDistance)
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

        if (PlayerIsInAttackRange())
        {
            state = State.Attack;
            return;
        }
    }

    // được gọi bơi Attack animation
    void Attack()
    {
        Collider2D hit = Physics2D.OverlapCapsule(transform.position + AttackOffSet, AttackRange, CapsuleDirection2D.Vertical, 0, playerMask);
        if (hit)
        {
            object[] package = new object[1];
            package[0] = AttackDamage;
            hit.SendMessage("TakeDamage", package);
        }
    }

    // được gọi bởi Attack animation và TakeHit animaiton sau khi nó kết thúc hoạt ảnh
    void BackToPatrol()
    {
        state = State.Patrol;
    }

    bool PlayerIsInAttackRange()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.right * moveDirection, PlayerDetectRange, playerMask);
        return hit;
    }

    bool IsInAir()
    {
        return rb.velocity.y != 0f;
    }

    //Được gọi bởi Player
    //Packege[0] là lượng dame, Package[1] là hướng bị đánh
    public void TakeDamage(object[] package)
    {
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
        // bị đẩy lùi
        ApplyKnockback(Convert.ToInt32(package[1]));
        if (eData.CurrentHealth <= 0)
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
            state = State.Dead;
            var bc = GetComponent<BoxCollider2D>();
            bc.enabled = false;
        }
    }

    void ApplyMovement()
    {
        rb.velocity = new Vector2(Speed * moveDirection, rb.velocity.y);
    }

    void ApplyKnockback(int hitDirection)
    {
        rb.velocity = new Vector2(KnockBackForce * hitDirection, KnockBackForce);
    }

    //cập nhật hướng quay mặt
    void FacingDirectionUpdate()
    {
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * moveDirection, transform.localScale.y, transform.localScale.z);
    }

    //moveDiection có đang hướng về PatrolPoint?
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
    // đường cụt là khi phía trước gặp vực hoặc tường
    bool IsDeadEnd()
    {
        //Physics2D.Raycast() sẽ trả giá trị true nếu nó va chạm với groundMask
        //groundDetector.transform.position là gốc của tia Raycast
        //Vector2.down là hướng bắn của tia
        //groundDetectorLength, wallDetectorLength là độ dài của tia
        var groundCollided = Physics2D.Raycast(groundDetector.transform.position, Vector2.down, groundDetectorLength, groundMask);
        var wallCollided = Physics2D.Raycast(transform.position, Vector2.right * moveDirection, wallDetectorLength, wallMask);
        if (!groundCollided || wallCollided)
        {
            return true;
        }
        return false;
    }
}
