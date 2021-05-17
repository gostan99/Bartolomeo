using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Items;

namespace Assets.Scripts.Entities
{
    public class DropItem : MonoBehaviour
    {
        private readonly Dictionary<Item, int> _itemsAndOdd;

        public DropItem(Dictionary<Item, int> itemsAndOdd)
        {
            _itemsAndOdd = itemsAndOdd;
        }

        public Item Drop()
        {
            List<Item> items = new List<Item>();
            foreach (var key in _itemsAndOdd.Keys)
            {
                _itemsAndOdd.TryGetValue(key, out int odd);
                for (int i = 0; i < odd; i++)
                {
                    items.Add(key);
                }
            }
            int index = UnityEngine.Random.Range(0, items.Count);
            return items[index];
        }
    }
}