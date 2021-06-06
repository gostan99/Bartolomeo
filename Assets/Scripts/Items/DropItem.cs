using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Items;
using Assets.Scripts.Entities;

namespace Assets.Scripts.Items
{
    public class DropItem : MonoBehaviour
    {
        public GameObject Item;
        public int Odd;
        public int Amount = 1;

        private EnemyData eData;
        private bool willDrop = false;

        private void Start()
        {
            eData = GetComponent<EnemyData>();
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
            if (eData.CurrentHealth <= 0)
            {
                if (willDrop)
                {
                    Vector2 pos = new Vector2(transform.position.x, transform.position.y + 5);
                    for (int i = 0; i < Amount; i++)
                    {
                        Instantiate(Item, pos, Quaternion.identity);
                    }
                    Destroy(this);
                }
            }
        }
    }
}