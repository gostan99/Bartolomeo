using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public bool enable = false;
    public GameObject DebugUI;
    private GameObject nextLevelPoint;
    private GameObject player;
    private PlayerData playerData;

    // Use this for initialization
    private void Start()
    {
        DebugUI.SetActive(false);
        //tìm kiếm Player trong scene
        player = GameObject.Find("Player");
        nextLevelPoint = GameObject.Find("NextLevel");
        //lấy PlayerData component
        playerData = player.GetComponent<PlayerData>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete) && !enable)
        {
            enable = true;
            DebugUI.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Delete) && enable)
        {
            enable = false;
            DebugUI.SetActive(false);
        }
    }

    public void TpToNextLevelPoint()
    {
        if (nextLevelPoint == null)
        {
            return;
        }
        var newPos = new Vector3(nextLevelPoint.transform.position.x - 50,
            nextLevelPoint.transform.position.y, player.transform.position.z);
        player.transform.position = newPos;
    }

    public void FullHealth()
    {
        playerData.currentHealth = playerData.maxHealth;
    }

    public void FullMana()
    {
        if (playerData.currentMana == playerData.maxMana)
        {
            playerData.currentMana += 0;
        }
        else
        {
            playerData.currentMana += 10;
        }
    }
}