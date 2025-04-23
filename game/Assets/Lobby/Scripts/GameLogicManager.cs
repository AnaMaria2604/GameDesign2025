using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Mirror;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Diagnostics;
using System;

public class GameLogicManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject pawnPrefab;
    [SerializeField] private List<Transform> playerZones;
    [SerializeField] private List<Sprite> pawnSprites;
    [SerializeField] private GameObject dicePrefab;
    [SerializeField] private List<Dice> playerDice;

    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private float delayBeforeGameUI = 2.5f;
    [SerializeField] private AudioSource startSound;
    [SerializeField] private List<TMP_Text> playerNameTexts;

    //[SerializeField] private UIFader uiFader;

    void Start()
    {
        UnityEngine.Debug.Log(">>> GameLogicManager Start() s-a apelat");

        StartCoroutine(ShowGameBoardAfterDelay());
    }

    private IEnumerator ShowGameBoardAfterDelay()
    {
        UnityEngine.Debug.Log(">>> Corutina ShowGameBoardAfterDelay() a pornit");

        yield return new WaitForSeconds(delayBeforeGameUI);

        UnityEngine.Debug.Log(">>> Afisam tabla si pionii");
        loadingCanvas.SetActive(false);
        gameCanvas.SetActive(true);

        SpawnPawnsForPlayers();
    }


    private void SpawnPawnsForPlayers()
    {
        var players = FindObjectsOfType<NetworkGamePlayerLobby>();
        UnityEngine.Debug.Log($"🎮 Am găsit {players.Length} jucători conectați.");

        // Curățăm textele
        for (int i = 0; i < playerNameTexts.Count; i++)
        {
            playerNameTexts[i].text = "";
        }

        for (int i = 0; i < players.Length && i < playerZones.Count; i++)
        {
            var player = players[i];
            Transform zone = playerZones[i];

            // 1. Obținem sprite-ul jucătorului
            int index = player.CharacterIndex;
            Sprite characterSprite = null;

            if (index >= 0 && index < pawnSprites.Count)
            {
                characterSprite = pawnSprites[index];
            }
            else
            {
                UnityEngine.Debug.LogWarning($"⚠️ Player {player.DisplayName} are un CharacterIndex invalid: {index}");
                continue;
            }

            // 2. Instanțiem pionii
            for (int j = 0; j < 4; j++)
            {
                GameObject pawnGO = Instantiate(pawnPrefab, zone);
                PawnDisplay display = pawnGO.GetComponent<PawnDisplay>();
                display.Setup(characterSprite);
            }

            // 3. Afișăm numele jucătorului
            if (playerNameTexts[i] != null)
            {
                playerNameTexts[i].text = player.DisplayName;
            }

            // 4. Legăm Dice-ul de jucător
            //if (i < playerDice.Count)
            //{
            //    playerDice[i].SetOwner(player.netIdentity); // 🧠 Dice.cs trebuie să aibă funcția SetOwner(NetworkIdentity)
            //}
            if (NetworkServer.active)
            {
                if (dicePrefab == null)
                {
                    UnityEngine.Debug.LogError("🚨 DicePrefab este NULL la instanțiere!");
                    continue;
                }

                GameObject diceGO = Instantiate(dicePrefab, zone);
                NetworkServer.Spawn(diceGO, player.connectionToClient);

                UnityEngine.Debug.Log($"🎲 Zar instanțiat pentru {player.DisplayName}, netId={player.netId}");

                Dice dice = diceGO.GetComponent<Dice>();
                if (dice == null)
                {
                    UnityEngine.Debug.LogError("⚠️ Dice.cs lipsește de pe prefab!");
                    continue;
                }

                dice.SetOwner(player.netIdentity);
                playerDice.Add(dice);
            }
            else
            {
                UnityEngine.Debug.Log("⚠️ Suntem pe client, nu pe server, nu instanțiem zaruri.");
            }
        }
    }
    

}