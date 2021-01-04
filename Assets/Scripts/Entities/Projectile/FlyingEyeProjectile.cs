using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEyeProjectile : MonoBehaviour
{
    public float Speed = 5f;
    public float ColliderSize = 8.5f;
    private Vector3 StartPos;                     //vị trí xuất hiện
    public float KhoangCachBayToiDa = 500f;
    private Vector3 Direction = Vector3.right;     //hướng bay
    public float AttackDamage = 10;

    private LayerMask playerMask;
    private Animator animator;
    private string currentAnimation = "Move";

    private enum State { Move, Explosive }
    State state = State.Move;


    // Start is called before the first frame update
    void Start()
    {
        StartPos = transform.position;

        playerMask = LayerMask.GetMask("Player");
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.Play(currentAnimation);
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Move:
                currentAnimation = "Move";
                Move();
                break;
            case State.Explosive:
                currentAnimation = "Explosive";
                break;
        }        
    }

    void Move()
    {
        transform.position += Direction * Speed;
        // bay quá giới hạn hoặc va chạm trúng player thì sẽ chuyển sang state phát nổ
        if (Vector3.Distance(transform.position, StartPos) >= KhoangCachBayToiDa || IsCollided())
        {
            state = State.Explosive;
        }
    }

    public void SetDirection(Vector3 dir)
    {
        Direction = dir;
    }

    //Được gọi vào cuối animation của Explosive animation
    void Destroy()
    {
        // tắt renderer
        var r = GetComponent<SpriteRenderer>();
        r.enabled = false;
        // hủy gameObject sau 2s để tránh lỗi null refference
        Destroy(this.gameObject, 2f);
    }


    //Được gọi vào đầu animation của Explosive animation
    void DealDamage()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, ColliderSize, playerMask);
        if (hit)
        {
            object[] package = new object[1];
            package[0] = AttackDamage;
            hit.SendMessage("TakeDamage", package);
        }
    }

    //Được gọi bởi Player
    //Packege[0] là lượng dame, Package[1] là hướng bị đánh
    public void TakeDamage(object[] package)
    {
        state = State.Explosive;
    }

    //kiểm tra va chạm với player
    bool IsCollided()
    {
        var _collider = Physics2D.OverlapCircle(transform.position, ColliderSize, playerMask);
        return _collider;
    }
}
