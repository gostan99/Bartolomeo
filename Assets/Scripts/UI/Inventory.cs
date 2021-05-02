using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public bool pause = false;
    public GameObject inventoryUI;

    // Use this for initialization
    void Start()
    {
        inventoryUI.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !pause)
        {
            pause = true;
            inventoryUI.SetActive(true);
            Time.timeScale = 0;
        }
        else if (Input.GetKeyDown(KeyCode.P) && pause)
        {
            pause = false;
            inventoryUI.SetActive(false);
            Time.timeScale = 1;
        }
        Debug.Log(Time.timeScale);

    }
}
