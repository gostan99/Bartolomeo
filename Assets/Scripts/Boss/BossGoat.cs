using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGoat : MonoBehaviour
{
    private enum State { Idle, Walk, Turnaround, Attack1, Attack2, Attack3, SmashToIdle, Dead }
    State state = State.Idle;

    public float Speed = 80f;
    private float moveDirection = 1;        //hướng di chuyển
    public Transform ToPLayerDirection;

    public float MaxHealth = 100;
    public float CurrentHealth;
    public float AttackDamage = 10;
    
    Collider2D playerDetector =null;

    public GameObject PatrolPoint;      // vị trí để đi tuần tra
    public float PatrolDistance = 400f;      // khoảng cách giới hạn để đi tuần tra

    public float PlayerDetectRange = 1000f; // tầm phát hiện player

    Animator animator;
    string currentAnimation = "Idle_Animation";

    Rigidbody2D rb;

    LayerMask playerMask;

    float idleTime = 2f; // thời gian ở trạng thái idle là 2s
    float idleTimer; // đếm thời gian ở trạng thái idle còn lại

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerMask = LayerMask.GetMask("Player");

        //cho phép player và entity đi xuyên qua nhau
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Entity"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Entity"), LayerMask.NameToLayer("Entity"));
    }

    void Update()
    {
        if (idleTimer > 0)
        {
            idleTimer -= Time.deltaTime;
        }
        FacingDirectionUpdate();
        animator.Play(currentAnimation);
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Idle:
                currentAnimation = "Idle_Animation";
                Idle();
                break;
            case State.Turnaround:
                currentAnimation = "Tunrnaround_Animation";
                Turnaround();
                break;
            case State.Walk:
                currentAnimation = "Walk_Animation";
                Walk();
                break;
            case State.Dead:
                currentAnimation = "Dead_Animation";
                break;
        }
    }

    void Idle()
    {
        moveDirection = -moveDirection;
        if (!IsDirectToPatrolPoint())
        {
            idleTimer = idleTime;
            state = State.Turnaround;
            return;
        }

    }

    void Walk()
    {

        //thay đổi hướng di chuyển nếu đi quá xa khỏi điểm tuần tra
        if (Vector2.Distance(transform.position, PatrolPoint.transform.position) >= PatrolDistance)
        {

            if (!IsDirectToPatrolPoint())
            {
                idleTimer = idleTime;
                state = State.Idle;
                return;
            }
        }
        if (!IsInAir())
        {
            rb.velocity = new Vector2(Speed * moveDirection, rb.velocity.y);
        }
    }

    void Turnaround()
    {
        if (idleTimer <= 0)
        {
            state = State.Walk;
            moveDirection = -moveDirection;
        }
    }


    bool IsInAir()
    {
        return rb.velocity.y != 0f;
    }

    bool PlayerIsDetected()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.right * moveDirection, PlayerDetectRange, playerMask);
        return hit;
    }

    //cập nhật hướng quay mặt
    void FacingDirectionUpdate()
    {
        if (moveDirection > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
        }
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
}
