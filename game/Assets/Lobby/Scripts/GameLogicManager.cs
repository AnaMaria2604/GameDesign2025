using System.Collections;
using UnityEngine;
using UnityEngine;
using System.Collections.Generic;
using Mirror;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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


    //private void SpawnPawnsForPlayers()
    //{
    //    var players = FindObjectsOfType<NetworkGamePlayerLobby>();

    //    for (int i = 0; i < players.Length && i < playerZones.Count; i++)
    //    {
    //        var player = players[i];
    //        Transform zone = playerZones[i];


    //        for (int j = 0; j < 4; j++)
    //        {
    //            GameObject pawnGO = Instantiate(pawnPrefab, zone);

    //            Sprite randomSprite = pawnSprites[Random.Range(0, pawnSprites.Count)];

    //            PawnDisplay display = pawnGO.GetComponent<PawnDisplay>();
    //            display.Setup(randomSprite, player.DisplayName);
    //        }
    //        UnityEngine.Debug.Log($"Spawnez pionii pentru {player.DisplayName}");

    //    }
    //    UnityEngine.Debug.Log($"Am gasit {players.Length} jucatori.");

    //}
    private void SpawnPawnsForPlayers()
    {
        Debug.Log(" NU sunt jucatori conectati – rulam simulare");

        for (int i = 0; i < playerZones.Count; i++)
        {
            Transform zone = playerZones[i];

            for (int j = 0; j < 4; j++)
            {
                GameObject pawnGO = Instantiate(pawnPrefab, zone);
                Debug.Log($" Pion instantiat in zona {zone.name}");

                Sprite randomSprite = pawnSprites[Random.Range(0, pawnSprites.Count)];
                var display = pawnGO.GetComponent<PawnDisplay>();

                if (display == null)
                {
                    Debug.LogError(" PawnDisplay NU este setat pe prefab!");
                }
                else
                {
                    display.Setup(randomSprite, $"Player {i + 1}");
                }
            }
        }
    }


}
