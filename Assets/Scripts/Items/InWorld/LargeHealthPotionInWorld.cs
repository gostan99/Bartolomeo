using UnityEngine;

namespace Assets.Scripts.Items.InWorld
{
    public class LargeHealthPotionInWorld : ItemInWorld
    {
        private void Update()
        {
            if (isCollided)
            {
                var item = ScriptableObject.CreateInstance<LargeHealthPotion>();
                if (inventory.AddItem(item, ItemType.LargeHealthPotion, 1))
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