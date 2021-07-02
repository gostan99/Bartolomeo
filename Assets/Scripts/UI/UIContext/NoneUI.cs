using Assets.Scripts.Player;
using Assets.Scripts.UI.UIContext.InventorySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI.UIContext
{
    public class NoneUI : UI
    {
        private UIController uController;
        private PlayerInput pInput;
        private ItemSlotManager itemSlotManager;
        private GameObject player;
        private PlayerController pController;
        private PlayerData playerData;

        private Buttons buttons;

        private GameObject InventoryUI;

        private void Start()
        {
            //tìm kiếm Player trong scene
            player = GameObject.Find("Player");
            //lấy PlayerData component
            playerData = player.GetComponent<PlayerData>();
            pController = player.GetComponent<PlayerController>();
            uController = GetComponent<UIController>();

            InventoryUI = transform.Find("InventoryUI").gameObject;
            InventoryUI.SetActive(true);

            pInput = GameObject.Find("Player").GetComponent<PlayerController>().pInput;
            itemSlotManager = InventoryUI.transform.Find("Items").GetComponent<ItemSlotManager>();
            buttons = InventoryUI.transform.Find("Buttons").GetComponent<Buttons>();
        }

        public override void LogicUpdate()
        {
            if (playerData.currentHealth <= 0)
            {
                if (pController.DeathState.AnimationIsFinished)
                {
                    NewUI = uController.PauseMenu;
                }
                return;
            }

            if (InventoryUI.activeSelf)
            {
                InventoryUI.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                NewUI = uController.PauseMenu;
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                NewUI = uController.InventoryMenu;
            }

            if (pInput == null) pInput = GameObject.Find("Player").GetComponent<PlayerController>().pInput;

            if (pInput.LargePotionInput)
            {
                InventoryUI.SetActive(true);
                var SelectedItemSlot = buttons.SelectedItemSlot;
                if (itemSlotManager.TryGetLargePotionItemSlot(out SelectedItemSlot))
                {
                    buttons.SelectedItemSlot = SelectedItemSlot;
                    buttons.Use();
                }
                InventoryUI.SetActive(false);
            }
            else if (pInput.ManaPotionInput)
            {
                InventoryUI.SetActive(true);
                var SelectedItemSlot = buttons.SelectedItemSlot;
                if (itemSlotManager.TryGetManaPotionItemSlot(out SelectedItemSlot))
                {
                    buttons.SelectedItemSlot = SelectedItemSlot;
                    buttons.Use();
                }
                InventoryUI.SetActive(false);
            };
        }

        public override void Enter()
        {
            NewUI = this;
        }

        public override void Exit()
        {
        }
    }
}