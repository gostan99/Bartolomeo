using Assets.Scripts.Player;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI.GameProfile
{
    public class Buttons : MonoBehaviour
    {
        private string savePlayerDataPath = @"Assets\Data\Save\playerdata";
        private string saveItemPath = @"inventoryItemData";
        private string saveEquipmentPath = @"Assets\Data\Save\inventoryEpuipmentData";
        private string selectedProfilePath = @"selectedProfile.txt";
        private GameObject selectedProfile;
        private GameObject selectedObj;
        private ProfileSlot profileSlot;

        private GameObject DeleteBtn;
        private GameObject NewGameBtn;
        private GameObject LoadBtn;

        private void Start()
        {
            DeleteBtn = transform.Find("Delete").gameObject;
            NewGameBtn = transform.Find("NewGame").gameObject;
            LoadBtn = transform.Find("Load").gameObject;
            GameObject profileSlotObj = transform.parent.Find("ProfileSlots").gameObject;
            profileSlot = profileSlotObj.GetComponent<ProfileSlot>();

            LoadBtn.GetComponent<Button>().enabled = false;
            DeleteBtn.GetComponent<Button>().enabled = false;
            NewGameBtn.GetComponent<Button>().enabled = true;
            LoadBtn.GetComponent<Image>().enabled = false;
            DeleteBtn.GetComponent<Image>().enabled = false;
            NewGameBtn.GetComponent<Image>().enabled = false;
        }

        private void Update()
        {
            selectedObj = EventSystem.current.currentSelectedGameObject;
            if (selectedObj != null && selectedObj.CompareTag("ProfileSlot"))
            {
                selectedProfile = selectedObj;
                if (selectedProfile.transform.Find("Contents").gameObject.activeSelf)
                {
                    LoadBtn.GetComponent<Button>().enabled = true;
                    DeleteBtn.GetComponent<Button>().enabled = true;
                    NewGameBtn.GetComponent<Button>().enabled = false;
                    LoadBtn.GetComponent<Image>().enabled = true;
                    DeleteBtn.GetComponent<Image>().enabled = true;
                    NewGameBtn.GetComponent<Image>().enabled = false;
                }
                else
                {
                    LoadBtn.GetComponent<Button>().enabled = false;
                    DeleteBtn.GetComponent<Button>().enabled = false;
                    NewGameBtn.GetComponent<Button>().enabled = true;
                    LoadBtn.GetComponent<Image>().enabled = false;
                    DeleteBtn.GetComponent<Image>().enabled = false;
                    NewGameBtn.GetComponent<Image>().enabled = true;
                }
            }
        }

        private void LateUpdate()
        {
            if (selectedObj == null)
            {
                DisableAllBtn();
            }
        }

        private void DisableAllBtn()
        {
            LoadBtn.GetComponent<Button>().enabled = false;
            DeleteBtn.GetComponent<Button>().enabled = false;
            NewGameBtn.GetComponent<Button>().enabled = true;
            LoadBtn.GetComponent<Image>().enabled = false;
            DeleteBtn.GetComponent<Image>().enabled = false;
            NewGameBtn.GetComponent<Image>().enabled = false;
        }

        public void NewGame()
        {
            using (StreamWriter writer = new StreamWriter(selectedProfilePath))
            {
                switch (selectedProfile.name)
                {
                    default:
                        break;

                    case "Slot":
                        savePlayerDataPath += "1" + ".json";
                        saveItemPath += "1" + ".json";
                        saveEquipmentPath += "1" + ".json";
                        writer.Write("1");
                        break;

                    case "Slot (1)":
                        savePlayerDataPath += "2" + ".json";
                        saveItemPath += "2" + ".json";
                        saveEquipmentPath += "2" + ".json";
                        writer.Write("2");
                        break;

                    case "Slot (2)":
                        savePlayerDataPath += "3" + ".json";
                        saveItemPath += "3" + ".json";
                        saveEquipmentPath += "3" + ".json";
                        writer.Write("3");
                        break;
                }
                DisableAllBtn();
                SceneManager.LoadScene(1);
            }
        }

        public void Load()
        {
            using (StreamWriter writer = new StreamWriter(selectedProfilePath))
            {
                switch (selectedProfile.name)
                {
                    default:
                        break;

                    case "Slot":
                        savePlayerDataPath += "1" + ".json";
                        saveItemPath += "1" + ".json";
                        saveEquipmentPath += "1" + ".json";
                        writer.Write("1");
                        break;

                    case "Slot (1)":
                        savePlayerDataPath += "2" + ".json";
                        saveItemPath += "2" + ".json";
                        saveEquipmentPath += "2" + ".json";
                        writer.Write("2");
                        break;

                    case "Slot (2)":
                        savePlayerDataPath += "3" + ".json";
                        saveItemPath += "3" + ".json";
                        saveEquipmentPath += "3" + ".json";
                        writer.Write("3");
                        break;
                }
                string json = File.ReadAllText(savePlayerDataPath);
                PlayerDTO dto = JsonConvert.DeserializeObject<PlayerDTO>(json);
                DisableAllBtn();
                SceneManager.LoadScene(dto.SceneIndex);
            }
        }

        public void Delete()
        {
            switch (selectedProfile.name)
            {
                default:
                    break;

                case "Slot":
                    savePlayerDataPath += "1" + ".json";
                    saveItemPath += "1" + ".json";
                    saveEquipmentPath += "1" + ".json";
                    break;

                case "Slot (1)":
                    savePlayerDataPath += "2" + ".json";
                    saveItemPath += "2" + ".json";
                    saveEquipmentPath += "2" + ".json";
                    break;

                case "Slot (2)":
                    savePlayerDataPath += "3" + ".json";
                    saveItemPath += "3" + ".json";
                    saveEquipmentPath += "3" + ".json";
                    break;
            }
            File.Delete(savePlayerDataPath);
            File.Delete(saveItemPath);
            File.Delete(saveEquipmentPath);
            profileSlot.UpdateSlot();
            DisableAllBtn();
        }

        public void Back()
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
}