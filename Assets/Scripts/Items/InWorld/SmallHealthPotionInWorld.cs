using UnityEngine;

namespace Assets.Scripts.Items.InWorld
{
    internal class SmallHealthPotionInWorld : ItemInWorld
    {
        private void Update()
        {
            if (isCollided)
            {
                var item = ScriptableObject.CreateInstance<SmallHealthPotion>();
                if (inventory.AddItem(item, ItemType.SmallHealthPotion, 1))
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    //TO DO: hiện thông báo túi đồ đã đầy
                    Debug.LogWarning("Inventory is full!");
                }
            }
        }
    }
}