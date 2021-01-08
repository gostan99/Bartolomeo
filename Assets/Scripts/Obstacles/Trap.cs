using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trap : MonoBehaviour
{
    public float AttackDamage = 99999999f;

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer.Equals(12))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }
}
