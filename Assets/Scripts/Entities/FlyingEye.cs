using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEye : MonoBehaviour
{
    public float Speed = 0.8f;
    private Vector3 moveDirection = Vector2.right;          //hướng di chuyển

    public float KhoangCachVoiPlayer = 100f; //giữ khoảng cách tới Player

    private float fallingSpeed = 3f;
    private int KnockbackDirection = 1;
    public float KnockBackSpeed = 8;          //tốc độ bị đẩy lùi
    public float KnockBackDistance = 50;          //khoảng cách bị đẩy lùi
    private float KnockbackDistanceRemain;          //khoảng cách còn lại để bị đẩy lùi

    public float MaxHealth = 100;
    public float CurrentHealth;
    public float AttackDamage = 10;

    Collider2D playerIsDetected = null;

    public GameObject PatrolPoint;      // vị trí để giơi đi tuần tra
    public float PatrolDistance = 50f;      // khoảng cách giới hạn để giơi đi tuần tra

    public float PlayerDetectorRadius = 50f;

    public GameObject Projectile;

    private BoxCollider2D Hitbox;
    private LayerMask PlayerMask;
    private Animator animator;
    private string currentAnimation = "Flying";

    enum State { Patrol, Knockback, Falling, Dead,
        Attack
    }
    State state = State.Patrol;
    private LayerMask groundMask;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
        PlayerMask = LayerMask.GetMask("Player");
        groundMask = LayerMask.GetMask("Ground");
        Hitbox = GetComponent<BoxCollider2D>();
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
            case State.Patrol:
                currentAnimation = "Flying";
                Patrol();
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

    //Đi tuần tra
    void Patrol()
    {
        if (playerIsDetected)
        {
            state = State.Attack;
            return;
        }

        //thay đổi hướng di chuyển nếu con dơi đi quá xa
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
            state = State.Patrol;
        }
    }

    void LookingForPlayer()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, PlayerDetectorRadius, PlayerMask);
        if (collider)
        {
            playerIsDetected = collider;
        }
        else
        {
            playerIsDetected = null;
        }
    }

    //Được gọi trong animation Event
    void Attack()
    {
        //nếu không phát hiện player nữa thì chuyển sang trạng thái tuần tra
        if (playerIsDetected == null)
        {
            state = State.Patrol;
            return;
        }
        //cập nhật hướng di chuyển tới nhân vật
        moveDirection = (playerIsDetected.transform.position - transform.position).normalized;

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

        transform.position += Vector3.down * fallingSpeed;
    }

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
