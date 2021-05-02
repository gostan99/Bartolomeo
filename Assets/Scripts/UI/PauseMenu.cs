using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public bool pause = false;
    public GameObject pauseUI;
    private GameObject player;
    private PlayerData playerData;

    // Use this for initialization
    void Start()
    {
        pauseUI.SetActive(false);
        //tìm kiếm Player trong scene
        player = GameObject.Find("Player");
        //lấy PlayerData component
        playerData = player.GetComponent<PlayerData>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (playerData.currentHealth <= 0 && !pause) //check máu player
        {
            pause = true;
            pauseUI.SetActive(true);
            Time.timeScale = 0;

            //nếu player hết máu thì tắt Image và Button component
            if (playerData.currentHealth <= 0)
            {
                var resumeBtn = pauseUI.transform.Find("ResumeButton");
                resumeBtn.GetComponent<Image>().enabled = false;
                resumeBtn.GetComponent<Button>().enabled = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && !pause )
        {
            pause = true;
            pauseUI.SetActive(true);
            Time.timeScale = 0;

            //nếu player hết máu thì tắt Image và Button component
            if (playerData.currentHealth <= 0)
            {
                var resumeBtn = pauseUI.transform.Find("ResumeButton");
                resumeBtn.GetComponent<Image>().enabled = false;
                resumeBtn.GetComponent<Button>().enabled = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && pause)
        {
            pause = false;
            pauseUI.SetActive(false);
            Time.timeScale = 1;
        }

        //if (pause)
        //{
        //    pauseUI.SetActive(true);
        //    Time.timeScale = 0;

        //    //nếu player hết máu thì tắt Image và Button component
        //    if (playerData.currentHealth <= 0)
        //    {
        //        var resumeBtn = pauseUI.transform.Find("ResumeButton");
        //        resumeBtn.GetComponent<Image>().enabled = false;
        //        resumeBtn.GetComponent<Button>().enabled = false;
        //    }
        //}
        //if (pause == false)
        //{
        //    pauseUI.SetActive(false);
        //    Time.timeScale = 1;
        //}
    }

    public void Resume()
    {

        Time.timeScale = 1;
        pause = false;
        pauseUI.SetActive(false);

    }

    public void Restart()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().path);
    }

    public void BackToMainMenu()
    {
        PlayerDTO saveData = new PlayerDTO();
        saveData.Level = SceneManager.GetActiveScene().path;
        saveData.HasDash = PlayerData.HasDash;
        saveData.MaxJumpCounter = PlayerData.MaxJumpCounter;

        var jsonData = JsonUtility.ToJson(saveData);
        string path = @"Assets\Data\Save\playerdata.json";
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteAsync(jsonData);
        }

        SceneManager.LoadScene(0);
    }

}
