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
        public GameObject UnequipItemBtn;
        public GameObject UseItemBtn;
        public GameObject DropItemBtn;
        public GameObject InventoryUI;
        public GameObject ItemDescriptionText;
        public GameObject ItemSlotBtnParent;//game object mà chứa các nút item
        public GameObject EquipmentSlotBtnParent;//game object mà chứa các nút Epuipment

        private UIController uController;

        private const int maxItemSlot = 45;//các nút item(có tất cả 45 nút)
        private const int maxEpuipmentSlot = 3;//các nút trang bị(có tất cả 3 nút)

        private GameObject[] itemSlotBtns = new GameObject[maxItemSlot];
        private GameObject[] equipmentSlotBtns = new GameObject[maxEpuipmentSlot];

        private List<SlotData> itemSlotDatum = new List<SlotData>();//Datum là số nhiều của Data :v
        private List<SlotData> equipmentSlotDatum = new List<SlotData>();//Datum là số nhiều của Data :v
        private int selectedItemIndex = -1;
        private int selectedEquipmentIndex = -1;

        private void Start()
        {
            #region Lấy tất cả các nút slot

            for (int i = 0; i < ItemSlotBtnParent.transform.childCount; i++)
            {
                itemSlotBtns[i] = ItemSlotBtnParent.transform.GetChild(i).gameObject;
            }

            for (int i = 0; i < EquipmentSlotBtnParent.transform.childCount; i++)
            {
                equipmentSlotBtns[i] = EquipmentSlotBtnParent.transform.GetChild(i).gameObject;
            }

            #endregion Lấy tất cả các nút slot

            #region Tắt nút dùng và nút vứt item

            UnequipItemBtn.gameObject.GetComponent<Image>().enabled = false;
            UnequipItemBtn.gameObject.GetComponent<Button>().enabled = false;

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
            string inventoryItemDataPath = @"Assets\Data\Save\inventoryItemData.json";
            string inventoryEpuipmentDataPath = @"Assets\Data\Save\inventoryEpuipmentData.json";
            string inventoryItemData = JsonConvert.SerializeObject(itemSlotDatum, Formatting.Indented);
            string inventoryEpuipmentData = JsonConvert.SerializeObject(equipmentSlotDatum, Formatting.Indented);

            using (StreamWriter writer = new StreamWriter(inventoryItemDataPath))
            {
                writer.Write(inventoryItemData);
            }
            using (StreamWriter writer = new StreamWriter(inventoryEpuipmentDataPath))
            {
                writer.Write(inventoryEpuipmentData);
            }
        }

        private void LoadInventory()
        {
            string inventoryItemDataPath = @"Assets\Data\Save\inventoryItemData.json";
            string inventoryEpuipmentDataPath = @"Assets\Data\Save\inventoryEpuipmentData.json";
            if (!File.Exists(inventoryItemDataPath) || !File.Exists(inventoryEpuipmentDataPath))
            {
                return;
            }
            string inventoryItemData = File.ReadAllText(inventoryItemDataPath);
            string inventoryEpuipmentData = File.ReadAllText(inventoryEpuipmentDataPath);

            var itemSlots = JsonConvert.DeserializeObject<List<SlotData>>(inventoryItemData);
            var epuipmentSlots = JsonConvert.DeserializeObject<List<SlotData>>(inventoryEpuipmentData);
            foreach (var slot in itemSlots)
            {
                switch (slot.Type)
                {
                    case ItemType.SmallHealthPotion:
                        {
                            var item = ScriptableObject.CreateInstance<SmallHealthPotion>();
                            AddItem(item, slot.Type, slot.Amount);
                        }
                        break;

                    case ItemType.LargeHealthPotion:
                        break;

                    case ItemType.ManaPotion:
                        break;

                    default:
                        break;
                }
            }
            foreach (var slot in epuipmentSlots)
            {
                switch (slot.Type)
                {
                    case ItemType.GiantSword:
                        {
                            var item = ScriptableObject.CreateInstance<GiantSword>();
                            var slotData = new SlotData { Item = item, Type = slot.Type, Amount = 1 };
                            equipmentSlotDatum.Add(slotData);
                            var img = equipmentSlotBtns[equipmentSlotDatum.Count].transform.Find("ItemImg").gameObject;
                            var imgComp = img.GetComponent<Image>();
                            imgComp.sprite = item.sprite;
                        }
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
            if (itemSlotDatum.Count < maxItemSlot)
            {
                //check từng slot data xem có tồn tại item được truyền vào hay chưa. nếu rồi thì tăng amount của slot data
                for (int i = 0; i < itemSlotDatum.Count; i++)
                {
                    if (itemSlotDatum[i].Contain(item))
                    {
                        itemSlotDatum[i].Amount += amount;
                        UpdateItemAmount(i);
                        return true;
                    }
                }
                //thêm item vào slot
                itemSlotDatum.Add(new SlotData { Item = item, Type = type, Amount = amount });

                AddItemImgToBtn(itemSlotDatum.Count - 1, item.sprite);
                EnableItemImgDisplay(itemSlotDatum.Count - 1, true);
                EnableItemAmountDisplay(itemSlotDatum.Count - 1, true);
                UpdateItemAmount(itemSlotDatum.Count - 1);

                return true;
            }
            //Inventory is full!;
            return false;
        }

        private void AddItemImgToBtn(int btnIdex, Sprite img)
        {
            GameObject itemImageObj = itemSlotBtns[btnIdex].transform.Find("ItemImg").gameObject;
            var imgComp = itemImageObj.GetComponent<Image>();
            imgComp.sprite = img;

            var width = itemSlotDatum[btnIdex].Item.ImageWidth;
            var height = itemSlotDatum[btnIdex].Item.ImageHeight;
            var rectTransformComp = itemImageObj.GetComponent<RectTransform>();
            rectTransformComp.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rectTransformComp.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        //Được gọi qua Hàm Onclick của Button
        public void EquipSelectedItem()
        {
            itemSlotDatum[selectedItemIndex].Item.Equip();
            for (int i = 0; i < equipmentSlotBtns.Length; i++)
            {
                var img = equipmentSlotBtns[i].transform.Find("ItemImg").gameObject;
                var imgComp = img.GetComponent<Image>();
                if (!imgComp.enabled)
                {
                    imgComp.enabled = true;
                    imgComp.sprite = itemSlotDatum[selectedItemIndex].Item.sprite;
                    equipmentSlotDatum.Add(itemSlotDatum[selectedItemIndex]);
                    var item = equipmentSlotDatum[i].Item;

                    var width = item.ImageWidth;
                    var height = item.ImageHeight;
                    var rectTransformComp = img.GetComponent<RectTransform>();
                    rectTransformComp.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                    rectTransformComp.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

                    DropSelectedItem();
                    return;
                }
            }
        }

        //Được gọi qua Hàm Onclick của Button
        public void UnequipSelectedItem()
        {
            var img = equipmentSlotBtns[selectedEquipmentIndex].transform.Find("ItemImg").gameObject;
            var imgComp = img.GetComponent<Image>();
            imgComp.enabled = false;
            var item = equipmentSlotDatum[selectedEquipmentIndex].Item;
            var type = equipmentSlotDatum[selectedEquipmentIndex].Type;
            AddItem(item, type, 1);
            selectedEquipmentIndex = -1;
            UpdateUnequipBtn();
        }

        //Được gọi qua Hàm Onclick của Button
        public void UseSelectedItem()
        {
            itemSlotDatum[selectedItemIndex].Item.Use();
            DropSelectedItem();
        }

        //Được gọi qua Hàm Onclick của Button
        public void DropSelectedItem()
        {
            if (itemSlotDatum[selectedItemIndex].Amount > 1)
            {
                itemSlotDatum[selectedItemIndex].Amount--;
                UpdateItemAmount(selectedItemIndex);
                return;
            }
            itemSlotDatum.RemoveAt(selectedItemIndex);
            EnableItemImgDisplay(selectedItemIndex, false);
            EnableItemAmountDisplay(selectedItemIndex, false);
            selectedItemIndex = -1;         //sau khi Remove 1 itemBucket cần gán giá trị -1 cho selectedIndex để tắt nút Equip, Use và Drop và update các phần hiển thị

            UpdateEquipBtn();
            UpdateUseBtn();
            UpdateDropBtn();

            UpdateItemDescriptionTextDisplay();
        }

        //Được gọi qua Hàm Onclick của Button
        public void SelectedItem(int index)
        {
            selectedItemIndex = index;
            UpdateEquipBtn();
            UpdateUseBtn();
            UpdateDropBtn();
            UpdateItemDescriptionTextDisplay();
        }

        //Được gọi qua Hàm Onclick của Button
        public void SelectedEquipment(int index)
        {
            selectedItemIndex = -1;
            selectedEquipmentIndex = index;
            UpdateEquipBtn();
            UpdateUnequipBtn();
            UpdateUseBtn();
            UpdateDropBtn();
            UpdateItemDescriptionTextDisplay();
        }

        private void EnableItemImgDisplay(int index, bool enable)
        {
            GameObject itemImage = itemSlotBtns[index].transform.Find("ItemImg").gameObject;
            var imgComp = itemImage.GetComponent<Image>();
            imgComp.enabled = enable;
        }

        private void EnableItemAmountDisplay(int index, bool enable)
        {
            GameObject itemAmount = itemSlotBtns[index].transform.Find("Amount").gameObject;
            var textComp = itemAmount.GetComponent<Text>();
            textComp.enabled = enable;
        }

        private void UpdateItemAmount(int index)
        {
            if (index < itemSlotDatum.Count && index >= 0)
            {
                GameObject itemAmount = itemSlotBtns[index].transform.Find("Amount").gameObject;
                var textComp = itemAmount.GetComponent<Text>();
                textComp.enabled = true;
                textComp.text = itemSlotDatum[index].Amount.ToString();
            }
        }

        private void UpdateItemDescriptionTextDisplay()
        {
            var textComp = ItemDescriptionText.GetComponent<Text>();
            if (selectedItemIndex < itemSlotDatum.Count && selectedItemIndex >= 0)
            {
                textComp.text = itemSlotDatum[selectedItemIndex].Item.Description;
            }
            else
            {
                textComp.text = "";
            }
        }

        private void UpdateEquipBtn()
        {
            if (selectedItemIndex < itemSlotDatum.Count && selectedItemIndex >= 0 && equipmentSlotDatum.Count < maxEpuipmentSlot)
            {
                if (itemSlotDatum[selectedItemIndex].Item.IsEquipable)
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

        private void UpdateUnequipBtn()
        {
            if (selectedEquipmentIndex < equipmentSlotDatum.Count && selectedEquipmentIndex >= 0)
            {
                UnequipItemBtn.gameObject.GetComponent<Image>().enabled = true;
                UnequipItemBtn.gameObject.GetComponent<Button>().enabled = true;
            }
            else
            {
                UnequipItemBtn.gameObject.GetComponent<Image>().enabled = false;
                UnequipItemBtn.gameObject.GetComponent<Button>().enabled = false;
            }
        }

        private void UpdateUseBtn()
        {
            if (selectedItemIndex < itemSlotDatum.Count && selectedItemIndex >= 0)
            {
                if (itemSlotDatum[selectedItemIndex].Item.IsUseable)
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
            if (selectedItemIndex < itemSlotDatum.Count && selectedItemIndex >= 0)
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