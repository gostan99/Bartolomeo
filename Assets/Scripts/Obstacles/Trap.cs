using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Obstacles
{
    class Trap : MonoBehaviour
    {
        private Transform TrapPos;                          
        public Transform TrapRange;
        public float DamgeTrap = 100f;
    }
}
