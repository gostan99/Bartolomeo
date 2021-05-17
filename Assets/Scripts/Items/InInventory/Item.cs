using System;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public abstract class Item : ScriptableObject
    {
        public bool IsUseable { get; protected set; }
        public string Description;
        public Sprite sprite { get; set; }
        public float ImageWidth { get; set; }
        public float ImageHeight { get; set; }

        public abstract void Use();
    }

    public enum ItemType
    {
        SmallHealthPotion,
        BigHealthPotion
    }
}