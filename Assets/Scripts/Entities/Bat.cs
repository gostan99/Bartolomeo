using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour
{
    public float Speed = 4;
    private Vector3 moveDirection = Vector2.right;          //hướng di chuyển
    private int KnockbackDirection = 1;
    public float KnockBackSpeed = 7;          //tốc độ bị đẩy lùi
    public float KnockBackDistance = 50;          //khoảng cách bị đẩy lùi
    private float KnockbackDistanceRemain;          //khoảng cách còn lại để bị đẩy lùi

    public float MaxHealth = 100;
    public float CurrentHealth;
    public float AttackDamage = 10;

    Collider2D playerDetector = null;
    public float PlayerDetectRadius = 50f;

    public GameObject PatrolPoint;      // vị trí để đi tuần tra
    public float PatrolDistance = 50f;      // khoảng cách giới hạn để đi tuần tra


    private BoxCollider2D Hitbox;
    private LayerMask PlayerMask;

    enum State { Patrol,Chase, Knockback}
    State state = State.Patrol;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
        PlayerMask = LayerMask.GetMask("Player");
        Hitbox = GetComponent<BoxCollider2D>();

        //cho phép player và entity đi xuyên qua nhau
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Entity"));
    }

    // Update is called once per frame
    void Update()
    {
        // dò tìm Player
        LookingForPlayer();
        // cập nhật hướng quay mặt
        FacingDirectionUpdate();
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Patrol:
                Move();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Knockback:
                Knockback();
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

        //thay đổi hướng di chuyển nếu con đi quá xa khỏi điểm tuần tra
        if (Vector2.Distance(transform.position, PatrolPoint.transform.position) > PatrolDistance)
        {
            //Vector AB = B - A
            moveDirection = (PatrolPoint.transform.position - transform.position).normalized;
        }

        transform.position += moveDirection * Speed;
    }

    //Packege[0] là lượng dame, Package[1] là hướng bị đánh
    public void GetHit(object[] package)
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
            Die();
        }
    }

    void Die()
    {
        gameObject.SetActive(false);
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

        transform.position += new Vector3(moveDirection.x, moveDirection.y, transform.position.z) * (Speed + 2);
        //Attack nếu chạm được vào người của player
        DealDamage();
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

    void DealDamage()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position,Hitbox.size,0,PlayerMask);
        if (hit)
        {
            Debug.Log("Á hự");
        }
    }
}
