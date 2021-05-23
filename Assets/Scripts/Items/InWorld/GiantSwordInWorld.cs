using UnityEngine;

namespace Assets.Scripts.Items.InWorld
{
    public class GiantSwordInWorld : ItemInWorld
    {
        private void Update()
        {
            if (isCollided)
            {
                var item = ScriptableObject.CreateInstance<GiantSword>();
                if (inventory.AddItem(item, ItemType.GiantSword, 1))
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