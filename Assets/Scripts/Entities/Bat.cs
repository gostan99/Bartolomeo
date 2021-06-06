using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Entities;
using Assets.Scripts.Player;

public class Bat : MonoBehaviour
{
    public EnemyData eData;

    public float Speed = 4;
    private Vector3 moveDirection = Vector2.right;  //hướng di chuyển
    private int KnockbackDirection = 1;             //hướng bị đẩy lùi
    public float KnockBackSpeed = 7;                //tốc độ bị đẩy lùi
    public float KnockBackDistance = 50;            //khoảng cách bị đẩy lùi
    private float KnockbackDistanceRemain;          //khoảng cách còn lại để bị đẩy lùi

    public float AttackDamage = 3;

    private Collider2D playerDetector = null;
    public float PlayerDetectRadius = 50f;

    public GameObject PatrolPoint;          // vị trí để đi tuần tra
    public float PatrolDistance = 50f;      // khoảng cách giới hạn để đi tuần tra

    private BoxCollider2D Hitbox;
    private LayerMask PlayerMask;

    private enum State
    { Patrol, Chase, Knockback }

    private State state = State.Patrol;

    private Canvas healthCanvas;

    // Start is called before the first frame update
    private void Start()
    {
        eData = GetComponent<EnemyData>();

        PlayerMask = LayerMask.GetMask("Player");
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
        // dò tìm Player
        LookingForPlayer();
        // cập nhật hướng quay mặt
        FacingDirectionUpdate();

        if (eData.CurrentHealth <= 0)
        {
            healthCanvas.enabled = false;
        }
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
            state = State.Chase;
            return;
        }

        //thay đổi hướng di chuyển nếu con đi quá xa khỏi điểm tuần tra
        if (Vector2.Distance(transform.position, PatrolPoint.transform.position) > PatrolDistance)
        {
            //Vector AB = B - A
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
            Die();
        }

        if (!healthCanvas.isActiveAndEnabled)
        {
            healthCanvas.enabled = true;
        }

        //chuyển sang trạng thái bị đẩy lùi
        state = State.Knockback;
        KnockbackDistanceRemain = KnockBackDistance;
        //Gán hướng bị đẩy lùi
        KnockbackDirection = Convert.ToInt32(package[1]);
    }

    private void Die()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(this.gameObject, 5);
    }

    private void Knockback()
    {
        var velocity = new Vector3(KnockBackSpeed * KnockbackDirection, 0, 0);
        transform.position += velocity;
        KnockbackDistanceRemain -= Mathf.Abs(velocity.x);

        if (KnockbackDistanceRemain <= 0)
        {
            state = State.Chase;
        }
    }

    private void Chase()
    {
        //nếu không phát hiện player nữa thì chuyển sang trạng thái tuần tra
        if (playerDetector == null)
        {
            state = State.Patrol;
            return;
        }

        //cập nhật hướng di chuyển tới nhân vật
        moveDirection = (playerDetector.transform.position - transform.position).normalized;

        transform.position += new Vector3(moveDirection.x, moveDirection.y, 0) * (Speed + 2);
        //Attack nếu chạm được vào người của player
        DealDamage();
    }

    //cập nhật giá trị cho playerDetector
    private void LookingForPlayer()
    {
        playerDetector = Physics2D.OverlapCircle(transform.position, PlayerDetectRadius, PlayerMask);
    }

    private void DealDamage()
    {
        if (eData.CurrentHealth <= 0)
        {
            return;
        }
        Collider2D hit = Physics2D.OverlapBox(transform.position, Hitbox.size, 0, PlayerMask);
        if (hit)
        {
            object[] package = new object[1];
            package[0] = AttackDamage;
            hit.SendMessage("TakeDamage", package);
        }
    }
}