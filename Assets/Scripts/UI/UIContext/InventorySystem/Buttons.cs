using Assets.Scripts.Items;
using Assets.Scripts.Player;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.UIContext.InventorySystem
{
    public class Buttons : MonoBehaviour
    {
        private PlayerInput pInput;

        private GameObject EquipBtn;
        private GameObject UnequipBtn;
        private GameObject UseBtn;
        private GameObject DropBtn;

        private GameObject SelectedItemSlot;
        private GameObject SelectedEquipmentSlot;
        private GameObject currentSelectedObj;

        private ItemSlotManager itemSlotManager;
        private EquipmentSlotManager equipmentSlotManager;

        private void Start()
        {
            pInput = GameObject.Find("Player").GetComponent<PlayerController>().pInput;

            EquipBtn = transform.Find("EquipBtn").gameObject;
            UnequipBtn = transform.Find("UnequipBtn").gameObject;
            UseBtn = transform.Find("UseBtn").gameObject;
            DropBtn = transform.Find("DropBtn").gameObject;

            itemSlotManager = transform.parent.Find("Items").GetComponent<ItemSlotManager>();
            equipmentSlotManager = transform.parent.Find("Equipments").GetComponent<EquipmentSlotManager>();
        }

        private void Update()
        {
            currentSelectedObj = EventSystem.current.currentSelectedGameObject;
            if (currentSelectedObj != null)
            {
                if (currentSelectedObj.CompareTag("InventorySlot"))
                {
                    SelectedItemSlot = currentSelectedObj;
                    TryEnableEquipBtn();
                    TryEnableUseBtn();
                    TryEnableDropBtn();

                    DisableUnequipBtn();
                }
                else if (currentSelectedObj.CompareTag("EquipmentSlot"))
                {
                    SelectedEquipmentSlot = currentSelectedObj;
                    TryEnableUnequipBtn();

                    DisableEquipBtn();
                    DisableUseBtn();
                    DisableDropBtn();
                }
            }
            else
            {
                DisableEquipBtn();
                DisableUnequipBtn();
                DisableUseBtn();
                DisableDropBtn();
            }

            if (pInput.LargePotionInput)
            {
                if (itemSlotManager.TryGetLargePotionItemSlot(out SelectedItemSlot))
                {
                    Use();
                }
            }
            else if (pInput.SmallPotionInput)
            {
                if (itemSlotManager.TryGetSmallPotionItemSlot(out SelectedItemSlot))
                {
                    Use();
                }
            }
        }

        private void TryEnableEquipBtn()
        {
            Item item;
            bool hasAnItem = SelectedItemSlot.transform.Find("Item").TryGetComponent<Item>(out item);
            if (hasAnItem)
            {
                if (item.IsEquipable)
                {
                    EquipBtn.GetComponent<Button>().enabled = true;
                    EquipBtn.GetComponent<Image>().enabled = true;
                    return;
                }
            }
            EquipBtn.GetComponent<Button>().enabled = false;
            EquipBtn.GetComponent<Image>().enabled = false;
        }

        private void TryEnableUnequipBtn()
        {
            Item item;
            bool hasAnItem = SelectedEquipmentSlot.transform.Find("Item").TryGetComponent<Item>(out item);
            if (hasAnItem)
            {
                UnequipBtn.GetComponent<Button>().enabled = true;
                UnequipBtn.GetComponent<Image>().enabled = true;
            }
            else
            {
                UnequipBtn.GetComponent<Button>().enabled = false;
                UnequipBtn.GetComponent<Image>().enabled = false;
            }
        }

        private void TryEnableUseBtn()
        {
            Item item;
            bool hasAnItem = SelectedItemSlot.transform.Find("Item").TryGetComponent<Item>(out item);
            if (hasAnItem)
            {
                if (item.IsUseable)
                {
                    UseBtn.GetComponent<Button>().enabled = true;
                    UseBtn.GetComponent<Image>().enabled = true;
                    return;
                }
            }
            UseBtn.GetComponent<Button>().enabled = false;
            UseBtn.GetComponent<Image>().enabled = false;
        }

        private void TryEnableDropBtn()
        {
            Item item;
            bool hasAnItem = SelectedItemSlot.transform.Find("Item").TryGetComponent<Item>(out item);
            if (hasAnItem)
            {
                DropBtn.GetComponent<Button>().enabled = true;
                DropBtn.GetComponent<Image>().enabled = true;
            }
            else
            {
                DropBtn.GetComponent<Button>().enabled = false;
                DropBtn.GetComponent<Image>().enabled = false;
            }
        }

        private void DisableEquipBtn()
        {
            EquipBtn.GetComponent<Button>().enabled = false;
            EquipBtn.GetComponent<Image>().enabled = false;
        }

        private void DisableUnequipBtn()
        {
            UnequipBtn.GetComponent<Button>().enabled = false;
            UnequipBtn.GetComponent<Image>().enabled = false;
        }

        private void DisableUseBtn()
        {
            UseBtn.GetComponent<Button>().enabled = false;
            UseBtn.GetComponent<Image>().enabled = false;
        }

        private void DisableDropBtn()
        {
            DropBtn.GetComponent<Button>().enabled = false;
            DropBtn.GetComponent<Image>().enabled = false;
        }

        //Được gọi bởi btn
        public void Equip()
        {
            Item item = SelectedItemSlot.transform.Find("Item").GetComponent<Item>();
            equipmentSlotManager.AddItemToEquipmentSlot(item.GetType());
            item.EquipItem();
            itemSlotManager.RemoveItemFromInventorySlot(SelectedItemSlot, 1);

            if (SelectedSlotIsEmpty())
            {
                DisableEquipBtn();
                DisableDropBtn();
            }
            DisableUnequipBtn();
            DisableUseBtn();
        }

        //Được gọi bởi btn
        public void Unequip()
        {
            // Thêm item vào inventory
            var item = SelectedEquipmentSlot.transform.Find("Item").GetComponent<Item>();
            itemSlotManager.AddItemToInventorySlot(item.GetType(), 1);
            // Xóa item trong equipment
            equipmentSlotManager.RemoveItemFromEquipmentSlot(SelectedEquipmentSlot);
            item.UnequipItem();

            DisableUnequipBtn();
        }

        //Được gọi bởi btn
        public void Use()
        {
            Item item = SelectedItemSlot.transform.Find("Item").GetComponent<Item>();
            item.UseItem();
            Drop();

            if (SelectedSlotIsEmpty())
            {
                DisableUseBtn();
                DisableDropBtn();
            }
            DisableEquipBtn();
            DisableUnequipBtn();
        }

        //Được gọi bởi btn
        public void Drop()
        {
            itemSlotManager.RemoveItemFromInventorySlot(SelectedItemSlot, 1);

            if (SelectedSlotIsEmpty())
            {
                DisableDropBtn();
                DisableUseBtn();
            }
            DisableEquipBtn();
            DisableUnequipBtn();
        }

        private bool SelectedSlotIsEmpty()
        {
            return SelectedItemSlot.transform.Find("Amount").GetComponent<Text>().text == "" ? true : false;
        }
    }
}