using Assets.Scripts.Player;
using System;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class ManaPotion : Item
    {
        private GameObject player;
        private PlayerData pData;
        private PlayerController pController;

        private void Awake()
        {
            //tìm kiếm Player trong scene
            player = GameObject.Find("Player");
            //lấy PlayerData component
            pData = player.GetComponent<PlayerData>();
            //lấy PlayerController component
            pController = player.GetComponent<PlayerController>();
            IsUseable = true;
            IsEquipable = false;
            Description = "Fill Up Full Mana Bar";
            ImageWidth = 62;
            ImageHeight = 72;
            sprite = Resources.LoadAll<Sprite>("Images/Items/ManaPotion/Mana_Potion")[0];
        }

        public override void Use()
        {
            pData.currentMana = pData.maxMana;
        }

        public override void Equip()
        {
            throw new NotImplementedException();
        }
    }
}