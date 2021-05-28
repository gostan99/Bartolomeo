using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Items;

namespace Assets.Scripts.Items
{
    public class DropItem : MonoBehaviour
    {
        public GameObject Item;
        public int Odd;
        public GameObject Host;

        private BoxCollider2D boxCollider2D;
        private bool willDrop = false;

        private void Start()
        {
            boxCollider2D = Host.GetComponentInChildren<BoxCollider2D>();
            int val = Random.Range(1, 101);// random số từ 1 đến 100
            if (val <= Odd)
            {
                willDrop = true;
            }
            else
            {
                willDrop = false;
            }
        }

        private void Update()
        {
            if (!boxCollider2D.enabled)
            {
                if (willDrop)
                {
                    Vector2 pos = new Vector2(Host.transform.position.x, Host.transform.position.y + 5);
                    Instantiate(Item, pos, Quaternion.identity);
                    Destroy(this);
                }
            }
        }
    }
}