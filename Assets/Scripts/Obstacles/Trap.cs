using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public float AttackDamage = 99999999f;

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer.Equals(12))
        {
            object[] package = new object[2];
            package[0] = AttackDamage;
            col.collider.SendMessage("TakeDamage", package);
        }

    }
}
