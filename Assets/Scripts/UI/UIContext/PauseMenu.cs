using Assets.Scripts.Player;
using Assets.Scripts.UI;
using Assets.Scripts.UI.UIContext.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI.UIContext
{
    public class PauseMenu : UI
    {
        public GameObject pauseUI;
        private UIController uController;
        private GameObject player;
        private PlayerData playerData;
        private ItemSlotManager itemSlotManager;
        private EquipmentSlotManager equipmentSlotManager;

        // Use this for initialization
        private void Start()
        {
            pauseUI.SetActive(false);
            uController = GetComponent<UIController>();
            player = GameObject.Find("Player");
            //lấy PlayerData component
            playerData = player.GetComponent<PlayerData>();

            itemSlotManager = transform.Find("InventoryUI").Find("Items").GetComponent<ItemSlotManager>();
            equipmentSlotManager = transform.Find("InventoryUI").Find("Equipments").GetComponent<EquipmentSlotManager>();
        }

        // Update is called once per frame
        public override void LogicUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                NewUI = uController.None;
            }
        }

        public void Resume()
        {
            NewUI = uController.None;
        }

        public void Restart()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().path);
        }

        public void BackToMainMenu()
        {
            SceneManager.LoadScene(0);
        }

        public override void Enter()
        {
            NewUI = this;
            pauseUI.SetActive(true);
            Time.timeScale = 0;
            //nếu player hết máu thì tắt Image và Button component
            if (playerData.currentHealth <= 0)
            {
                var resumeBtn = pauseUI.transform.Find("ResumeButton");
                resumeBtn.GetComponent<Image>().enabled = false;
                resumeBtn.GetComponent<Button>().enabled = false;

                playerData.currentHealth = playerData.maxHealth;
            }
            else
            {
                var resumeBtn = pauseUI.transform.Find("ResumeButton");
                resumeBtn.GetComponent<Image>().enabled = true;
                resumeBtn.GetComponent<Button>().enabled = true;
            }
        }

        public override void Exit()
        {
            pauseUI.SetActive(false);
            Time.timeScale = 1;
        }
    }
}