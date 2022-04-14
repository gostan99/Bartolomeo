using Assets.Scripts.Player;
using System;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class LargeHealthPotion : Item
    {
        private GameObject player;
        private PlayerData pData;
        private PlayerController pController;
        public int HealValue => 25;

        public override bool IsEquipable => false;
        public override bool IsUseable => true;
        public override string Description => "Heal " + HealValue.ToString() + " Health";
        public override Sprite Sprite => Resources.LoadAll<Sprite>("Images/Items/HealthPotion/Large_Health_Postion")[0];
        public override float ImageWidth => 62;
        public override float ImageHeight => 72;

        private void Awake()
        {
            //tìm kiếm Player trong scene
            player = GameObject.Find("Player");
            //lấy PlayerData component
            pData = player.GetComponent<PlayerData>();
            //lấy PlayerController component
            pController = player.GetComponent<PlayerController>();
        }

        public override void UseItem()
        {
            if (pData.currentHealth + HealValue > pData.maxHealth)
            {
                pData.currentHealth = pData.maxHealth;
            }
            else
            {
                pData.currentHealth += HealValue;
            }
        }

        public override void EquipItem()
        {
            throw new NotImplementedException();
        }

        public override void UnequipItem()
        {
            throw new NotImplementedException();
        }
    }
}