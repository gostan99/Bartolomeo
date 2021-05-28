using System;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public abstract class Item : MonoBehaviour
    {
        public abstract bool IsEquipable { get; }
        public abstract bool IsUseable { get; }
        public abstract string Description { get; }
        public abstract Sprite Sprite { get; }
        public abstract float ImageWidth { get; }
        public abstract float ImageHeight { get; }

        public abstract void UseItem();

        public abstract void EquipItem();

        public abstract void UnequipItem();
    }
}