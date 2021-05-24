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
        private const string savePath = @"Assets\Data\Save\inventoryItemData.json";
        private bool isLoadingData = false;

        protected GameObject[] ItemSlots;

        private void Start()
        {
            ItemSlots = new GameObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                ItemSlots[i] = transform.GetChild(i).gameObject;
            }

            SlotDataList = new List<SlotData>();
            LoadSavedSlotData();
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

        public void AddItemToInventorySlot(Type itemType, int amount)
        {
            Component item;
            bool hasAnItem;
            //Duyệt hết các slot, nếu đã tồn tại 1 item thuộc kiểu của class này tăng số lương lên thôi
            for (int i = 0; i < ItemSlots.Length; i++)
            {
                hasAnItem = ItemSlots[i].transform.Find("Item").TryGetComponent(itemType, out item);
                if (hasAnItem)
                {
                    int currentAmount = Convert.ToInt32(ItemSlots[i].transform.Find("Amount").GetComponent<Text>().text);
                    currentAmount += amount;
                    ItemSlots[i].transform.Find("Amount").GetComponent<Text>().text = currentAmount.ToString();

                    int index = SlotDataList.FindIndex(x => x.ItemType == itemType);
                    SlotDataList[index] = new SlotData { ItemType = itemType, Amount = currentAmount };
                    return;
                }
            }
            //Nếu chưa tồn tại thì kiếm slot nào trống để thêm item vào
            for (int i = 0; i < ItemSlots.Length; i++)
            {
                hasAnItem = ItemSlots[i].transform.Find("Item").TryGetComponent(typeof(Item), out item);
                if (!hasAnItem)
                {
                    item = ItemSlots[i].transform.Find("Item").gameObject.AddComponent(itemType);
                    //Thêm hình
                    var itemImg = ItemSlots[i].transform.Find("ItemImg").GetComponent<Image>();
                    itemImg.sprite = ((Item)item).Sprite;
                    itemImg.enabled = true;
                    //Sửa kích thước hình
                    var width = ((Item)item).ImageWidth;
                    var height = ((Item)item).ImageHeight;
                    var rectTransformComp = ItemSlots[i].transform.Find("ItemImg").GetComponent<RectTransform>();
                    rectTransformComp.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                    rectTransformComp.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                    //Thêm số lượng
                    ItemSlots[i].transform.Find("Amount").GetComponent<Text>().text = amount.ToString();

                    //thêm giữ liệu vào serializeDat
                    if (isLoadingData)
                    {
                        return;
                    }
                    var slotData = new SlotData { ItemType = itemType, Amount = amount };
                    SlotDataList.Add(slotData);

                    return;
                }
            }
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
                int index = SlotDataList.FindIndex(x => x.ItemType == item.GetType());
                SlotDataList[index] = new SlotData { ItemType = item.GetType(), Amount = currentAmount };

                SelectedItemSlot.transform.Find("Amount").GetComponent<Text>().text = currentAmount.ToString();
            }
        }
    }
}