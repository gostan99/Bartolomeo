using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Entities;
using Assets.Scripts.Player;

public class BasicEnemy : MonoBehaviour
{
    public EnemyData eData;
    private PlayerData pData;

    public float Speed = 150f;
    private int moveDirection = 1;            //hướng di chuyển
    public int KnockBackForce = 150;          //lực bị đẩy lùi
    private Rigidbody2D rb;
    private Transform groundDetector;         //Dùng để làm vị trí gốc cho Raycast
    public float groundDetectorLength = 48;   //Độ dài tia Raycast để check phía trước có mặt đất hay k
    private int groundMask;                   //Ground layer
    public float wallDetectorLength = 40f;    //Độ dài tia Raycast để check phía trước có tường hay k
    private int wallMask;                     //Wall layer
    public float AttackDamage = 3;

    private BoxCollider2D Hitbox;
    private LayerMask PlayerMask;             //Player player

    private Canvas healthCanvas;

    // Start is called before the first frame update
    private void Start()
    {
        eData = GetComponent<EnemyData>();
        pData = GetComponent<PlayerData>();

        groundDetector = transform.Find("GroundDetector");
        groundMask = LayerMask.GetMask("Ground");
        wallMask = LayerMask.GetMask("Wall");
        PlayerMask = LayerMask.GetMask("Player");

        rb = GetComponent<Rigidbody2D>();
        Hitbox = GetComponent<BoxCollider2D>();

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
        //cập nhật hướng di chuyển
        MoveDirectionUpdate();
        //cập nhật hướng nhìn theo hướng di chuyển
        FacingDirectionUpdate();

        if (eData.CurrentHealth <= 0)
        {
            healthCanvas.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void MoveDirectionUpdate()
    {
        // chuyển hướng nếu thấy đường cụt và không đang ở trên không trung
        if (IsDeadEnd() && !IsInAir())
        {
            moveDirection *= -1;
        }
    }

    //cập nhật hướng quay mặt
    private void FacingDirectionUpdate()
    {
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * moveDirection, transform.localScale.y, transform.localScale.z);
    }

    private void Move()
    {
        if (eData.CurrentHealth <= 0)
        {
            return;
        }
        if (!IsInAir())
        {
            rb.velocity = new Vector2(Speed * moveDirection, rb.velocity.y);
        }
        // nếu chạm phải player thì sẽ làm player mất máu
        DealDamage();
    }

    private bool IsInAir()
    {
        return rb.velocity.y != 0f;
    }

    //Được gọi bởi Player
    //Packege[0] là lượng dame, Package[1] là hướng bị đánh
    public void TakeDamage(object[] package)
    {
        if (eData.CurrentHealth <= 0)
        {
            return;
        }
        // trừ máu
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
            Die();
            return;
        }
        if (!healthCanvas.isActiveAndEnabled)
        {
            healthCanvas.enabled = true;
        }
        // bị đẩy lùi
        Knockback(Convert.ToInt32(package[1]));
    }

    private void Die()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(this.gameObject, 5);
    }

    private void Knockback(int hitDirection)
    {
        rb.velocity = new Vector2(KnockBackForce * hitDirection, KnockBackForce);
    }

    private void DealDamage()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, Hitbox.size, 0, PlayerMask);
        if (hit)
        {
            object[] package = new object[1];
            package[0] = AttackDamage;
            hit.SendMessage("TakeDamage", package);
        }
    }

    // kiểm  tra có tới đường cụt?
    // đường cụt là khi phía trước gặp vực hoặc tường
    private bool IsDeadEnd()
    {
        //groundDetector.transform.position là gốc của tia Raycast
        //Vector2.down là hướng bắn của tia
        //groundDetectorLength, wallDetectorLength là độ dài của tia
        var groundCollided = Physics2D.Raycast(groundDetector.transform.position, Vector2.down, groundDetectorLength, groundMask);
        var wallCollided = Physics2D.Raycast(transform.position, Vector2.right * moveDirection, wallDetectorLength, wallMask);
        if (!groundCollided || wallCollided)
        {
            return true;
        }
        return false;
    }
}