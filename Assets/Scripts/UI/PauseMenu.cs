using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool pause = false;
    public GameObject pauseUI;

    // Use this for initialization
    void Start()
    {
        pauseUI.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !pause)
        {
            pause =true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && pause)
        {
            pause = false;
        }


        if (pause)
        {
            pauseUI.SetActive(true);
            Time.timeScale = 0;
        }
        if (pause == false)
        {
            pauseUI.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void Resume()
    {
        pause = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().path);
    }

    public void BackToMainMenu()
    {
        PlayerDTO saveData = new PlayerDTO();
        saveData.Level = SceneManager.GetActiveScene().path;
        saveData.HasDash = PlayerData.HasDash;
        saveData.MaxJumpCounter = PlayerData.MaxJumpCounter;

        var jsonData = JsonUtility.ToJson(saveData);
        string path = @"C:\Dev\Long's Project\Bartolomeo\Assets\Data\Save\playerdata.json";
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteAsync(jsonData);
        }

        SceneManager.LoadScene(0);
    }

}
