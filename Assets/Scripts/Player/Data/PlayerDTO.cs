using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.Scripts.Player.PlayerData;

namespace Assets.Scripts.Player
{
    [Serializable]
    public class PlayerDTO
    {
        public PlayerSerializeData playerSerializeData;
        public string Level;
    }
}