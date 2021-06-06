using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Items.InInventory
{
    public class Coin : Item
    {
        public override bool IsEquipable => throw new NotImplementedException();

        public override bool IsUseable => throw new NotImplementedException();

        public override string Description => throw new NotImplementedException();

        public override Sprite Sprite => throw new NotImplementedException();

        public override float ImageWidth => throw new NotImplementedException();

        public override float ImageHeight => throw new NotImplementedException();

        public override void EquipItem()
        {
            throw new NotImplementedException();
        }

        public override void UnequipItem()
        {
            throw new NotImplementedException();
        }

        public override void UseItem()
        {
            throw new NotImplementedException();
        }
    }
}