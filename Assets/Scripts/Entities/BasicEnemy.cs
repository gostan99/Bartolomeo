using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public float Speed = 150f;
    private int moveDirection = 1;          //hướng di chuyển
    public int KnockBackForce = 150;          //hướng di chuyển
    private Rigidbody2D rb;
    private Transform groundDetector;     //Dùng để làm vị trí gốc cho Raycast
    private int groundMask;               //Ground layer
    public float MaxHealth = 100;
    public float CurrentHealth;
    public float AttackDamage = 10;

    private BoxCollider2D Hitbox;
    private LayerMask PlayerMask;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;

        groundDetector = transform.Find("GroundDetector");
        groundMask = LayerMask.GetMask("Ground");
        PlayerMask = LayerMask.GetMask("Player");

        rb = GetComponent<Rigidbody2D>();
        Hitbox = GetComponent<BoxCollider2D>();

        //cho phép player và entity đi xuyên qua nhau
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Entity"));
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
        Attack();
        MoveDirectionUpdate();
        FacingDirectionUpdate();
    }

    void MoveDirectionUpdate()
    {
        //Physics2D.Raycast() sẽ trả giá trị true nếu nó va chạm với groundMask
        //groundDetector.transform.position là gốc của tia Raycast
        //Vector2.down là hướng bắn của tia
        //50.0f là độ dài của tia
        var collided = Physics2D.Raycast(groundDetector.transform.position, Vector2.down, 46.0f, groundMask);
        if (!collided && !IsInAir())
        {
            moveDirection *= -1;
        }
    }

    //cập nhật hướng quay mặt
    void FacingDirectionUpdate()
    {
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * moveDirection, transform.localScale.y, transform.localScale.z);
    }

    //di chuyển
    void Patrol()
    {
        if (!IsInAir())
        {
            rb.velocity = new Vector2(Speed * moveDirection, rb.velocity.y);
        }
    }

    bool IsInAir()
    {
        return rb.velocity.y != 0f;
    }

    public void GetHit(object[] package)
    {
        CurrentHealth -= Convert.ToSingle(package[0]);
        ApplyKnockback(Convert.ToInt32(package[1]));
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        gameObject.SetActive(false);
    }

    void ApplyKnockback(int hitDirection)
    {
        rb.velocity= new Vector2(KnockBackForce * hitDirection, KnockBackForce);
    }

    void Attack()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position,Hitbox.size,0,PlayerMask);
        if (hit)
        {
            Debug.Log("Á hự");
        }
    }
}
