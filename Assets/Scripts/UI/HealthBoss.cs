using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Entities;

public class HealthBoss : MonoBehaviour
{
    public bool healthBossUI;
    public GameObject BossGoat;
    private EnemyData bossData;
    public Slider slider { get; private set; }

    //   public Gradient gradient;

    // Start is called before the first frame update
    private void Start()
    {
        bossData = BossGoat.GetComponent<EnemyData>();
        slider = gameObject.GetComponent<Slider>();

        slider.maxValue = bossData.MaxHealth;
        slider.minValue = 0;
        slider.value = bossData.CurrentHealth;
    }

    // Update is called once per frame
    private void Update()
    {
        slider.value = bossData.CurrentHealth;
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