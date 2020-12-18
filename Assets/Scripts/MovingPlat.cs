using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlat : MonoBehaviour
{

    public float speed = 20.0f, direction = 1;
    //Vector3 Move;
    public Rigidbody2D Move;
    // Start is called before the first frame update
    void Start()
    {
        Move = gameObject.GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {


        if (Move.velocity.x == 0)
        {
            direction = -direction;
        }
        Move.velocity = new Vector3(speed * direction, Move.velocity.y);

    }

}



