﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinManagement : MonoBehaviour {
    public enum CoinType
    {
        Normal,
        Negative,
        Glow
    };
    private Queue<CoinType> coinQueue;

    public int coinMultiplier;
    public int amountOfNegativeCoins;
    public int amountOfGlowCoins;
    public GameObject coin;
    public GameObject negativeCoin;
    public GameObject glowCoin;
    public GameObject scoreText;

    private CharacterController playerController;

    void Awake()
    {
        coinQueue = new Queue<CoinType>();
        amountOfNegativeCoins = 0;
        coinMultiplier = 0;
        playerController = GetComponent<CharacterController>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && coinQueue.Count != 0)
        {
            CoinType poppedCoin = coinQueue.Dequeue();
            GameObject currentCoin = coin;
            Vector3 spawnPosition = transform.position + (transform.up.normalized * 1.2f);
            Vector3 velocity = (transform.up.normalized + -transform.forward.normalized) * 5.0f;
            if (poppedCoin == CoinType.Negative)
            {
                currentCoin = negativeCoin;
                amountOfNegativeCoins--;
            }
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit);
            if (!playerController.isGrounded && hit.distance > 1.5f)
            {
                GetComponent<PlayerController>().MidAirJump(coinMultiplier);
                velocity = -transform.up.normalized * 5.0f;
                spawnPosition = transform.position - (transform.up.normalized * 1.5f);
            }
            GameObject newcoin = Instantiate(currentCoin, spawnPosition, Quaternion.identity);
            newcoin.GetComponent<Rigidbody>().useGravity = true;
            newcoin.GetComponent<CoinAnimation>().shouldAnimate = false;

            newcoin.GetComponent<Rigidbody>().velocity = velocity;
            newcoin.GetComponent<Rigidbody>().angularVelocity = transform.right * -4;
            coinMultiplier = coinQueue.Count - (amountOfNegativeCoins * 2);
        }
        scoreText.GetComponent<TextMeshProUGUI>().SetText(coinMultiplier.ToString());
    }
    public CoinType PopCoin()
    {
        CoinType poppedCoin = coinQueue.Dequeue();
        if (poppedCoin == CoinType.Negative)
        {
            amountOfNegativeCoins--;
        }
        coinMultiplier = coinQueue.Count - (amountOfNegativeCoins * 2);
        return poppedCoin;
    }
    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag.Contains("Coin"))
        {
            switch(c.gameObject.tag)
            {
                case "Coin":
                    coinQueue.Enqueue(CoinType.Normal);
                    break;
                case "NegativeCoin":
                    coinQueue.Enqueue(CoinType.Negative);
                    amountOfNegativeCoins++;
                    break;
            }
            c.gameObject.SetActive(false);
            Destroy(c.gameObject);
        }
        coinMultiplier = coinQueue.Count - (amountOfNegativeCoins * 2);
    }
}
