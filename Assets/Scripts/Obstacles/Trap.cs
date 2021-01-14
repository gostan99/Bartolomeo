using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trap : MonoBehaviour
{
    public float AttackDamage = 99999999f;

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer.Equals(12) || col.gameObject.layer.Equals(11))
        {
            object[] package = new object[2];
            package[0] = 99999f;
            package[1] = null;
            col.gameObject.SendMessage("TakeDamage", package);
        }
    }
}
