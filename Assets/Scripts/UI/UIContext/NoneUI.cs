using Assets.Scripts.Player;
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
        private GameObject player;
        private PlayerData playerData;

        private void Start()
        {
            //tìm kiếm Player trong scene
            player = GameObject.Find("Player");
            //lấy PlayerData component
            playerData = player.GetComponent<PlayerData>();
            uController = GetComponent<UIController>();
        }

        public override void LogicUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                NewUI = uController.PauseMenu;
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                NewUI = uController.InventoryMenu;
            }
            else if (playerData.currentHealth <= 0)
            {
                NewUI = uController.PauseMenu;
            }
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