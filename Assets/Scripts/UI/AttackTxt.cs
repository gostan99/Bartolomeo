using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

public class AttackTxt : MonoBehaviour
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
        txt.text = pData.AttackDamage.ToString();
    }
}