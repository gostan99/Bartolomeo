using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Player;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    public GameObject Player;
    public PlayerData playerData;
    public Slider slider { get; private set; }

    // Start is called before the first frame update
    private void Start()
    {
        Player = GameObject.FindWithTag("Player");
        playerData = Player.GetComponent<PlayerData>();
        slider = gameObject.GetComponent<Slider>();

        slider.maxValue = playerData.maxMana;
        slider.minValue = 0;
        slider.value = playerData.currentMana;
    }

    // Update is called once per frame
    private void Update()
    {
        slider.value = playerData.currentMana;
    }

    public void SetMaxHealth(float mana)
    {
        slider.maxValue = mana;
    }

    public void SetHealth(float mana)
    {
        slider.value = mana;
    }
}