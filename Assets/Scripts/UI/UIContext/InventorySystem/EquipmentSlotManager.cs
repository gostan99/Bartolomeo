using Assets.Scripts.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI.UIContext.InventorySystem
{
    public class EquipmentSlotManager : MonoBehaviour
    {
        [Serializable]
        private struct SlotData
        {
            public Type ItemType;
            public int Amount;
        }

        private List<SlotData> SlotDataList;
        private const string savePath = @"Assets\Data\Save\inventoryEpuipmentData.json";
        private bool isLoadingData = false;

        protected GameObject[] EquipmentSlots;

        private void Start()
        {
            EquipmentSlots = new GameObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                EquipmentSlots[i] = transform.GetChild(i).gameObject;
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
                AddItemToEquipmentSlot(slotData.ItemType);
            }
            isLoadingData = false;
        }

        public void AddItemToEquipmentSlot(Type itemType)
        {
            //Duyệt hết tất cả các slot, thấy slot nào trống thì gán item vào slot đó
            Image itemImg;
            for (int i = 0; i < EquipmentSlots.Length; i++)
            {
                itemImg = EquipmentSlots[i].transform.Find("ItemImg").GetComponent<Image>();
                if (!itemImg.enabled)
                {
                    //Thêm Item vào trang bị slot
                    var item = EquipmentSlots[i].transform.Find("Item").gameObject.AddComponent(itemType);

                    //Thêm hình
                    itemImg.sprite = ((Item)item).Sprite;
                    itemImg.enabled = true;

                    //Sửa kích thước hình
                    var width = ((Item)item).ImageWidth;
                    var height = ((Item)item).ImageHeight;
                    var rectTransformComp = itemImg.GetComponent<RectTransform>();
                    rectTransformComp.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                    rectTransformComp.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

                    //thêm giữ liệu vào serializeDat
                    if (isLoadingData)
                    {
                        return;
                    }
                    var slotData = new SlotData { ItemType = itemType, Amount = 1 };
                    SlotDataList.Add(slotData);
                    return;
                }
            }
        }

        public void RemoveItemFromEquipmentSlot(GameObject selectedEquipmentSlot)
        {
            var itemImg = selectedEquipmentSlot.transform.Find("ItemImg").GetComponent<Image>();
            itemImg.enabled = false;
            var item = selectedEquipmentSlot.transform.Find("Item").GetComponent<Item>();
            var slotData = SlotDataList.Find(x => x.ItemType == item.GetType());
            SlotDataList.Remove(slotData);
        }
    }
}