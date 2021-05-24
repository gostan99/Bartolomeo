using Assets.Scripts.UI.UIContext.InventorySystem;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Items
{
    public class ItemInWorld<T> : MonoBehaviour where T : Item
    {
        protected ItemSlotManager itemSlotManager;
        protected bool isCollided;

        private void Start()
        {
            var canvas = GameObject.FindGameObjectWithTag("Canvas");
            var inventoryUI = canvas.transform.Find("InventoryUI");
            itemSlotManager = inventoryUI.Find("Items").GetComponent<ItemSlotManager>();

            var rb = GetComponent<Rigidbody2D>();
            float rand = UnityEngine.Random.Range(-0.6f, 0.6f);
            float thrust = 200f;
            Vector2 direction = new Vector2(rand, 1);
            rb.AddForce(direction * thrust, ForceMode2D.Impulse);
        }

        private void Update()
        {
            if (isCollided)
            {
                itemSlotManager.AddItemToInventorySlot(typeof(T), 1);
                Destroy(this.gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer.Equals(12))//12 là player
            {
                isCollided = true;
            }
        }
    }
}