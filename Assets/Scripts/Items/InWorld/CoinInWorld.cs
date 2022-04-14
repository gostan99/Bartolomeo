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
            if (isCollided && GetComponent<BoxCollider2D>().enabled)
            {
                pData = GameObject.Find("Player").GetComponent<PlayerData>();
                pData.Money += 1;
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<BoxCollider2D>().enabled = false;
                if (!soundHasPlayed)
                {
                    soundHasPlayed = true;
                    audioSource.PlayOneShot(sound);
                }
            }
            if (soundHasPlayed && !audioSource.isPlaying)
            {
                Destroy(this.gameObject);
            }
        }
    }
}