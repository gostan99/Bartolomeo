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
    void Start()
    {
        playerData = Player.GetComponent<PlayerData>();
        slider=gameObject.GetComponent<Slider>();

        playerData.currentHealth = playerData.maxHealth;
        SetMaxHealth(playerData.maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            TakeDamage(20);
        }
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }

    public void TakeDamage(int damage)
    {
        playerData.currentHealth -= damage;
        SetHealth(playerData.currentHealth);
    }
}
