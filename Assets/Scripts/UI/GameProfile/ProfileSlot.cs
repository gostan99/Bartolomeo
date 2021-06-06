using Assets.Scripts.Player;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI.GameProfile
{
    public class ProfileSlot : MonoBehaviour
    {
        private GameObject[] slots = new GameObject[3];
        private string savePlayerDataPath = @"Assets\Data\Save\playerdata";
        private string saveItemPath = @"Assets\Data\Save\inventoryItemData";
        private string saveEquipmentPath = @"Assets\Data\Save\inventoryEpuipmentData";

        private void Start()
        {
            UpdateSlot();
        }

        public void UpdateSlot()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                slots[i] = transform.GetChild(i).gameObject;
                string slot = (i + 1).ToString();
                savePlayerDataPath += slot + ".json";
                saveItemPath += slot + ".json";
                saveEquipmentPath += slot + ".json";
                if (!File.Exists(savePlayerDataPath) && !File.Exists(saveItemPath) && !File.Exists(saveEquipmentPath))
                {
                    //tắt contents
                    slots[i].transform.Find("Contents").gameObject.SetActive(false);
                }
                else
                {
                    //Thiết lập nội dung
                    string json = File.ReadAllText(savePlayerDataPath);
                    PlayerDTO dto = JsonConvert.DeserializeObject<PlayerDTO>(json);
                    var contents = slots[i].transform.Find("Contents").gameObject;
                    var level = contents.transform.Find("Level").GetComponent<Text>();
                    var money = contents.transform.Find("Money").GetComponent<Text>();
                    var playtime = contents.transform.Find("Playtime").GetComponent<Text>();

                    level.text = "LEVEL: " + dto.SceneIndex.ToString();
                    money.text = dto.playerSerializeData.Money.ToString();
                    var hours = TimeSpan.FromSeconds(dto.Playtime).Hours;
                    var minutes = TimeSpan.FromSeconds(dto.Playtime).Minutes;
                    playtime.text = $"Playtime: {hours}h {minutes}m";
                }
            }
        }
    }
}