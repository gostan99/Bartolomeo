using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public Vector2 size = new Vector2(45.21f, 85.87f);
    private LayerMask PlayerLayer;
    private PlayerData playerData;

    // Start is called before the first frame update
    private void Start()
    {
        PlayerLayer = LayerMask.GetMask("Player");
        playerData = GameObject.Find("Player").GetComponent<PlayerData>();
    }

    // Update is called once per frame
    private void Update()
    {
        Collider2D collided = Physics2D.OverlapBox(transform.position, size, 0, PlayerLayer);
        if (collided)
        {
            playerData.HasCheckPoint = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}