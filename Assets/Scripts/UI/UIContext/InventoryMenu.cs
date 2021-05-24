using Assets.Scripts.UI.UIContext.InventorySystem;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI.UIContext
{
    public class InventoryMenu : UI
    {
        private GameObject InventoryUI;
        private UIController uController;

        private ItemSlotManager itemSlotManager;
        private EquipmentSlotManager equipmentSlotManager;

        private void Start()
        {
            InventoryUI = transform.Find("InventoryUI").gameObject;
            InventoryUI.SetActive(false);
            uController = GetComponent<UIController>();

            itemSlotManager = InventoryUI.transform.Find("Items").GetComponent<ItemSlotManager>();
            equipmentSlotManager = InventoryUI.transform.Find("Equipments").GetComponent<EquipmentSlotManager>();
        }

        private void OnDestroy()
        {
            itemSlotManager.SaveSlotData();
            equipmentSlotManager.SaveSlotData();
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