using Assets.Scripts.Player;
using System;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class SmallHealthPotion : Item
    {
        private GameObject player;
        private PlayerData pData;
        private PlayerController pController;
        public int HealValue { get; } = 25;

        private void Awake()
        {
            //tìm kiếm Player trong scene
            player = GameObject.Find("Player");
            //lấy PlayerData component
            pData = player.GetComponent<PlayerData>();
            //lấy PlayerController component
            pController = player.GetComponent<PlayerController>();
            IsUseable = true;
            Description = "Heal " + HealValue.ToString() + " Health";
            ImageWidth = 62;
            ImageHeight = 72;
            sprite = Resources.LoadAll<Sprite>("Images/Items/HealthPotion/Mini_Health_Postion")[0];
        }

        public override void Use()
        {
            if (pData.currentHealth + HealValue > pData.maxHealth)
            {
                pData.currentHealth = pData.maxHealth;
            }
            else
            {
                pData.currentHealth += HealValue;
            }
            //Đưa player vào state hồi máu
            //pController.currentState.SetNewState(pController.HeallingState);
        }

        public override void Equip()
        {
            throw new NotImplementedException();
        }
    }
}