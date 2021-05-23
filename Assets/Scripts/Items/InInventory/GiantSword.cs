using Assets.Scripts.Player;
using System;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class GiantSword : Item
    {
        private GameObject player;
        private PlayerData pData;
        private PlayerController pController;
        public int value { get; } = 5;

        private void Awake()
        {
            //tìm kiếm Player trong scene
            player = GameObject.Find("Player");
            //lấy PlayerData component
            pData = player.GetComponent<PlayerData>();
            //lấy PlayerController component
            pController = player.GetComponent<PlayerController>();
            IsUseable = false;
            IsEquipable = true;
            Description = "Increase max attack by " + value.ToString();
            ImageWidth = 37;
            ImageHeight = 72;
            sprite = Resources.LoadAll<Sprite>("Images/Items/Sword/pontiff_giantSword_swordSprite")[0];
        }

        public override void Use()
        {
            throw new NotImplementedException();
        }

        public override void Equip()
        {
            pData.AttackDamage += value;
        }
    }
}