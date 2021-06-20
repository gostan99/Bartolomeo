using Assets.Scripts.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.UIContext.InventorySystem
{
    public class ItemSlotManager : MonoBehaviour
    {
        [Serializable]
        private struct SlotData
        {
            public Type ItemType { get; set; }
            public int Amount { get; set; }
        }

        private List<SlotData> SlotDataList;
        private string selectedProfile;
        private string selectedProfilePath = @"Assets\Data\Save\selectedProfile.txt";
        private string savePath = @"Assets\Data\Save\inventoryItemData";
        private bool isLoadingData = false;

        protected GameObject[] ItemSlots = new GameObject[45];
        private int lastSlotHasAnItem = -1;

        private void Start()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                ItemSlots[i] = transform.GetChild(i).gameObject;
            }

            SlotDataList = new List<SlotData>();
            if (File.Exists(selectedProfilePath))
            {
                selectedProfile = File.ReadAllText(selectedProfilePath);
                savePath += selectedProfile + ".json";
            }
            LoadSavedSlotData();
        }

        public void EraseSavedSlotData()
        {
            SlotDataList = new List<SlotData>();
        }

        public void SaveSlotData()
        {
            string jsonData = JsonConvert.SerializeObject(SlotDataList, Formatting.Indented);

            using (StreamWriter writer = new StreamWriter(savePath))
            {
                writer.Write(jsonData);
            }
        }

        private void LoadSavedSlotData()
        {
            if (!File.Exists(savePath))
            {
                return;
            }
            isLoadingData = true;
            string jsonData = File.ReadAllText(savePath);
            SlotDataList = JsonConvert.DeserializeObject<List<SlotData>>(jsonData);
            foreach (var slotData in SlotDataList)
            {
                AddItemToInventorySlot(slotData.ItemType, slotData.Amount);
            }
            isLoadingData = false;
        }

        public bool TryGetLargePotionItemSlot(out GameObject largePotionItemSlot)
        {
            GameObject slot = new GameObject();
            largePotionItemSlot = slot;

            for (int i = 0; i < ItemSlots.Length; i++)
            {
                slot = ItemSlots[i];
                if (slot.transform.Find("Item").TryGetComponent<LargeHealthPotion>(out _))
                {
                    largePotionItemSlot = slot;
                    return true;
                }
            }
            return false;
        }

        public bool TryGetSmallPotionItemSlot(out GameObject smallPotionItemSlot)
        {
            GameObject slot = new GameObject();
            smallPotionItemSlot = slot;

            for (int i = 0; i < ItemSlots.Length; i++)
            {
                slot = ItemSlots[i];
                if (slot.transform.Find("Item").TryGetComponent<SmallHealthPotion>(out _))
                {
                    smallPotionItemSlot = slot;
                    return true;
                }
            }
            return false;
        }

        public void AddItemToInventorySlot(Type itemType, int amount)
        {
            Component item;
            bool hasAnSpecificItem;
            bool hasAnyItem;
            //Duyệt hết các slot, nếu đã tồn tại 1 item thuộc kiểu của class này tăng số lương lên thôi
            for (int i = 0; i < ItemSlots.Length; i++)
            {
                hasAnyItem = ItemSlots[i].transform.Find("Item").TryGetComponent(typeof(Item), out item);
                if (hasAnyItem)
                {
                    hasAnSpecificItem = ItemSlots[i].transform.Find("Item").TryGetComponent(itemType, out item);
                    if (hasAnSpecificItem)
                    {
                        int currentAmount = Convert.ToInt32(ItemSlots[i].transform.Find("Amount").GetComponent<Text>().text);
                        currentAmount += amount;
                        ItemSlots[i].transform.Find("Amount").GetComponent<Text>().text = currentAmount.ToString();

                        int index = SlotDataList.FindIndex(x => x.ItemType == itemType);
                        SlotDataList[index] = new SlotData { ItemType = itemType, Amount = currentAmount };
                        return;
                    }
                }
                else
                {
                    lastSlotHasAnItem = i - 1;
                    break;
                }
            }
            //Nếu chưa tồn tại thì thêm item vào trước slot cuối cùng có item

            //Thêm item
            item = ItemSlots[lastSlotHasAnItem + 1].transform.Find("Item").gameObject.AddComponent(itemType);
            //Thêm hình
            var itemImg = ItemSlots[lastSlotHasAnItem + 1].transform.Find("ItemImg").GetComponent<Image>();
            itemImg.sprite = ((Item)item).Sprite;
            itemImg.enabled = true;
            //Sửa kích thước hình
            var width = ((Item)item).ImageWidth;
            var height = ((Item)item).ImageHeight;
            var rectTransformComp = ItemSlots[lastSlotHasAnItem + 1].transform.Find("ItemImg").GetComponent<RectTransform>();
            rectTransformComp.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rectTransformComp.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            //Thêm số lượng
            ItemSlots[lastSlotHasAnItem + 1].transform.Find("Amount").GetComponent<Text>().text = amount.ToString();

            //thêm giữ liệu vào serializeDat
            if (isLoadingData)
            {
                return;
            }
            SlotData slotData = new SlotData { ItemType = itemType, Amount = amount };
            SlotDataList.Add(slotData);
        }

        public void RemoveItemFromInventorySlot(GameObject SelectedItemSlot, int amount)
        {
            //Giảm amount hoặc loại bỏ item trong item slot nếu amount <= 0
            Item item = SelectedItemSlot.transform.Find("Item").GetComponent<Item>();
            int currentAmount = Convert.ToInt32(SelectedItemSlot.transform.Find("Amount").GetComponent<Text>().text);
            currentAmount -= amount;
            if (currentAmount <= 0)
            {
                SelectedItemSlot.transform.Find("Amount").GetComponent<Text>().text = "";
                SelectedItemSlot.transform.Find("ItemImg").GetComponent<Image>().enabled = false;

                var slotData = SlotDataList.Find(x => x.ItemType == item.GetType());
                SlotDataList.Remove(slotData);

                Destroy(item);
            }
            else
            {
                SelectedItemSlot.transform.Find("Amount").GetComponent<Text>().text = currentAmount.ToString();

                int index = SlotDataList.FindIndex(x => x.ItemType == item.GetType());
                SlotDataList[index] = new SlotData { ItemType = item.GetType(), Amount = currentAmount };
            }
        }
    }
}