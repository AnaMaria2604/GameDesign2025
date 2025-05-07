// using System.Collections;
// using UnityEngine;
// using System.Collections.Generic;
// using TMPro;

// [System.Serializable]
// public class CharacterVariants
// {
//     public Sprite baseSprite;
//     public Sprite x2Sprite;
//     public Sprite x3Sprite;
//     public Sprite x4Sprite;
// }

// public class GameLogicManager : MonoBehaviour
// {
//     [Header("Setup")]
//     [SerializeField] private GameObject pawnPrefab;
//     [SerializeField] private List<Transform> playerZones;
//     [SerializeField] private List<CharacterVariants> pawnVariants; // normale
//     [SerializeField] private List<CharacterVariants> monsterVariants; // monștri

//     [SerializeField] private GameObject dicePrefab;
//     [SerializeField] private GameObject loadingCanvas;
//     [SerializeField] private GameObject gameCanvas;
//     [SerializeField] private float delayBeforeGameUI = 2.5f;
//     [SerializeField] private AudioSource startSound;
//     [SerializeField] private List<TMP_Text> playerNameTexts;

//     private List<LocalPlayer> players = new List<LocalPlayer>();

//     void Start()
//     {
//         SetupLocalPlayers();
//         StartCoroutine(ShowGameBoardAfterDelay());
//     }


// private void SetupLocalPlayers()
// {
//     players.Clear();

//     List<int> availableIndexes = new List<int>();
//     for (int i = 0; i < pawnSprites.Count; i++)
//     {
//         availableIndexes.Add(i);
//     }

//     int monsterPlayerIndex = Random.Range(0, GameSettings.NumberOfPlayers); // un jucător random va fi monstru
//     int monsterSpriteIndex = Random.Range(0, monsterSprites.Count); // un monstru random

//     for (int i = 0; i < GameSettings.NumberOfPlayers; i++)
//     {
//         LocalPlayer player = new LocalPlayer();
//         player.DisplayName = (GameSettings.PlayerNames != null && i < GameSettings.PlayerNames.Count) ? GameSettings.PlayerNames[i] : $"Player {i + 1}";

//         if (i == monsterPlayerIndex)
//         {
//             player.CharacterIndex = -(monsterSpriteIndex + 1); // folosim index negativ pt. monștri, ex: -1, -2, etc.
//         }
//         else
//         {
//             int normalIndex = GetRandomCharacterIndex(availableIndexes);
//             player.CharacterIndex = normalIndex;
//         }

//         players.Add(player);
//     }

//     GameSettings.LocalPlayers = new List<LocalPlayer>(players);
// }


// private int GetRandomCharacterIndex(List<int> availableIndexes)
// {
//     int rand = Random.Range(0, availableIndexes.Count);
//     int index = availableIndexes[rand];
//     availableIndexes.RemoveAt(rand); // asigură unicitate
//     return index;
// }



//     private IEnumerator ShowGameBoardAfterDelay()
//     {
//         yield return new WaitForSeconds(delayBeforeGameUI);

//         loadingCanvas.SetActive(false);
//         gameCanvas.SetActive(true);

//         SpawnPawnsForPlayers();

//         if (dicePrefab != null)
//         {
//             Instantiate(dicePrefab, Vector3.zero, Quaternion.identity);
//         }
//     }

//     private Transform GetStartSquareForPlayer(int playerIndex)
//     {
//         switch (playerIndex)
//         {
//             case 0: return GameObject.Find("Start_TopLeft").transform;
//             case 1: return GameObject.Find("Start_TopRight").transform;
//             case 2: return GameObject.Find("Start_BottomRight").transform;
//             case 3: return GameObject.Find("Start_BottomLeft").transform;
//             default: return null;
//         }
//     }


//     private void SpawnPawnsForPlayers()
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

//         Sprite characterSprite = null;

//         // Alegem sprite-ul în funcție de index: >=0 -> normal, <0 -> monstru
//         if (index >= 0 && index < pawnSprites.Count)
//         {
//             characterSprite = pawnSprites[index];
//         }
//         else if (index < 0 && -index - 1 < monsterSprites.Count)
//         {
//             characterSprite = monsterSprites[-index - 1];
//         }

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
//                 movement.display = pawnGO.GetComponent<PawnDisplay>();

//             }

//             Debug.Log($"🧩 Pawn created for player {player.DisplayName} with CharacterIndex {player.CharacterIndex}");
//         }

//         if (playerNameTexts[i] != null)
//         {
//             playerNameTexts[i].text = player.DisplayName;
//         }
//     }
// }
// public void UpdateAllPawnSprites()
// {
//     var pawns = FindObjectsOfType<PawnMovement>();
//     Dictionary<Vector3, List<PawnMovement>> grouped = new Dictionary<Vector3, List<PawnMovement>>();

//     foreach (var pawn in pawns)
//     {
//         Vector3 pos = pawn.transform.position;
//         pos.z = 0; // ignorăm Z

//         if (!grouped.ContainsKey(pos))
//             grouped[pos] = new List<PawnMovement>();

//         grouped[pos].Add(pawn);
//     }

//     foreach (var group in grouped.Values)
//     {
//         Dictionary<int, List<PawnMovement>> byType = new Dictionary<int, List<PawnMovement>>();

//         foreach (var pawn in group)
//         {
//             int id = pawn.Owner.CharacterIndex;

//             if (!byType.ContainsKey(id))
//                 byType[id] = new List<PawnMovement>();

//             byType[id].Add(pawn);
//         }

//         foreach (var kvp in byType)
//         {
//             int characterIndex = kvp.Key;
//             List<PawnMovement> sameTypePawns = kvp.Value;

//             CharacterVariants variants = null;

//             if (characterIndex >= 0 && characterIndex < pawnVariants.Count)
//                 variants = pawnVariants[characterIndex];
//             else if (characterIndex < 0 && -characterIndex - 1 < monsterVariants.Count)
//                 variants = monsterVariants[-characterIndex - 1];

//             if (variants == null) continue;

//             foreach (var pawn in sameTypePawns)
//             {
//                 pawn.UpdateSprite(sameTypePawns.Count, variants);
//             }
//         }
//     }
// }


// }
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class CharacterVariants
{
    public Sprite baseSprite;
    public Sprite x2Sprite;
    public Sprite x3Sprite;
    public Sprite x4Sprite;
}

public class GameLogicManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject pawnPrefab;
    [SerializeField] private List<Transform> playerZones;

    [Header("Variants")]
    [SerializeField] private List<CharacterVariants> normalVariantLists; // 5 liste pentru playeri normali
    [SerializeField] private List<CharacterVariants> monsterVariantLists; // 3 liste pentru monștri

    [Header("UI & Game")]
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

    private void SetupLocalPlayers()
    {
        players.Clear();
        List<int> availableNormalIndexes = new List<int> { 0, 1, 2, 3, 4 };

        int monsterIndex = Random.Range(0, GameSettings.NumberOfPlayers); // jucătorul monstru
        int chosenMonsterVariant = Random.Range(0, monsterVariantLists.Count); // una dintre cele 3 liste

        for (int i = 0; i < GameSettings.NumberOfPlayers; i++)
        {
            LocalPlayer player = new LocalPlayer();
            player.DisplayName = (GameSettings.PlayerNames != null && i < GameSettings.PlayerNames.Count)
                ? GameSettings.PlayerNames[i]
                : $"Player {i + 1}";

            if (i == monsterIndex)
            {
                player.CharacterIndex = -(chosenMonsterVariant + 1); // ex: -1, -2, -3
            }
            else
            {
                int normalIdx = GetRandomCharacterIndex(availableNormalIndexes);
                player.CharacterIndex = normalIdx; // ex: 0–4
            }

            players.Add(player);
        }

        GameSettings.LocalPlayers = new List<LocalPlayer>(players);
    }

    private int GetRandomCharacterIndex(List<int> availableIndexes)
    {
        int rand = Random.Range(0, availableIndexes.Count);
        int index = availableIndexes[rand];
        availableIndexes.RemoveAt(rand);
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

            CharacterVariants variants = null;

            if (index >= 0 && index < normalVariantLists.Count)
            {
                variants = normalVariantLists[index];
            }
            else if (index < 0 && -index - 1 < monsterVariantLists.Count)
            {
                variants = monsterVariantLists[-index - 1];
            }

            if (variants == null) continue;

            for (int j = 0; j < 4; j++)
            {
                GameObject pawnGO = Instantiate(pawnPrefab, zone);

                PawnDisplay display = pawnGO.GetComponent<PawnDisplay>();
                display.Setup(variants.baseSprite);

                PawnMovement movement = pawnGO.GetComponent<PawnMovement>();
                if (movement != null)
                {
                    movement.homeZone = zone;
                    movement.startSquare = GetStartSquareForPlayer(i);
                    movement.Owner = player;
                    movement.display = display;
                }

                Debug.Log($"🧩 Pawn created for player {player.DisplayName} with CharacterIndex {player.CharacterIndex}");

            }

            if (playerNameTexts[i] != null)
            {
                playerNameTexts[i].text = player.DisplayName;
            }
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
            default: { Debug.LogWarning($"❌ GetStartSquareForPlayer: Invalid index {playerIndex}"); return null; }
        }
    }

    public void UpdateAllPawnSprites()
    {
        var pawns = FindObjectsOfType<PawnMovement>();
        Dictionary<Vector3, List<PawnMovement>> grouped = new Dictionary<Vector3, List<PawnMovement>>();

        foreach (var pawn in pawns)
        {
            Vector3 pos = pawn.transform.position;
            pos.z = 0;

            if (!grouped.ContainsKey(pos))
                grouped[pos] = new List<PawnMovement>();

            grouped[pos].Add(pawn);
        }

        foreach (var group in grouped.Values)
        {
            Dictionary<int, List<PawnMovement>> byType = new Dictionary<int, List<PawnMovement>>();

            foreach (var pawn in group)
            {
                int id = pawn.Owner.CharacterIndex;

                if (!byType.ContainsKey(id))
                    byType[id] = new List<PawnMovement>();

                byType[id].Add(pawn);
            }

            foreach (var kvp in byType)
            {
                int characterIndex = kvp.Key;
                List<PawnMovement> sameTypePawns = kvp.Value;

                CharacterVariants variants = null;

                if (characterIndex >= 0 && characterIndex < normalVariantLists.Count)
                    variants = normalVariantLists[characterIndex];
                else if (characterIndex < 0 && -characterIndex - 1 < monsterVariantLists.Count)
                    variants = monsterVariantLists[-characterIndex - 1];

                if (variants == null) continue;

                foreach (var pawn in sameTypePawns)
                {
                    pawn.UpdateSprite(sameTypePawns.Count, variants);
                }
            }
        }
    }
}
