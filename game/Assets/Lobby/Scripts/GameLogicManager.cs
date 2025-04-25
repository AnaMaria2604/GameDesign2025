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

    private void SpawnPawnsForPlayers()
    {
        var players = FindObjectsOfType<NetworkGamePlayerLobby>();

        for (int i = 0; i < playerNameTexts.Count; i++)
        {
            playerNameTexts[i].text = "";
        }

        for (int i = 0; i < players.Length && i < playerZones.Count; i++)
        {
            var player = players[i];
            Transform zone = playerZones[i];

            int index = player.CharacterIndex;
            Sprite characterSprite = (index >= 0 && index < pawnSprites.Count) ? pawnSprites[index] : null;

            if (characterSprite == null)
            {
                //Debug.LogWarning($"⚠️ Sprite invalid pentru playerul {player.DisplayName}");
                continue;
            }

            for (int j = 0; j < 4; j++)
            {
                GameObject pawnGO = Instantiate(pawnPrefab, zone);
                PawnDisplay display = pawnGO.GetComponent<PawnDisplay>();
                display.Setup(characterSprite);
            }

            if (playerNameTexts[i] != null)
            {
                playerNameTexts[i].text = player.DisplayName;
            }
        }
    }
}
