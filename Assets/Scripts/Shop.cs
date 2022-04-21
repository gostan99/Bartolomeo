﻿using Assets.Scripts.Items;
using Assets.Scripts.Player;
using Assets.Scripts.UI.UIContext.InventorySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public Vector2 size = new Vector2(93, 138);
    private LayerMask PlayerLayer;
    private PlayerData playerData;
    private PlayerController playerController;
    private Text hintText; // text gợi ý
    public GameObject storeUI;
    private GameObject currentSelectedObj;
    public GameObject selectedItemSlot;
    private GameObject buyBtn;
    protected ItemSlotManager itemSlotManagerInventory;
    private bool isBuying = false;

    // Start is called before the first frame update
    private void Start()
    {
        var canvas = GameObject.FindGameObjectWithTag("Canvas");
        var inventoryUI = canvas.transform.Find("InventoryUI");
        itemSlotManagerInventory = inventoryUI.Find("Items").GetComponent<ItemSlotManager>();
        buyBtn = transform.Find("Canvas").Find("StoreUI").Find("Buttons").Find("BuyBtn").gameObject;
        playerData = GameObject.Find("Player").GetComponent<PlayerData>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        PlayerLayer = LayerMask.GetMask("Player");
        hintText = transform.Find("Canvas").Find("HintText").GetComponent<Text>();
        hintText.enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        Collider2D collided = Physics2D.OverlapBox(transform.position, size, 0, PlayerLayer);
        if (collided)
        {
            hintText.enabled = true;
            if (Input.GetKey(KeyCode.E))
            {
                storeUI.SetActive(true);
            }
            else if (Input.GetKey(KeyCode.Escape))
            {
                storeUI.SetActive(false);
            }
        }
        else
        {
            hintText.enabled = false;
        }

        if (storeUI.activeSelf)
        {
            currentSelectedObj = EventSystem.current.currentSelectedGameObject;
            if (currentSelectedObj != null)
            {
                if (currentSelectedObj.CompareTag("InventorySlot"))
                {
                    selectedItemSlot = currentSelectedObj;

                    buyBtn.GetComponent<Button>().enabled = true;
                    buyBtn.GetComponent<Image>().enabled = true;
                }
            }
        }
        if (isBuying)
        {
            Item item = selectedItemSlot.transform.Find("Item").GetComponent<Item>();
            if (item != null)
            {
                itemSlotManagerInventory.AddItemToInventorySlot(item.GetType(), 1);
            }
            buyBtn.GetComponent<Button>().enabled = false;
            buyBtn.GetComponent<Image>().enabled = false;
            isBuying = false;
        }
    }

    public void Buy()
    {
        isBuying = true;
    }
}