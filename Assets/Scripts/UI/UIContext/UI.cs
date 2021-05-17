using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public abstract class UI : MonoBehaviour
    {
        protected UI NewUI { get; set; }

        public UI()
        {
            NewUI = this;
        }

        public abstract void Enter();

        public abstract void LogicUpdate();

        public abstract void Exit();

        public UI GetNewUI()
        {
            return NewUI;
        }

        public void SetNewUI(UI ui)
        {
            NewUI = ui;
        }
    }
}