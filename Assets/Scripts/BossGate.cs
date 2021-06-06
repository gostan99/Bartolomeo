using Assets.Scripts.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGate : MonoBehaviour
{
    public GameObject bossGoat;
    public GameObject TuongChan;
    public Canvas BossHealthBar;
    public EnemyData BossData;

    // Start is called before the first frame update
    private void Start()
    {
        bossGoat.SetActive(false);
        TuongChan.SetActive(false);
        BossHealthBar.gameObject.SetActive(false);
        BossData = bossGoat.GetComponent<EnemyData>();
    }

    // Update is called once per frame
    private void Update()
    {
        KiemTraMauBoss();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(12))
        {
            bossGoat.SetActive(true);
            TuongChan.SetActive(true);
            BossHealthBar.gameObject.SetActive(true);
        }
    }

    private void KiemTraMauBoss()
    {
        if (bossGoat.gameObject.activeSelf)
        {
            if (BossData.CurrentHealth <= 0)
            {
                TuongChan.SetActive(false);
                BossHealthBar.gameObject.SetActive(false);
                Destroy(this.gameObject);
            }
        }
    }
}