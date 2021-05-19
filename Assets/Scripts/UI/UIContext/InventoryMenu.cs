using Assets.Scripts.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
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

        public GameObject EquipItemBtn;
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
            UseItemBtn = transform.Find("InventoryUI").Find("EquipBtn").gameObject;
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

            EquipItemBtn.gameObject.GetComponent<Image>().enabled = false;
            EquipItemBtn.gameObject.GetComponent<Button>().enabled = false;

            UseItemBtn.gameObject.GetComponent<Image>().enabled = false;
            UseItemBtn.gameObject.GetComponent<Button>().enabled = false;

            DropItemBtn.gameObject.GetComponent<Image>().enabled = false;
            DropItemBtn.gameObject.GetComponent<Button>().enabled = false;

            #endregion Tắt nút dùng và nút vứt item

            InventoryUI.SetActive(false);
            uController = GetComponent<UIController>();

            SceneManager.sceneUnloaded += SaveInventory;
            LoadInventory();
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
                        UpdateItemAmountDisplay();
                        return true;
                    }
                }
                //thêm item vào slot
                slotDatum.Add(new SlotData { Item = item, Type = type, Amount = amount });

                AddItemImgToBtn(slotDatum.Count - 1, item.sprite);
                EnableItemImgDisplay(slotDatum.Count - 1, true);
                selectedIndex = slotDatum.Count - 1;
                UpdateItemAmountDisplay();
                selectedIndex = -1;

                return true;
            }
            //Inventory is full!;
            return false;
        }

        private void AddItemImgToBtn(int btnIdex, Sprite img)
        {
            GameObject itemImageObj = slotBtns[btnIdex].transform.Find("ItemImg").gameObject;
            var imgComp = itemImageObj.GetComponent<Image>();
            imgComp.sprite = img;

            var width = slotDatum[btnIdex].Item.ImageWidth;
            var height = slotDatum[btnIdex].Item.ImageHeight;
            var rectTransformComp = itemImageObj.GetComponent<RectTransform>();
            rectTransformComp.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rectTransformComp.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        //Được gọi qua Hàm Onclick của Button
        public void DropSelectedItem()
        {
            if (slotDatum[selectedIndex].Amount > 1)
            {
                slotDatum[selectedIndex].Amount--;
                UpdateItemAmountDisplay();
                return;
            }
            slotDatum.RemoveAt(selectedIndex);
            EnableItemImgDisplay(selectedIndex, false);
            selectedIndex = -1;         //sau khi Remove 1 itemBucket cần gán giá trị -1 cho selectedIndex để tắt nút Equip, Use và Drop và update các phần hiển thị

            UpdateEquipBtn();
            UpdateUseBtn();
            UpdateDropBtn();

            UpdateItemAmountDisplay();
            UpdateItemDescriptionTextDisplay();
        }

        //Được gọi qua Hàm Onclick của Button
        public void UseSelectedItem()
        {
            slotDatum[selectedIndex].Item.Use();
            DropSelectedItem();
        }

        //Được gọi qua Hàm Onclick của Button
        public void EquipSelectedItem()
        {
            //TO DO: implement this method
        }

        //Được gọi qua Hàm Onclick của Button
        public void SelectedItem(int index)
        {
            selectedIndex = index;
            UpdateEquipBtn();
            UpdateUseBtn();
            UpdateDropBtn();
            UpdateItemDescriptionTextDisplay();
        }

        private void EnableItemImgDisplay(int index, bool enable)
        {
            GameObject itemImage = slotBtns[index].transform.Find("ItemImg").gameObject;
            var imgComp = itemImage.GetComponent<Image>();
            imgComp.enabled = enable;
        }

        private void UpdateItemAmountDisplay()
        {
            if (selectedIndex < slotDatum.Count && selectedIndex >= 0)
            {
                GameObject itemAmount = slotBtns[selectedIndex].transform.Find("Amount").gameObject;
                var textComp = itemAmount.GetComponent<Text>();
                textComp.enabled = true;
                textComp.text = slotDatum[selectedIndex].Amount.ToString();
            }
            else
            {
                GameObject itemAmount = slotBtns[selectedIndex].transform.Find("Amount").gameObject;
                var textComp = itemAmount.GetComponent<Text>();
                textComp.enabled = false;
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

        private void UpdateEquipBtn()
        {
            if (selectedIndex < slotDatum.Count && selectedIndex >= 0)
            {
                if (slotDatum[selectedIndex].Item.IsEquipable)
                {
                    EquipItemBtn.gameObject.GetComponent<Image>().enabled = true;
                    EquipItemBtn.gameObject.GetComponent<Button>().enabled = true;
                }
                else
                {
                    EquipItemBtn.gameObject.GetComponent<Image>().enabled = false;
                    EquipItemBtn.gameObject.GetComponent<Button>().enabled = false;
                }
            }
            else
            {
                EquipItemBtn.gameObject.GetComponent<Image>().enabled = false;
                EquipItemBtn.gameObject.GetComponent<Button>().enabled = false;
            }
        }

        private void UpdateUseBtn()
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
            }
            else
            {
                UseItemBtn.gameObject.GetComponent<Image>().enabled = false;
                UseItemBtn.gameObject.GetComponent<Button>().enabled = false;
            }
        }

        private void UpdateDropBtn()
        {
            if (selectedIndex < slotDatum.Count && selectedIndex >= 0)
            {
                DropItemBtn.gameObject.GetComponent<Image>().enabled = true;
                DropItemBtn.gameObject.GetComponent<Button>().enabled = true;
            }
            else
            {
                DropItemBtn.gameObject.GetComponent<Image>().enabled = false;
                DropItemBtn.gameObject.GetComponent<Button>().enabled = false;
            }
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