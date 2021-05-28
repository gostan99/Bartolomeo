using Assets.Scripts.Player;
using System;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class GiantSword : Item
    {
        private GameObject player;
        private PlayerData pData;
        public int Value => 5;

        public override bool IsEquipable => true;

        public override bool IsUseable => false;

        public override string Description => "Increase max attack by " + Value.ToString();
        public override Sprite Sprite => Resources.LoadAll<Sprite>("Images/Items/Sword/pontiff_giantSword_swordSprite")[0];

        public override float ImageWidth => 37;
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
            pData.AttackDamage += Value;
        }

        public override void UseItem()
        {
            throw new NotImplementedException();
        }

        public override void UnequipItem()
        {
            pData.AttackDamage -= Value;
        }
    }
}