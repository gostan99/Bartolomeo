using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Player;
using UnityEngine.UI;
using Assets.Scripts.Entities;

public class HealthEnemy : MonoBehaviour
{
    public GameObject Enemy;
    public EnemyData eData;
    public PlayerData pData;
    public Slider slider { get; private set; }


    // Start is called before the first frame update
    void Start()
    {
        eData = Enemy.GetComponent<EnemyData>();
        pData = GetComponent<PlayerData>();
        slider = gameObject.GetComponent<Slider>();

        slider.maxValue = eData.MaxHealth;
        slider.minValue = 0;
        slider.value = eData.CurrentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = eData.CurrentHealth;
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
