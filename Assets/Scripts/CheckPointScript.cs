using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPointScript : MonoBehaviour
{
    public Vector2 size = new Vector2(93, 138);
    private LayerMask PlayerLayer;
    private PlayerData playerData;
    private PlayerController playerController;
    private Text hintText; // text gợi ý

    // Start is called before the first frame update
    private void Start()
    {
        playerData = GameObject.Find("Player").GetComponent<PlayerData>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        PlayerLayer = LayerMask.GetMask("Player");
        hintText = transform.Find("Canvas").Find("Text").GetComponent<Text>();
        hintText.enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        Collider2D collided = Physics2D.OverlapBox(transform.position, size, 0, PlayerLayer);
        if (collided)
        {
            hintText.enabled = true;
            if (Input.GetKey(KeyCode.E))
            {
                hintText.text = "Saved!";
                playerData.HasCheckPoint = true;
                playerData.PosX = transform.position.x;
                playerData.PosY = transform.position.y;
                playerController.currentState.SetNewState(playerController.CheckPointState);
            }
        }
        else
        {
            hintText.enabled = false;
        }
    }
}