using Assets.Scripts.Player;
using Assets.Scripts.UI.UIContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class ItemInWorld : MonoBehaviour
    {
        private GameObject canvas;
        protected InventoryMenu inventory;
        protected bool isCollided;

        private void Start()
        {
            canvas = GameObject.FindGameObjectWithTag("Canvas");
            inventory = canvas.GetComponent<InventoryMenu>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer.Equals(12))
            {
                isCollided = true;
            }
        }
    }
}