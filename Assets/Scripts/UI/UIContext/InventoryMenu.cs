using Assets.Scripts.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI.UIContext
{
    public class InventoryMenu : UI
    {
        [Serializable]
        private class SlotData
        {
            [NonSerialized]
            public Item Item;

            public ItemType Type = ItemType.SmallHealthPotion;

            public int Amount { get; set; }

            public SlotData()
            {
            }

            public bool Contain(Item item)
            {
                return this.Item.GetType().Name == item.GetType().Name;
            }
        }

        public GameObject UseItemBtn;
        public GameObject DropItemBtn;
        public GameObject InventoryUI;
        public GameObject ItemDescriptionText;
        public GameObject SlotBtnParent;//game object mà chứa các nút item

        private UIController uController;

        private GameObject[] slotBtns = new GameObject[45];//các nút item(có tất cả 45 nút)

        private const int maxSlot = 45;

        private List<SlotData> slotDatum = new List<SlotData>();//Datum là số nhiều của Data :v
        private int selectedIndex = -1;

        private void Start()
        {
            UseItemBtn = transform.Find("InventoryUI").Find("UseBtn").gameObject;
            DropItemBtn = transform.Find("InventoryUI").Find("DropBtn").gameObject;
            InventoryUI = transform.Find("InventoryUI").gameObject;
            ItemDescriptionText = transform.Find("InventoryUI").Find("ItemDescription").Find("Text").gameObject;
            SlotBtnParent = transform.Find("InventoryUI").Find("Items").Find("SlotBtnParent").gameObject;

            #region Lấy tất cả các nút slot

            for (int i = 0; i < SlotBtnParent.transform.childCount; i++)
            {
                slotBtns[i] = SlotBtnParent.transform.GetChild(i).gameObject;
            }

            #endregion Lấy tất cả các nút slot

            #region Tắt nút dùng và nút vứt item

            UseItemBtn.gameObject.GetComponent<Image>().enabled = false;
            UseItemBtn.gameObject.GetComponent<Button>().enabled = false;

            DropItemBtn.gameObject.GetComponent<Image>().enabled = false;
            DropItemBtn.gameObject.GetComponent<Button>().enabled = false;

            #endregion Tắt nút dùng và nút vứt item

            InventoryUI.SetActive(false);
            uController = GetComponent<UIController>();

            SceneManager.sceneUnloaded += SaveInventory;
            LoadInventory();
            for (int i = 0; i < slotDatum.Count; i++)
            {
                UpdateItemAmountDisplay(i);
            }
        }

        public void SaveInventory(Scene current)
        {
            string path = @"Assets\Data\Save\inventorydata.json";
            string saveData = JsonConvert.SerializeObject(slotDatum, Formatting.Indented);

            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(saveData);
            }
        }

        private void LoadInventory()
        {
            string path = @"Assets\Data\Save\inventorydata.json";
            if (!File.Exists(path))
            {
                return;
            }
            string saveData = File.ReadAllText(path);

            var tempSlot = JsonConvert.DeserializeObject<List<SlotData>>(saveData);
            foreach (var slot in tempSlot)
            {
                switch (slot.Type)
                {
                    case ItemType.SmallHealthPotion:
                        var item = ScriptableObject.CreateInstance<SmallHealthPotion>();
                        AddItem(item, slot.Type, slot.Amount);
                        break;

                    case ItemType.BigHealthPotion:
                        break;

                    default:
                        break;
                }
            }
        }

        //tăng biến amount của slot data nếu item đã có tồn tại 1 slot data của item đó. nếu không tồn tại thì tạo 1 slot data
        public bool AddItem(Item item, ItemType type, int amount)
        {
            //nên check lại điều kiện < có thể bị sai
            if (slotDatum.Count < maxSlot)
            {
                //check từng slot data xem có tồn tại item được truyền vào hay chưa. nếu rồi thì tăng amount của slot data
                for (int i = 0; i < slotDatum.Count; i++)
                {
                    if (slotDatum[i].Contain(item))
                    {
                        slotDatum[i].Amount += amount;
                        UpdateItemAmountDisplay(i);
                        return true;
                    }
                }
                //thêm item vào slot
                slotDatum.Add(new SlotData { Item = item, Type = type, Amount = amount });

                #region tạo phần hiển thị hình ảnh item

                GameObject itemImageObj = slotBtns[slotDatum.Count - 1].transform.Find("ItemImg").gameObject;

                var imgComp = itemImageObj.GetComponent<Image>();
                Sprite sprite = slotDatum[slotDatum.Count - 1].Item.sprite;
                imgComp.sprite = sprite;

                var width = slotDatum[slotDatum.Count - 1].Item.ImageWidth;
                var height = slotDatum[slotDatum.Count - 1].Item.ImageHeight;
                var rectTransformComp = itemImageObj.GetComponent<RectTransform>();
                rectTransformComp.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                rectTransformComp.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

                UpdateItemImgDisplay(slotDatum.Count - 1);

                #endregion tạo phần hiển thị hình ảnh item

                return true;
            }
            //Inventory is full!;
            return false;
        }

        private void UpdateItemAmountDisplay(int slotIndex)
        {
            GameObject itemAmount = slotBtns[slotIndex].transform.Find("Amount").gameObject;

            var textComp = itemAmount.GetComponent<Text>();
            if (slotDatum[slotIndex].Amount == 0)
            {
                textComp.enabled = false;
            }
            else
            {
                textComp.enabled = true;
                textComp.text = slotDatum[slotIndex].Amount.ToString();
            }
        }

        private void UpdateItemDescriptionTextDisplay()
        {
            var textComp = ItemDescriptionText.GetComponent<Text>();
            if (selectedIndex < slotDatum.Count && selectedIndex >= 0)
            {
                textComp.text = slotDatum[selectedIndex].Item.Description;
            }
            else
            {
                textComp.text = "";
            }
        }

        private void UpdateItemImgDisplay(int slotIndex)
        {
            GameObject itemImage = slotBtns[slotIndex].transform.Find("ItemImg").gameObject;

            var imgComp = itemImage.GetComponent<Image>();
            if (slotDatum[slotIndex].Amount == 0)
            {
                imgComp.enabled = false;
            }
            else
            {
                imgComp.enabled = true;
            }
        }

        private void UpdateUseAndDropBtn()
        {
            if (selectedIndex < slotDatum.Count && selectedIndex >= 0)
            {
                if (slotDatum[selectedIndex].Item.IsUseable)
                {
                    UseItemBtn.gameObject.GetComponent<Image>().enabled = true;
                    UseItemBtn.gameObject.GetComponent<Button>().enabled = true;
                }
                else
                {
                    UseItemBtn.gameObject.GetComponent<Image>().enabled = false;
                    UseItemBtn.gameObject.GetComponent<Button>().enabled = false;
                }
                DropItemBtn.gameObject.GetComponent<Image>().enabled = true;
                DropItemBtn.gameObject.GetComponent<Button>().enabled = true;
            }
            else
            {
                UseItemBtn.gameObject.GetComponent<Image>().enabled = false;
                UseItemBtn.gameObject.GetComponent<Button>().enabled = false;

                DropItemBtn.gameObject.GetComponent<Image>().enabled = false;
                DropItemBtn.gameObject.GetComponent<Button>().enabled = false;
            }
        }

        //Được gọi qua Hàm Onclick của Button
        public void DropSelectedItem()
        {
            if (slotDatum[selectedIndex].Amount > 1)
            {
                slotDatum[selectedIndex].Amount--;
                UpdateItemAmountDisplay(selectedIndex);
                return;
            }
            slotDatum[selectedIndex].Amount--;
            UpdateItemAmountDisplay(selectedIndex);
            UpdateItemImgDisplay(selectedIndex);
            slotDatum.RemoveAt(selectedIndex);
            //sau khi Remove 1 itemBucket cần gán giá trị -1 cho selectedIndex để hàm UpdateUseAndDropBtn tắt nút Use và Drop
            selectedIndex = -1;
            UpdateUseAndDropBtn();
        }

        //Được gọi qua Hàm Onclick của Button
        public void UseSelectedItem()
        {
            slotDatum[selectedIndex].Item.Use();
            DropSelectedItem();
        }

        //Được gọi qua Hàm Onclick của Button
        public void SelectedItem(int index)
        {
            selectedIndex = index;
            UpdateUseAndDropBtn();
            UpdateItemDescriptionTextDisplay();
        }

        #region Điều khiển UI

        public override void Enter()
        {
            NewUI = this;
            InventoryUI.SetActive(true);
            Time.timeScale = 0;
        }

        public override void Exit()
        {
            InventoryUI.SetActive(false);
            Time.timeScale = 1;
        }

        public override void LogicUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.I))
            {
                NewUI = uController.None;
            }
        }

        #endregion Điều khiển UI
    }
}