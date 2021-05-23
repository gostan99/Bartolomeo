using System;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public abstract class Item : ScriptableObject
    {
        public bool IsEquipable { get; protected set; } = false;
        public bool IsUseable { get; protected set; } = false;
        public string Description;
        public Sprite sprite { get; set; }
        public float ImageWidth { get; set; }
        public float ImageHeight { get; set; }

        public abstract void Use();

        public abstract void Equip();
    }

    public enum ItemType
    {
        SmallHealthPotion,
        LargeHealthPotion,
        ManaPotion,
        GiantSword
    }
}