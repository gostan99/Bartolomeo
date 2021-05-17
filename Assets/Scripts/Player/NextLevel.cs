using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public Vector2 size = new Vector2(45.21f, 85.87f);
    private LayerMask PlayerLayer;

    // Start is called before the first frame update
    private void Start()
    {
        PlayerLayer = LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    private void Update()
    {
        Collider2D collided = Physics2D.OverlapBox(transform.position, size, 0, PlayerLayer);
        if (collided)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}