using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;


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

    // private void ArrangePawnsInCircle(Vector3 center, List<PawnMovement> pawns)
    // {
    //     float radius = 0.2f;
    //     float angleStep = 360f / pawns.Count;

    //     for (int i = 0; i < pawns.Count; i++)
    //     {
    //         float angle = i * angleStep * Mathf.Deg2Rad;
    //         Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
    //         pawns[i].transform.position = center + offset;
    //         pawns[i].transform.localScale = Vector3.one * 0.6f; // Micșorare
    //     }
    // }
    private void ArrangePawnsInGrid(Vector3 center, List<PawnMovement> pawns)
    {
        int count = pawns.Count;

        float spacing = 40f; // 🔥 Distanță foarte mare între pionii diferiți
        float totalWidth = (count - 1) * spacing;

        // 📍 Începem de la stânga și aliniem pe axa X
        Vector3 startPos = center - new Vector3(totalWidth / 2f, 0f, 0f);

        for (int i = 0; i < count; i++)
        {
            Vector3 offset = new Vector3(i * spacing, 0f, 0f); // doar pe X
            Vector3 newPos = startPos + offset;

            pawns[i].transform.position = newPos;
            pawns[i].transform.localScale = Vector3.one * 0.6f; // micșorat

            Debug.Log($"⬛ Pion {i} poziționat la {newPos}");
        }
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
                    movement.safeZones = new List<Transform>
                    {
                        GameObject.Find("Start_TopLeft").transform,
                        GameObject.Find("Start_TopRight").transform,
                        GameObject.Find("Start_BottomLeft").transform,
                        GameObject.Find("Start_BottomRight").transform
                    };

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
        // foreach (var group in grouped)
        // {
        //     List<PawnMovement> pawnsAtSamePosition = group.Value;
        //     var distinctOwners = new HashSet<int>(pawnsAtSamePosition.Select(p => p.Owner.CharacterIndex));

        //     if (distinctOwners.Count > 1)
        //     {
        //         ArrangePawnsInCircle(group.Key, pawnsAtSamePosition);
        //     }
        //     else
        //     {
        //         foreach (var pawn in pawnsAtSamePosition)
        //         {
        //             pawn.transform.position = group.Key; // Reset poziția
        //             pawn.transform.localScale = Vector3.one; // Reset scalarea
        //         }
        //     }
        // }
        foreach (var group in grouped)
        {
            List<PawnMovement> pawnsAtSamePosition = group.Value;

            // ✅ Detectează câți jucători diferiți sunt în acel grup
            var distinctOwners = new HashSet<int>(pawnsAtSamePosition.Select(p => p.Owner.CharacterIndex));

            if (distinctOwners.Count > 1)
            {
                // 🧩 Sunt pioni de la jucători diferiți → aplicăm aranjarea circulară și micșorare
                UnityEngine.Debug.Log($"🔵 Sunt {distinctOwners.Count} jucători diferiți pe aceeași pătrățică la poziția {group.Key}");
                ArrangePawnsInGrid(group.Key, pawnsAtSamePosition);
            }
            else
            {
                // ♻️ Toți pionii sunt ai aceluiași jucător → păstrează dimensiunea normală și poziția centrală
                foreach (var pawn in pawnsAtSamePosition)
                {
                    pawn.transform.position = group.Key;
                    pawn.transform.localScale = Vector3.one; // dimensiune normală
                }
            }
        }



    }
}
