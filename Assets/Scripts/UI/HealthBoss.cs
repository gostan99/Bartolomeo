using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Entities;

public class HealthBoss : MonoBehaviour
{
    public bool healthBossUI;
    public GameObject BossGoat;
    private BossGoat bossGoat;
    public Slider slider { get; private set; }

    //   public Gradient gradient;

    // Start is called before the first frame update
    void Start()
    {
        bossGoat = BossGoat.GetComponent<BossGoat>();
        slider = gameObject.GetComponent<Slider>();

        slider.maxValue = bossGoat.MaxHealth;
        slider.minValue = 0;
        slider.value = bossGoat.CurrentHealth;

    }

    // Update is called once per frame
    void Update()
    {
        slider.value = bossGoat.CurrentHealth;
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
