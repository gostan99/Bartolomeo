using Assets.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.UIContext.InventorySystem
{
    public class ItemDescription : MonoBehaviour
    {
        private Text text;
        private GameObject SelectedItemSlot;

        private void Start()
        {
            text = transform.Find("Text").GetComponent<Text>();
        }

        private void Update()
        {
            SelectedItemSlot = EventSystem.current.currentSelectedGameObject;
            if (SelectedItemSlot != null && (SelectedItemSlot.CompareTag("InventorySlot") || SelectedItemSlot.CompareTag("EquipmentSlot")))
            {
                Item item;
                bool hasAnItem = SelectedItemSlot.transform.Find("Item").TryGetComponent<Item>(out item);
                if (hasAnItem)
                {
                    text.text = item.Description;
                }
                else
                {
                    text.text = "";
                }
            }
            else
            {
                text.text = "";
            }
        }
    }
}