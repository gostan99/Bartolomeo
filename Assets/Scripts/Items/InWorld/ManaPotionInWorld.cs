using UnityEngine;

namespace Assets.Scripts.Items.InWorld
{
    public class ManaPotionInWorld : ItemInWorld
    {
        private void Update()
        {
            if (isCollided)
            {
                var item = ScriptableObject.CreateInstance<ManaPotion>();
                if (inventory.AddItem(item, ItemType.ManaPotion, 1))
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