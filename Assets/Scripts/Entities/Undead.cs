﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undead : MonoBehaviour
{
    public float Speed = 2.5f;
    private Vector3 moveDirection = Vector2.right;          //hướng di chuyển
    private int KnockbackDirection = 1;                     //hướng bị bị đẩy lùi
    public float KnockBackSpeed = 8;                        //tốc độ bị đẩy lùi
    public float KnockBackDistance = 80;                    //khoảng cách bị đẩy lùi
    private float KnockbackDistanceRemain;                  //khoảng cách còn lại để bị đẩy lùi

    public float MaxHealth = 100;
    public float CurrentHealth;
    public float AttackDamage = 5;

    Collider2D playerDetector = null;
    public float PlayerDetectRadius = 160f;                 // bán kính phát hiện player

    public Vector3 AttackOffSet = new Vector3(15, 6, 0);    // vị trí đánh
    public float AttackRadius = 31f;                        // tầm đánh

    public GameObject PatrolPoint;                          // vị trí để đi tuần tra
    public float PatrolDistance = 100f;                     // khoảng cách giới hạn để đi tuần tra

    private LayerMask PlayerMask;

    enum State { Patrol, Chase, Knockback, Dead, Attack }
    State state = State.Patrol;

    Animator animator;
    string currentAnimation = "Idle";

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
        PlayerMask = LayerMask.GetMask("Player");
        animator = GetComponent<Animator>();

        //cho phép player và entity đi xuyên qua nhau
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Entity"));
        //cho phép entity đi xuyên qua nhau
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Entity"), LayerMask.NameToLayer("Entity"));
    }

    // Update is called once per frame
    void Update()
    {
        // dò tìm Player
        LookingForPlayer();
        // cập nhật hướng quay mặt
        FacingDirectionUpdate();
        // chạy hoạt ảnh
        animator.Play(currentAnimation);
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Patrol:
                currentAnimation = "Idle";
                Move();
                break;
            case State.Chase:
                currentAnimation = "Idle";
                Chase();
                break;
            case State.Attack:
                currentAnimation = "Attack";
                break;
            case State.Knockback:
                currentAnimation = "Idle";
                Knockback();
                break;
            case State.Dead:
                currentAnimation = "Dead";
                break;        
        }
    }

    //cập nhật hướng quay mặt
    void FacingDirectionUpdate()
    {
        if (moveDirection.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
        }
    }

    void Move()
    {
        if (playerDetector)
        {
            state = State.Chase;
            return;
        }

        //thay đổi hướng di chuyển nếu đi quá xa khỏi điểm tuần tra
        if (Vector2.Distance(transform.position, PatrolPoint.transform.position) > PatrolDistance)
        {
            moveDirection = (PatrolPoint.transform.position - transform.position).normalized;
        }

        transform.position += moveDirection * Speed;
    }

    //Được gọi bởi Player
    //Packege[0] là lượng dame, Package[1] là hướng bị đánh
    public void TakeDamage(object[] package)
    {
        //trừ máu
        CurrentHealth -= Convert.ToSingle(package[0]);

        //chuyển sang trạng thái bị đẩy lùi
        state = State.Knockback;
        KnockbackDistanceRemain = KnockBackDistance;
        //Gán hướng bị đẩy lùi
        KnockbackDirection = Convert.ToInt32(package[1]);

        if (CurrentHealth <= 0)
        {
            state = State.Dead;
        }
    }

    void Knockback()
    {
        var velocity = new Vector3(KnockBackSpeed * KnockbackDirection, 0, 0);
        transform.position += velocity;
        KnockbackDistanceRemain -= Mathf.Abs(velocity.x);

        if (KnockbackDistanceRemain <= 0)
        {
            state = State.Chase;
        }
    }

    void Chase()
    {
        //nếu không phát hiện player nữa thì chuyển sang trạng thái tuần tra
        if (playerDetector == null)
        {
            state = State.Patrol;
            return;
        }

        //cập nhật hướng di chuyển tới nhân vật
        moveDirection = (playerDetector.transform.position - transform.position).normalized;

        transform.position += new Vector3(moveDirection.x, moveDirection.y, transform.position.z) * (Speed + 3);

        if (CanAttack())
        {
            state = State.Attack;
        }
    }

    bool CanAttack()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position + AttackOffSet, AttackRadius, PlayerMask);
        return collider;
    }

    //Được gọi trong animation Attack
    void Attack()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position + AttackOffSet, AttackRadius, PlayerMask);
        if (hit)
        {
            object[] package = new object[1];
            package[0] = AttackDamage;
            hit.SendMessage("TakeDamage", package);
        }
    }

    //Được gọi cuối animation Attack
    void BackToChase()
    {
        state = State.Chase;
    }

    //Được gọi trong animation Dead
    void Die()
    {
        Destroy(this.gameObject);
    }

    void LookingForPlayer()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, PlayerDetectRadius, PlayerMask);
        if (collider)
        {
            playerDetector = collider;
        }
        else
        {
            playerDetector = null;
        }
    }
}
