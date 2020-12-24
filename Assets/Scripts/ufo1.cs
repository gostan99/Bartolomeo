using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ufo1 : MonoBehaviour
{
    public int health = 100;
    public float timer = 1;
    public Vector3 direction;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (health <= 0)
            Destroy(this.gameObject);

        transform.Translate(direction * Time.deltaTime / 0.4f);

        if (timer > 1)
        {
            timer = 0;
            newposition();
        }

    }

    void Damage(int dmg)
    {
        health -= dmg;
    }
    void newposition()
    {
        direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);

    }


}