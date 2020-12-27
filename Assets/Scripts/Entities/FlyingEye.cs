using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEye : MonoBehaviour
{
    public float Speed = 0.8f;
    private Vector3 moveDirection = Vector2.right;          //hướng di chuyển

    public float FallingSpeed = 3f;

    private int KnockbackDirection = 1;
    public float KnockBackSpeed = 8;          //tốc độ bị đẩy lùi
    public float KnockBackDistance = 50;          //khoảng cách bị đẩy lùi
    private float KnockbackDistanceRemain;          //khoảng cách còn lại để bị đẩy lùi

    public float MaxHealth = 100;
    public float CurrentHealth;

    Collider2D playerDetector = null;
    public float PlayerDetectRadius = 50f;

    public GameObject PatrolPoint;      // vị trí để đi tuần tra
    public float PatrolDistance = 50f;      // khoảng cách giới hạn để đi tuần tra


    public GameObject Projectile;

    private BoxCollider2D hitbox;
    private LayerMask playerMask;
    private Animator animator;
    private string currentAnimation = "Flying";

    enum State { Move, Knockback, Falling, Dead,
        Attack
    }
    State state = State.Move;
    private LayerMask groundMask;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
        playerMask = LayerMask.GetMask("Player");
        groundMask = LayerMask.GetMask("Ground");
        hitbox = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

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
        PlayAnimation();
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Move:
                currentAnimation = "Flying";
                Move();
                break;
            case State.Knockback:
                currentAnimation = "GetHit";
                Knockback();
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
            state = State.Attack;
            return;
        }

        //thay đổi hướng di chuyển nếu đi quá xa khỏi điểm tuần tra
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
            state = State.Falling;
        }
    }

    void Knockback()
    {
        var velocity = new Vector3(KnockBackSpeed * KnockbackDirection, 0, 0);
        transform.position += velocity;
        KnockbackDistanceRemain -= Mathf.Abs(velocity.x);

        if (KnockbackDistanceRemain <= 0)
        {
            state = State.Move;
        }
    }

    void LookingForPlayer()
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
    void Attack()
    {
        //nếu không phát hiện player nữa thì chuyển sang trạng thái tuần tra
        if (playerDetector == null)
        {
            state = State.Move;
            return;
        }
        //cập nhật hướng di chuyển tới nhân vật
        moveDirection = (playerDetector.transform.position - transform.position).normalized;

        GameObject p = Instantiate(Projectile) as GameObject;
        p.transform.position = transform.position;
        p.SendMessage("SetDirection", moveDirection);
    }

    void Falling()
    {
        if (IsHitGround())
        {
            state = State.Dead;
            return;
        }

        transform.position += Vector3.down * FallingSpeed;
    }

    void Dead()
    {
        var b = gameObject.GetComponent<BoxCollider2D>();
        b.enabled = false;
    }

    // được gọi vào cuối animation Falling
    bool IsHitGround()
    {
        var collider = Physics2D.Raycast(transform.position, Vector2.down, 25f, groundMask);
        return collider;
    }

    void PlayAnimation()
    {
        animator.Play(currentAnimation);
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - 25));
    //}
}
