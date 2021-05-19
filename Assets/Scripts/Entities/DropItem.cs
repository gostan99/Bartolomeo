using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Items;

namespace Assets.Scripts.Entities
{
    public class DropItem : MonoBehaviour
    {
        public GameObject Item;
        public int Odd;
        public GameObject Host;

        private Canvas healthCanvas;
        private bool healthCanvasHasEnable = false;
        private bool willDrop = false;
        private bool hasDrop = false;

        private void Start()
        {
            healthCanvas = Host.GetComponentInChildren<Canvas>();
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
            if (!healthCanvasHasEnable)
            {
                if (healthCanvas.enabled)
                {
                    healthCanvasHasEnable = true;
                }
            }
            else
            {
                if (!healthCanvas.enabled)
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
}