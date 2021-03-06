﻿using Assets.Scripts.Entities;
using Assets.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEye : MonoBehaviour
{
    public EnemyData eData;
    public float Speed = 0.8f;
    private Vector3 moveDirection = Vector2.right;          //hướng di chuyển

    public float FallingSpeed = 4f;

    private Collider2D playerDetector = null;
    public float PlayerDetectRadius = 50f;

    public GameObject PatrolPoint;          // vị trí để đi tuần tra
    public float PatrolDistance = 50f;      // khoảng cách giới hạn để đi tuần tra

    public GameObject Projectile;

    private LayerMask playerMask;
    private Animator animator;
    private string currentAnimation = "Flying";

    private enum State
    {
        Move, TakeHit, Falling, Dead,
        Attack
    }

    private State state = State.Move;
    private LayerMask groundMask;

    private Canvas healthCanvas;

    // Start is called before the first frame update
    private void Start()
    {
        eData = GetComponent<EnemyData>();
        playerMask = LayerMask.GetMask("Player");
        groundMask = LayerMask.GetMask("Ground");
        animator = GetComponent<Animator>();

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
        // dò tìm Player
        LookingForPlayer();
        // cập nhật hướng quay mặt
        FacingDirectionUpdate();
        animator.Play(currentAnimation);

        if (eData.CurrentHealth <= 0)
        {
            healthCanvas.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Move:
                currentAnimation = "Flying";
                Move();
                break;

            case State.TakeHit:
                currentAnimation = "TakeHit";
                break;

            case State.Falling:
                currentAnimation = "Falling";
                Falling();
                break;

            case State.Dead:
                currentAnimation = "Dead";
                Dead();
                break;

            case State.Attack:
                currentAnimation = "Attack";
                break;
        }
    }

    //cập nhật hướng quay mặt
    private void FacingDirectionUpdate()
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

    private void Move()
    {
        if (playerDetector)
        {
            state = State.Attack;
            return;
        }

        //thay đổi hướng di chuyển nếu đi quá xa khỏi điểm tuần tra
        if (Vector2.Distance(transform.position, PatrolPoint.transform.position) > PatrolDistance)
        {
            moveDirection = (PatrolPoint.transform.position - transform.position).normalized;
        }

        transform.position += new Vector3(moveDirection.x, moveDirection.y, 0) * Speed;
    }

    //Được gọi bởi Player
    //Packege[0] là lượng dame, Package[1] là hướng bị đánh
    public void TakeDamage(object[] package)
    {
        if (eData.CurrentHealth <= 0)
        {
            return;
        }
        //trừ máu
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
            state = State.Falling;
            Dead();
            return;
        }
        if (!healthCanvas.isActiveAndEnabled)
        {
            healthCanvas.enabled = true;
        }

        //chuyển sang trạng thái bị đánh
        state = State.TakeHit;
    }

    private void LookingForPlayer()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, PlayerDetectRadius, playerMask);
        if (collider)
        {
            playerDetector = collider;
        }
        else
        {
            playerDetector = null;
        }
    }

    //Được gọi trong animation Attack
    private void Attack()
    {
        if (eData.CurrentHealth <= 0)
        {
            return;
        }
        //nếu không phát hiện player nữa thì chuyển sang trạng thái tuần tra
        if (playerDetector == null)
        {
            state = State.Move;
            return;
        }
        //cập nhật hướng di chuyển tới nhân vật
        moveDirection = (playerDetector.transform.position - transform.position).normalized;

        // bắn quả cầu---------------------
        GameObject p = Instantiate(Projectile) as GameObject;   //khởi tạo
        p.transform.position = transform.position;              //đặt vị trí của quả cầu
        p.SendMessage("SetDirection", moveDirection);           //đặt hướng di chuyển của quả cầu
    }

    // được gọi bởi TakeHit animaiton sau khi nó kết thúc hoạt ảnh
    private void BackToMove()
    {
        state = State.Move;
    }

    private void Falling()
    {
        if (IsHitGround())
        {
            state = State.Dead;
            return;
        }

        transform.position += Vector3.down * FallingSpeed;
    }

    private void Dead()
    {
        Destroy(this.gameObject, 5);
    }

    // được gọi vào cuối animation Falling
    private bool IsHitGround()
    {
        var collider = Physics2D.Raycast(transform.position, Vector2.down, 25f, groundMask);
        return collider;
    }
}