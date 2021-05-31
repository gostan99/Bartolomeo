using Assets.Scripts.Player;
using System;
using UnityEngine;

namespace Assets.Scripts.Items.InInventory
{
    public class NeckLaceK : Item
    {
        private GameObject player;
        private PlayerData pData;
        private int Value = 30;

        public override bool IsEquipable => true;

        public override bool IsUseable => false;

        public override string Description => "Increase max Mana by " + Value.ToString();

        public override Sprite Sprite => Resources.LoadAll<Sprite>("Images/Items/NeckLece/necklace_01b")[0];

        public override float ImageWidth => 72;

        public override float ImageHeight => 72;

        private void Awake()
        {
            //tìm kiếm Player trong scene
            player = GameObject.Find("Player");
            //lấy PlayerData component
            pData = player.GetComponent<PlayerData>();
            //lấy PlayerController component
        }

        public override void EquipItem()
        {
            pData.maxMana += Value;
        }

        public override void UnequipItem()
        {
            pData.maxMana -= Value;
        }

        public override void UseItem()
        {
            throw new NotImplementedException();
        }
    }
}