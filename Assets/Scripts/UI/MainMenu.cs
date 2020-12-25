using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    PlayerDTO playerDTO;

    private void Start()
    {
        PlayerDTO playerDTO = new PlayerDTO();

        string path = @"C:\Dev\Long's Project\Bartolomeo\Assets\Data\Save\playerdata.json";
        string jsonData = File.ReadAllText(path);
        playerDTO = JsonUtility.FromJson<PlayerDTO>(jsonData);

        if (playerDTO.Level != "Assets/Scenes/Map1/VachNui.unity")
        {
            transform.Find("ContinueButton").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("ContinueButton").gameObject.SetActive(false);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Continue()
    {
        SceneManager.LoadScene(playerDTO.Level);
        PlayerData.HasDash = playerDTO.HasDash;
        PlayerData.MaxJumpCounter = playerDTO.MaxJumpCounter;
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
