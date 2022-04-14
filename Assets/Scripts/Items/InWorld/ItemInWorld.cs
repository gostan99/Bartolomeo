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
        protected AudioSource audioSource;
        protected AudioClip sound;
        protected bool soundHasPlayed = false;

        private void Start()
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("ItemInWorld"), LayerMask.NameToLayer("Entity"));

            var canvas = GameObject.FindGameObjectWithTag("Canvas");
            var inventoryUI = canvas.transform.Find("InventoryUI");
            itemSlotManager = inventoryUI.Find("Items").GetComponent<ItemSlotManager>();

            audioSource = GetComponent<AudioSource>();
            sound = Resources.Load<AudioClip>(@"Sounds/geo_small_collect_1");
            var rb = GetComponent<Rigidbody2D>();
            float rand = UnityEngine.Random.Range(-0.6f, 0.6f);
            float thrust = UnityEngine.Random.Range(200f, 300f);
            Vector2 direction = new Vector2(rand, 1);
            rb.AddForce(direction * thrust, ForceMode2D.Impulse);
        }

        protected virtual void Update()
        {
            if (soundHasPlayed && !audioSource.isPlaying)
            {
                Destroy(this.gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                isCollided = true;
                itemSlotManager.AddItemToInventorySlot(typeof(T), 1);
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<BoxCollider2D>().enabled = false;
                soundHasPlayed = true;
                audioSource.PlayOneShot(sound);
            }
        }
    }
}