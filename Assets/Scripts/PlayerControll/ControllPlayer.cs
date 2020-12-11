using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllPlayer : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public GameObject Player;
    float dirX, moveSpeed = 5f;
    int healthPoints = 3;
    bool isHurting, isDead;
    bool facingRight = true;
    Vector3 localScale;
    // Start is called before the first frame update
    void Start()
    {
        rb = Player.GetComponent<Rigidbody2D>();
        anim = Player.GetComponent<Animator>();
        localScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && !isDead && rb.velocity.y == 0)
            rb.AddForce(Vector2.up * 600f);

        if (Input.GetKey(KeyCode.LeftShift))
            moveSpeed = 10f;
        else
            moveSpeed = 5f;

        SetAnimationState();

        if (!isDead)
            dirX = Input.GetAxisRaw("Horizontal") * moveSpeed;
    }
    //void FixedUpdate()
    //{
    //    if (!isHurting)
    //        rb.velocity = new Vector2(dirX, rb.velocity.y);
    //}

    //void LateUpdate()
    //{
    //    CheckWhereToFace();
    //}

    void SetAnimationState()
    {
        if (dirX == 0)
        {
            anim.SetBool("Horizontal", true);
            anim.SetBool("isRunning", false);
        }

        if (rb.velocity.y == 0)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", false);
        }

        if (Mathf.Abs(dirX) == 5 && rb.velocity.y == 0)
            anim.SetBool("Horizontal", true);

        if (Mathf.Abs(dirX) == 10 && rb.velocity.y == 0)
            anim.SetBool("isRunning", true);
        else
            anim.SetBool("isRunning", false);

        if (Input.GetKey(KeyCode.DownArrow) && Mathf.Abs(dirX) == 10)
            anim.SetBool("isDash", true);
        else
            anim.SetBool("isDash", false);

        if (rb.velocity.y > 0)
            anim.SetBool("isJumping", true);

        if (rb.velocity.y < 0)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", true);
        }
    }

    void CheckWhereToFace()
    {
        if (dirX > 0)
            facingRight = true;
        else if (dirX < 0)
            facingRight = false;

        if (((facingRight) && (localScale.x < 0)) || ((!facingRight) && (localScale.x > 0)))
            localScale.x *= -1;

        transform.localScale = localScale;

    }
}
