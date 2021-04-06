using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class EnemyData : MonoBehaviour
    {
        public float MaxHealth = 100;
        public float CurrentHealth;

        private void Start()
        {
            CurrentHealth = MaxHealth;
        }

    }
}
