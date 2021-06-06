using Assets.Scripts.Items.InInventory;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Items.InWorld
{
    public class CoinInWorld : ItemInWorld<Coin>
    {
        private PlayerData pData;

        protected override void Update()
        {
            if (isCollided)
            {
                pData = GameObject.Find("Player").GetComponent<PlayerData>();
                pData.Money += 1;
                Destroy(this.gameObject);
            }
        }
    }
}