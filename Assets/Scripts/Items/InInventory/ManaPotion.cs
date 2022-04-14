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

        public override bool IsEquipable => false;
        public override bool IsUseable => true;
        public override string Description => "Fill Up Full Mana Bar";
        public override Sprite Sprite => Resources.LoadAll<Sprite>("Images/Items/ManaPotion/Mana_Potion")[0];
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
            pData.currentMana = pData.maxMana;
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