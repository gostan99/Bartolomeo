using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlat : MonoBehaviour
{
    public Vector2 size = new Vector2(40f, 122f);
    public float speed = 3f;
    public int direction = 1;
    private LayerMask Plat;
    // Start is called before the first frame update
    void Start()
    {
        Plat = LayerMask.GetMask("MovingPlatformClamp");
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Collider2D collided = Physics2D.OverlapBox(transform.position, size, 0, Plat);
        if (collided)
        {
            direction = -direction;
        }
        transform.position += new Vector3(speed * direction, 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(transform.position, size);
    }



    public void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer.Equals(12))
            col.transform.parent = transform;

    }

    public void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.layer.Equals(12))
            col.transform.parent = null;

    }
}
