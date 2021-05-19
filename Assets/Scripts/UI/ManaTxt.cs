using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

public class ManaTxt : MonoBehaviour
{
    private GameObject player;
    private PlayerData pData;
    private Text txt;

    // Start is called before the first frame update
    private void Start()
    {
        //tìm kiếm Player trong scene
        player = GameObject.Find("Player");
        //lấy PlayerData component
        pData = player.GetComponent<PlayerData>();
        txt = GetComponent<Text>();
    }

    private void Update()
    {
        txt.text = pData.currentMana.ToString() + "/" + pData.maxMana.ToString();
    }
}