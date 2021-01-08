using Assets.Scripts.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGate : MonoBehaviour
{
    public GameObject bossGoat;
    public GameObject TuongChan;
    // Start is called before the first frame update
    void Start()
    {
        bossGoat.SetActive(false);
        TuongChan.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        KiemTraMauBoss();
    }

     void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(12))
        {
            bossGoat.SetActive(true);
            TuongChan.SetActive(true);
            Debug.Log(collision.gameObject.layer);
        }
    }

    void KiemTraMauBoss()
    {

        if (!bossGoat.gameObject.activeSelf)
        {
            TuongChan.SetActive(false);
        }
    }
}
