using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGoat : MonoBehaviour
{
    private enum State { Idle, Walk, Turnaround, Slash, Smash, Stomp, SmashToIdle, Dead }
    State state = State.Walk;

    public float Speed = 80f;
    private float moveDirection = 1;        //hướng di chuyển
    public Transform Player;

    public float MaxHealth = 100;
    public float CurrentHealth;
    public float AttackDamage = 10;

    public Vector2 SlashOffSet = new Vector2(95, 70);
    public Vector2 SlashSize = new Vector2(200, 54);
    public float SlashAngle = -40;

    Collider2D playerDetector =null;

    public GameObject PatrolPoint;      // vị trí để đi tuần tra
    public float PatrolDistance = 400f;      // khoảng cách giới hạn để đi tuần tra


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
        switch (state)
        {
            case State.Idle:
                currentAnimation = "Idle_Animation";
                Idle();
                break;
            case State.Turnaround:
                currentAnimation = "Tunrnaround_Animation";
                //Turnaround(); 
                break;
            case State.Walk:
                currentAnimation = "Walk_Animation";
                Walk();
                break;
            case State.Slash:
                currentAnimation = "Slash_Animation";
                break;
            //case State.Dead:
            //    currentAnimation = "Dead_Animation";
            //    break;
        }
        FacingDirectionUpdate();
        animator.Play(currentAnimation);
    }

    private void FixedUpdate()
    {
        if (state == State.Walk)
        {
            ApplyMovement();
        }
    }

    void Idle()
    {
        idleTimer -= Time.deltaTime;
        if (idleTimer<=0)
        {
            if (IsDirectToPlayer())
            {
                state = State.Walk;
            }
            else
            {
                state = State.Turnaround;

            }
        }
    }

    void ChangeToWalk()
    {
            state = State.Walk;
    }

    void ChangeToIdle()
    {
        idleTimer = idleTime;
        state = State.Idle;
    }

    void Walk()
    {
        if(!IsDirectToPlayer())
        {
            state = State.Turnaround;
            return;
        }
        if (PlayerIsInAttackRange())
        {
            state = State.Slash;
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

    void ApplyMovement()
    {
        rb.velocity = new Vector2(Speed * moveDirection, rb.velocity.y);
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


    bool IsDirectToPlayer()
    {
        if(transform.position.x > Player.position.x)
        {
            if (moveDirection < 0)
                return true;
            else
                return false;
        }
        else
        {
            if (moveDirection > 0)
                return true;
            else
                return false;
        }
    } 

    bool PlayerIsInAttackRange()
    {
        Vector3 AttackPos = new Vector3((transform.position.x + SlashOffSet.x) * moveDirection, transform.position.y + SlashOffSet.y,transform.position.z);
        Collider2D hit = Physics2D.OverlapCapsule(AttackPos, SlashSize, CapsuleDirection2D.Horizontal, SlashAngle, playerMask);
        return hit;
    }
}

