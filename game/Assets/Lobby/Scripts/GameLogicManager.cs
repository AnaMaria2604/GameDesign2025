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
        SpawnDiceForEachPlayer();
    }

   

    

    private void SpawnDiceForEachPlayer()
    {
        GameObject dicePrefab = Resources.Load<GameObject>("SpawnablePrefabs/Dice");

        if (dicePrefab == null || !NetworkServer.active)
            return;

        var players = FindObjectsOfType<NetworkGamePlayerLobby>();

        for (int i = 0; i < players.Length && i < playerZones.Count; i++)
        {
            Vector3 spawnPos = playerZones[i].position + new Vector3(1, 0, 0); // zarul la dreapta fiecărei zone

            GameObject diceInstance = Instantiate(dicePrefab, spawnPos, Quaternion.identity);
            NetworkServer.Spawn(diceInstance);
        }
    }


    private void SpawnPawnsForPlayers()
{
    var players = FindObjectsOfType<NetworkGamePlayerLobby>();
    UnityEngine.Debug.Log($"Am gasit {players.Length} jucatori conectati.");

    for (int i = 0; i < players.Length && i < playerZones.Count; i++)
    {
        var player = players[i];
        Transform zone = playerZones[i];

        // Obținem sprite-ul aferent indexului de personaj al jucătorului
        int index = player.CharacterIndex;
        Sprite characterSprite = null;

        if (index >= 0 && index < pawnSprites.Count)
        {
            characterSprite = pawnSprites[index];
        }
        else
        {
            UnityEngine.Debug.LogWarning($"Player {player.DisplayName} are un CharacterIndex invalid: {index}");
            continue;
        }

        // Instanțiem 4 pioni identici
        for (int j = 0; j < 4; j++)
        {
            GameObject pawnGO = Instantiate(pawnPrefab, zone);
            PawnDisplay display = pawnGO.GetComponent<PawnDisplay>();
            display.Setup(characterSprite);

            // (Opțional) Dacă vrei să fie rețea-aware:
            // var netIdentity = pawnGO.GetComponent<NetworkIdentity>();
            // if (netIdentity != null && NetworkServer.active)
            // {
            //     NetworkServer.Spawn(pawnGO);
            // }
        }

        // Afișăm numele jucătorului
        TMP_Text nameText = playerNameTexts[i];
        if (nameText != null)
        {
            nameText.text = player.DisplayName;
        }
    }
}



}