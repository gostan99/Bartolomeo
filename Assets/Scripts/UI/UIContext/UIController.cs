using Assets.Scripts.UI.UIContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIController : MonoBehaviour
    {
        #region UI

        public UI currentUI { get; set; }
        public UI None { get; set; }
        public UI PauseMenu { get; set; }
        public UI InventoryMenu { get; set; }

        #endregion UI

        private void Start()
        {
            None = GetComponent<NoneUI>();
            currentUI = None;
            PauseMenu = GetComponent<PauseMenu>();
            InventoryMenu = GetComponent<InventoryMenu>();
        }

        private void Update()
        {
            currentUI.LogicUpdate();
            if (currentUI != currentUI.GetNewUI())
            {
                currentUI.Exit();
                currentUI = currentUI.GetNewUI();
                currentUI.Enter();
            }
        }
    }
}