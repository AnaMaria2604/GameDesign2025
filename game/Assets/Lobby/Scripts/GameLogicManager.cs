using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Mirror;
using TMPro;
using System.Diagnostics;

public class GameLogicManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject pawnPrefab;
    [SerializeField] private List<Transform> playerZones;
    [SerializeField] private List<Sprite> pawnSprites;
    [SerializeField] private GameObject dicePrefab;
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private float delayBeforeGameUI = 2.5f;
    [SerializeField] private AudioSource startSound;
    [SerializeField] private List<TMP_Text> playerNameTexts;

    void Start()
    {
        StartCoroutine(ShowGameBoardAfterDelay());
    }

    private IEnumerator ShowGameBoardAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeGameUI);

        loadingCanvas.SetActive(false);
        gameCanvas.SetActive(true);

        SpawnPawnsForPlayers();

        if (NetworkServer.active && dicePrefab != null)
        {
            GameObject diceGO = Instantiate(dicePrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.Spawn(diceGO);
            //Debug.Log("🎲 Zar global instanțiat.");
        }
    }

    private Transform GetStartSquareForPlayer(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0: return GameObject.Find("Start_TopLeft").transform;
            case 1: return GameObject.Find("Start_TopRight").transform;
            case 2: return GameObject.Find("Start_BottomRight").transform;
            case 3: return GameObject.Find("Start_BottomLeft").transform;
            default: return null;
        }
    }


    private void SpawnPawnsForPlayers()
    {
        var players = FindObjectsOfType<NetworkGamePlayerLobby>();

        for (int i = 0; i < playerNameTexts.Count; i++)
        {
            playerNameTexts[i].text = "";
        }

        // În loc să folosim direct zona părinților, setăm clar HomeZone

        
        for (int i = 0; i < players.Length && i < playerZones.Count; i++)
        {
            var player = players[i];
         
            Transform zone = playerZones[i]; // Aici e zona vizuală în care apar pionii, EX: PlayerZone_TopLeft
            UnityEngine.Debug.Log(zone);

            int index = player.CharacterIndex;
            Sprite characterSprite = (index >= 0 && index < pawnSprites.Count) ? pawnSprites[index] : null;

            if (characterSprite == null)
            {
                continue;
            }

            for (int j = 0; j < 4; j++)
            {
                GameObject pawnGO = Instantiate(pawnPrefab, zone);
                PawnDisplay display = pawnGO.GetComponent<PawnDisplay>();
                display.Setup(characterSprite);

                PawnMovement movement = pawnGO.GetComponent<PawnMovement>();
                //movement.homeZone = zone; // aici îi spunem pionului unde este "acasă"
                //movement.startSquare = GetStartSquareForPlayer(i); // aici îi spunem unde este "Start-ul" de ieșire
                if (movement != null)
                {
                    movement.homeZone = zone;
                    movement.startSquare = GetStartSquareForPlayer(i);
                    movement.Owner = player; // 🛠 setăm Owner-ul pionului
                }
            }

            if (playerNameTexts[i] != null)
            {
                playerNameTexts[i].text = player.DisplayName;
            }
        }

    }
}
