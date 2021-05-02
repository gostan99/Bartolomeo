using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Player;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject Player;
    private PlayerData playerData;
    public Slider slider { get; private set; }

    //   public Gradient gradient;

    // Start is called before the first frame update
    private void Start()
    {
        playerData = Player.GetComponent<PlayerData>();
        slider = gameObject.GetComponent<Slider>();

        slider.maxValue = playerData.maxHealth;
        slider.minValue = 0;
        slider.value = playerData.currentHealth;
    }

    // Update is called once per frame
    private void Update()
    {
        slider.value = playerData.currentHealth;
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
    }
}