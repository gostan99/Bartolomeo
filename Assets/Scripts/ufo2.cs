using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ufo2 : MonoBehaviour
{
    public int health = 100;
    public float speed = 100f;
    public Vector3 direction;
    public GameObject player;
    public Collider2D col;
    public Rigidbody2D r2;
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        col = gameObject.GetComponentInChildren<Collider2D>();
        r2 = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if (health <= 0)
            Destroy(this.gameObject);

        direction = player.transform.position - this.transform.position;
        direction.Normalize();
        r2.velocity = direction * speed;
        Debug.Log("position: " + direction);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
            transform.position = transform.position - (direction * 2);
        else
            transform.position = transform.position + (direction);


    }

    void Damage(int dmg)
    {
        health -= dmg;
    }



}