using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameLogicManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject pawnPrefab;
    [SerializeField] private List<Transform> playerZones;
    [SerializeField] private List<Sprite> pawnSprites;
    [SerializeField] private List<Sprite> monsterSprites; // <- lista de monștri
    [SerializeField] private GameObject dicePrefab;
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private float delayBeforeGameUI = 2.5f;
    [SerializeField] private AudioSource startSound;
    [SerializeField] private List<TMP_Text> playerNameTexts;

    private List<LocalPlayer> players = new List<LocalPlayer>();

    void Start()
    {
        SetupLocalPlayers();
        StartCoroutine(ShowGameBoardAfterDelay());
    }

//     private void SetupLocalPlayers()
// {
//     for (int i = 0; i < GameSettings.NumberOfPlayers; i++)
//     {
//         players.Add(new LocalPlayer
//         {
//             DisplayName = GameSettings.PlayerNames[i],
//             CharacterIndex = i
//         });
//     }
// }
// private void SetupLocalPlayers()
// {
//     players.Clear();

//     if (GameSettings.PlayerNames == null || GameSettings.PlayerNames.Count < GameSettings.NumberOfPlayers)
//     {
//         Debug.LogWarning("⚠ Player names list is missing or incomplete. Using fallback names.");
//         for (int i = 0; i < GameSettings.NumberOfPlayers; i++)
//         {
//             players.Add(new LocalPlayer
//             {
//                 DisplayName = $"Player {i + 1}",
//                 CharacterIndex = i
//             });
//         }
//         return;
//     }

//     for (int i = 0; i < GameSettings.NumberOfPlayers; i++)
//     {
//         players.Add(new LocalPlayer
//         {
//             DisplayName = GameSettings.PlayerNames[i],
//             CharacterIndex = i
//         });
//     }
// }

// private void SetupLocalPlayers()
// {
//     players.Clear();

//     List<int> availableIndexes = new List<int>();
//     for (int i = 0; i < pawnSprites.Count; i++)
//     {
//         availableIndexes.Add(i);
//     }

//     if (GameSettings.PlayerNames == null || GameSettings.PlayerNames.Count < GameSettings.NumberOfPlayers)
//     {
//         Debug.LogWarning("⚠ Player names list is missing or incomplete. Using fallback names.");
//         for (int i = 0; i < GameSettings.NumberOfPlayers; i++)
//         {
//             int randomIndex = GetRandomCharacterIndex(availableIndexes);
//             players.Add(new LocalPlayer
//             {
//                 DisplayName = $"Player {i + 1}",
//                 CharacterIndex = randomIndex
//             });
//         }
//         return;
//     }

//     for (int i = 0; i < GameSettings.NumberOfPlayers; i++)
//     {
//         int randomIndex = GetRandomCharacterIndex(availableIndexes);
//         players.Add(new LocalPlayer
//         {
//             DisplayName = GameSettings.PlayerNames[i],
//             CharacterIndex = randomIndex
//         });
//     }
//     GameSettings.LocalPlayers = players; // adaugă asta la finalul SetupLocalPlayers()

// }
private void SetupLocalPlayers()
{
    players.Clear();

    List<int> availableIndexes = new List<int>();
    for (int i = 0; i < pawnSprites.Count; i++)
    {
        availableIndexes.Add(i);
    }

    int monsterPlayerIndex = Random.Range(0, GameSettings.NumberOfPlayers); // un jucător random va fi monstru
    int monsterSpriteIndex = Random.Range(0, monsterSprites.Count); // un monstru random

    for (int i = 0; i < GameSettings.NumberOfPlayers; i++)
    {
        LocalPlayer player = new LocalPlayer();
        player.DisplayName = (GameSettings.PlayerNames != null && i < GameSettings.PlayerNames.Count) ? GameSettings.PlayerNames[i] : $"Player {i + 1}";

        if (i == monsterPlayerIndex)
        {
            player.CharacterIndex = -(monsterSpriteIndex + 1); // folosim index negativ pt. monștri, ex: -1, -2, etc.
        }
        else
        {
            int normalIndex = GetRandomCharacterIndex(availableIndexes);
            player.CharacterIndex = normalIndex;
        }

        players.Add(player);
    }

    GameSettings.LocalPlayers = new List<LocalPlayer>(players);
}


private int GetRandomCharacterIndex(List<int> availableIndexes)
{
    int rand = Random.Range(0, availableIndexes.Count);
    int index = availableIndexes[rand];
    availableIndexes.RemoveAt(rand); // asigură unicitate
    return index;
}



    private IEnumerator ShowGameBoardAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeGameUI);

        loadingCanvas.SetActive(false);
        gameCanvas.SetActive(true);

        SpawnPawnsForPlayers();

        if (dicePrefab != null)
        {
            Instantiate(dicePrefab, Vector3.zero, Quaternion.identity);
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

    // private void SpawnPawnsForPlayers()
    // {
    //     for (int i = 0; i < playerNameTexts.Count; i++)
    //     {
    //         playerNameTexts[i].text = "";
    //     }

    //     for (int i = 0; i < players.Count && i < playerZones.Count; i++)
    //     {
    //         var player = players[i];
    //         Transform zone = playerZones[i];
    //         int index = player.CharacterIndex;
    //         Sprite characterSprite = (index >= 0 && index < pawnSprites.Count) ? pawnSprites[index] : null;

    //         if (characterSprite == null) continue;

    //         for (int j = 0; j < 4; j++)
    //         {
    //             GameObject pawnGO = Instantiate(pawnPrefab, zone);
            

    //             PawnDisplay display = pawnGO.GetComponent<PawnDisplay>();
    //             display.Setup(characterSprite);

    //             PawnMovement movement = pawnGO.GetComponent<PawnMovement>();


    //             if (movement != null)
    //             {
    //                 movement.homeZone = zone;
    //                 movement.startSquare = GetStartSquareForPlayer(i);
    //                 movement.Owner = player;
    //             }
    //             Debug.Log($"🧩 Pawn created for player {player.DisplayName} with CharacterIndex {player.CharacterIndex}");

    //         }

    //         if (playerNameTexts[i] != null)
    //         {
    //             playerNameTexts[i].text = player.DisplayName;
    //         }

    //     }
    // }
    private void SpawnPawnsForPlayers()
{
    for (int i = 0; i < playerNameTexts.Count; i++)
    {
        playerNameTexts[i].text = "";
    }

    for (int i = 0; i < players.Count && i < playerZones.Count; i++)
    {
        var player = players[i];
        Transform zone = playerZones[i];
        int index = player.CharacterIndex;

        Sprite characterSprite = null;

        // Alegem sprite-ul în funcție de index: >=0 -> normal, <0 -> monstru
        if (index >= 0 && index < pawnSprites.Count)
        {
            characterSprite = pawnSprites[index];
        }
        else if (index < 0 && -index - 1 < monsterSprites.Count)
        {
            characterSprite = monsterSprites[-index - 1];
        }

        if (characterSprite == null) continue;

        for (int j = 0; j < 4; j++)
        {
            GameObject pawnGO = Instantiate(pawnPrefab, zone);
            
            PawnDisplay display = pawnGO.GetComponent<PawnDisplay>();
            display.Setup(characterSprite);

            PawnMovement movement = pawnGO.GetComponent<PawnMovement>();
            if (movement != null)
            {
                movement.homeZone = zone;
                movement.startSquare = GetStartSquareForPlayer(i);
                movement.Owner = player;
            }

            Debug.Log($"🧩 Pawn created for player {player.DisplayName} with CharacterIndex {player.CharacterIndex}");
        }

        if (playerNameTexts[i] != null)
        {
            playerNameTexts[i].text = player.DisplayName;
        }
    }
}

}
