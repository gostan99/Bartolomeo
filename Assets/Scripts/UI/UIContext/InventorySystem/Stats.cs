using Assets.Scripts.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.UIContext.InventorySystem
{
    public class Stats : MonoBehaviour
    {
        private Text AttackStat;
        private Text HealthStat;
        private Text ManaStat;

        private GameObject player;
        private PlayerData pData;

        private void Start()
        {
            player = GameObject.Find("Player");
            pData = player.GetComponent<PlayerData>();

            AttackStat = transform.Find("Attack").Find("Content").GetComponent<Text>();
            HealthStat = transform.Find("Health").Find("Content").GetComponent<Text>();
            ManaStat = transform.Find("Mana").Find("Content").GetComponent<Text>();
        }

        private void Update()
        {
            AttackStat.text = pData.AttackDamage.ToString();
            HealthStat.text = pData.currentHealth.ToString() + "/" + pData.maxHealth.ToString();
            ManaStat.text = pData.currentMana.ToString() + "/" + pData.maxMana.ToString();
        }
    }
}