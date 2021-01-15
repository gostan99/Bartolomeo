using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinGame : MonoBehaviour
{
    PlayerDTO playerDTO;

    public Vector2 size = new Vector2(45.21f, 85.87f);
    private LayerMask PlayerLayer;

    // Start is called before the first frame update
    void Start()
    {
        PlayerLayer = LayerMask.GetMask("Player");
       
        playerDTO = new PlayerDTO();
        string path = @"Assets\Data\Save\playerdata.json";
        string jsonData;

        if (File.Exists(path))
        {
            PlayerDTO saveData = new PlayerDTO();
            saveData.Level = "Assets/Scenes/Map1/VachNui2.unity";
            saveData.HasDash = PlayerData.HasDash;
            saveData.MaxJumpCounter = PlayerData.MaxJumpCounter;

            jsonData = JsonUtility.ToJson(saveData);
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteAsync(jsonData);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D collided = Physics2D.OverlapBox(transform.position, size, 0, PlayerLayer);
        if (collided)
        {
            SceneManager.LoadScene(0);
        }
    }
}