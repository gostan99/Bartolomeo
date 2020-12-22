using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public float Speed = 150f;
    private float direction = 1;          //hướng di chuyển
    private Rigidbody2D rb;
    private Transform groundDetector;     //Dùng để làm vị trí gốc cho Raycast
    private int groundMask;               //Ground layer

    // Start is called before the first frame update
    void Start()
    {
        groundDetector = transform.Find("GroundDetector");
        groundMask = LayerMask.GetMask("Ground");
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
        DirectionUpdate();
        FacingDirectionUpdate();
    }

    void DirectionUpdate()
    {
        if (!IsGrounded() && !IsFalling())
        {
            direction *= -1;
        }
    }

    //cập nhật hướng quay mặt
    void FacingDirectionUpdate()
    {
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);
    }

    //di chuyển
    void Patrol()
    {
        if (IsGrounded())
        {
            rb.velocity = new Vector2(Speed * direction, rb.velocity.y);
        }
    }

    bool IsGrounded()
    {
        //Physics2D.Raycast() sẽ trả giá trị true nếu nó va chạm với groundMask
        //groundDetector.transform.position là gốc của tia Raycast
        //Vector2.down là hướng bắn của tia
        //50.0f là độ dài của tia
        var collided = Physics2D.Raycast(groundDetector.transform.position, Vector2.down, 46.0f, groundMask);
        return collided;
    }

    bool IsFalling()
    {
        return rb.velocity.y < 0f;
    }
}
