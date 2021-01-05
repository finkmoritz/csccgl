﻿using System;
using System.Collections;
using System.Collections.Generic;
using csbcgf;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

public class PlayMaker : MonoBehaviourPunCallbacks
{
    private IGame game;
    private GameObject controlPanel;

    private static Vector3 CardDim = new Vector3(1f, 1.5f, 0.01f);

    // Start is called before the first frame update
    void Start()
    {
        InitGame(); //TODO: Remove, only used for testing
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if(PhotonNetwork.IsMasterClient)
        {
            InitGame();
        }
    }

    private void InitGame()
    {
        game = RandomGame();

        float distance = 0.02f;
        for (int p=0; p<2; ++p)
        {
            IPlayer player = game.Players[p];
            Vector3 position = new Vector3(4f - 8f * p, distance, -3f + 6f * p);
            foreach (MonsterCardWithGameObject monsterCard in player.Deck.AllCards)
            {
                GameObject gameObject = PhotonNetwork.Instantiate("MonsterCard", position, Quaternion.identity, 0);
                gameObject.transform.Rotate(-90f, 180f - 180f * p, 0f);
                monsterCard.gameObject = gameObject;

                Card3D card3D = monsterCard.gameObject.GetComponent<Card3D>();
                card3D.SetValue("Mana", monsterCard.ManaValue);
                card3D.SetValue("Attack", monsterCard.AttackValue);
                card3D.SetValue("Life", monsterCard.LifeValue);

                position.y += distance;
            }
        }

        game.StartGame(initialHandSize: 3, initialPlayerLife: 5);

        UpdateCards();
        UpdateUI();
    }

    private void UpdateCards()
    {
        for (int p = 0; p < 2; ++p)
        {
            IPlayer player = game.Players[p];
            int handSize = player.Hand.Size;
            float handAncorX = (1-2*p) * (-(0.5f * handSize) + 0.5f) * CardDim.x;
            Vector3 handAncor = new Vector3(handAncorX, 0.75f, -3.25f + 6.5f * p);
            Vector3 distance = new Vector3((1 - 2 * p) * CardDim.x, 0f, 2 * CardDim.z);
            Quaternion handRotation = Quaternion.Euler(45f, 180f * p, 0f);
            for (int i=0; i<handSize; ++i)
            {
                Card3D card3D = ((MonsterCardWithGameObject)player.Hand[i]).gameObject.GetComponent<Card3D>();
                card3D.targetPosition = handAncor + i * distance;
                card3D.targetRotation = handRotation;
            }
        }
    }

    private void UpdateUI()
    {
        bool activePlayerIsMaster = game.ActivePlayer == game.Players[0];
        photonView.RPC("SetUIActive", RpcTarget.MasterClient, activePlayerIsMaster);
        photonView.RPC("SetUIActive", RpcTarget.Others, !activePlayerIsMaster);
    }

    [PunRPC]
    private void SetUIActive(bool active)
    {
        if (controlPanel == null)
        {
            controlPanel = GameObject.FindWithTag("Control Panel");
        }
        controlPanel.SetActive(active);
    }

    private IGame RandomGame()
    {
        var random = new System.Random();
        IPlayer[] players = new Player[2];
        for (int i=0; i<2; ++i)
        {
            IDeck deck = new Deck();
            for(int j=0; j<30; ++j)
            {
                int mana = random.Next(10) + 1;
                int life = random.Next(mana) + 1;
                int attack = mana - life;
                deck.Push(new MonsterCardWithGameObject(mana, attack, life));
            }
            players[i] = new Player(deck);
        }
        return new Game(players);
    }

    public void OnEndTurnClicked()
    {
        photonView.RPC("EndTurn", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void EndTurn()
    {
        game.NextTurn();
        UpdateCards();
        UpdateUI();
    }
}