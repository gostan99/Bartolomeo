using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed = 5f;
    public float Size = 8.5f;
    private Vector3 StartPos;
    public float KhoangCachBayToiDa = 300f;
    public Vector3 Direction = Vector3.right;

    private LayerMask playerMask;
    private Animator animator;
    private string currentAnimation = "Move";

    private enum State { Moving, Explosive }
    State state = State.Moving;


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
            case State.Moving:
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
        if (Vector3.Distance(transform.position, StartPos) >= KhoangCachBayToiDa || IsCollidedWithPlayer())
        {
            state = State.Explosive;
        }
    }

    public void SetDirection(Vector3 dir)
    {
        Direction = dir;
    }

    void Destroy()
    {
        Destroy(this.gameObject);
    }

    bool IsCollidedWithPlayer()
    {
        var collider = Physics2D.OverlapCircle(transform.position, Size, playerMask);
        return collider;
    }
}
