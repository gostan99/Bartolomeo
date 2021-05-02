using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class HealthPotion : Item
    {
        public float Value = 30;

        private GameObject player;
        private PlayerData playerData;

        private void Start()
        {
            player = GameObject.Find("Player");
            //lấy PlayerData component
            playerData = player.GetComponent<PlayerData>();
        }

        public void OnClick()
        {
            _ = (playerData.currentHealth + Value) <= playerData.maxHealth ?
                playerData.currentHealth += Value : playerData.currentHealth = playerData.maxHealth;
            DestroyImmediate(this.gameObject);
        }
    }
}