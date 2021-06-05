using Assets.Scripts.Player;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private PlayerDTO playerDTO;

    private void Start()
    {
        playerDTO = new PlayerDTO();
        var continueBtn = transform.Find("ContinueButton");
        var continueText = continueBtn.Find("ContinueText");
        string path = @"Assets\Data\Save\playerdata.json";
        string jsonData;

        if (!File.Exists(path))
        {
            continueBtn.GetComponent<Image>().enabled = false;
            continueBtn.GetComponent<Button>().enabled = false;
            continueText.GetComponent<Text>().enabled = false;
        }
        else
        {
            jsonData = File.ReadAllText(path);
            playerDTO = JsonUtility.FromJson<PlayerDTO>(jsonData);

            if (playerDTO.Level == "Assets/Scenes/Map1/VachNui2.unity")
            {
                continueBtn.GetComponent<Image>().enabled = false;
                continueBtn.GetComponent<Button>().enabled = false;
                continueText.GetComponent<Text>().enabled = false;
            }
            else
            {
                continueBtn.GetComponent<Image>().enabled = true;
                continueBtn.GetComponent<Button>().enabled = true;
                continueText.GetComponent<Text>().enabled = true;
            }
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Continue()
    {
        //Bật continue menu
        transform.parent.Find("ContinueMenu").gameObject.SetActive(true);
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