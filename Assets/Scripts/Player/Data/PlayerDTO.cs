using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Player
{
    [Serializable]
    public class PlayerDTO
    {
        public bool HasDash;
        public int MaxJumpCounter;
        public string Level;
    }
}