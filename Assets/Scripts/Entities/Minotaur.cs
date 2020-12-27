using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minotaur : MonoBehaviour
{
    private enum State { Idle, Walk, Attack, Dead }
    State state = State.Walk;

    public float Speed = 150f;
    private int moveDirection = 1;          //hướng di chuyển
    public int KnockBackForce = 150;          //lực bị đẩy lùi
    public float MaxHealth = 100;
    public float CurrentHealth;
    public float AttackDamage = 10;

    public GameObject PatrolPoint;      // vị trí để đi tuần tra
    public float PatrolDistance = 50f;      // khoảng cách giới hạn để đi tuần tra

    public float PlayerDetectRange = 26f; // tầm phát hiện player
    public Vector3 AttackOffSet = new Vector3(16, 12, 0); // vị trí đánh
    public Vector2 AttackRange = new Vector2(44, 56); // tầm đánh

    Animator animator;
    string currentAnimation = "Walk";

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

    // Update is called once per frame
    void Update()
    {
        if (idleTimer > 0)
        {
            // giảm thời gian ở trạng thái idle còn lại
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
                currentAnimation = "Idle";
                Idle();
                break;
            case State.Walk:
                currentAnimation = "Walk";
                Walk();
                break;
            case State.Attack:
                currentAnimation = "Attack";
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
            state = State.Walk;
            moveDirection = -moveDirection;
        }

        if (PlayerIsDetected())
        {
            state = State.Attack;
            return;
        }
    }

    void Walk()
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

        if (PlayerIsDetected())
        {
            state = State.Attack;
            return;
        }

        //Movement
        if (!IsInAir())
        {
            rb.velocity = new Vector2(Speed * moveDirection, rb.velocity.y);
        }
    }

    // được gọi bơi Attack animation
    void Attack()
    {
        Collider2D hit = Physics2D.OverlapCapsule(transform.position + AttackOffSet, AttackRange, CapsuleDirection2D.Vertical, 0, playerMask);
        if (hit)
        {
            Debug.Log("Á hự");
        }
    }

    // được gọi bởi Attack animation sau khi nó kết thúc hoạt ảnh attack
    void BackToWalk()
    {
        state = State.Walk;
    }

    bool PlayerIsDetected()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.right * moveDirection, PlayerDetectRange, playerMask);
        return hit;
    }

    bool IsInAir()
    {
        return rb.velocity.y != 0f;
    }

    // Được gọi bơi player
    public void GetHit(object[] package)
    {
        if (state == State.Dead)
        {
            return;
        }

        // trừ máu
        CurrentHealth -= Convert.ToSingle(package[0]);
        // bị đẩy lùi
        Knockback(Convert.ToInt32(package[1]));
        if (CurrentHealth <= 0)
        {
            state = State.Dead;
        }
    }

    void Knockback(int hitDirection)
    {
        rb.velocity = new Vector2(KnockBackForce * hitDirection, KnockBackForce);
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
}
