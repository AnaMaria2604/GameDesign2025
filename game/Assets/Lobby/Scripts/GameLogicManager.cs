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

    [SerializeField] private GameObject winPopup;
    [SerializeField] private TMP_Text winPopupText;

    public bool gameEnded = false; // jocul s-a încheiat

    public HashSet<Vector3> temporarySafePositions = new HashSet<Vector3>();

    [SerializeField] private AudioSource winSound;





    void Start()
    {
        SetupLocalPlayers();
        StartCoroutine(ShowGameBoardAfterDelay());
    }


    public void CheckWinCondition()
    {
        var pawns = FindObjectsOfType<PawnMovement>();
        var groupedByPlayer = pawns.GroupBy(p => p.Owner);

        foreach (var group in groupedByPlayer)
        {

            if (group.Count(p => p.hasFinished) == 4)
            {
                ShowWinPopup(group.Key.DisplayName);
                break;
            }
        }
    }


    private void ShowWinPopup(string playerName)
    {
        winPopup.SetActive(true);
        gameEnded = true;

        LocalPlayer winner = GameSettings.LocalPlayers.FirstOrDefault(p => p.DisplayName == playerName);

        if (winner != null && winner.CharacterIndex < 0)
        {
            winPopupText.text = "Monstrul a câștigat!";
        }
        else
        {
            winPopupText.text = "Echipa Misterelor a câștigat!";
        }

        // ▶️ Redă sunetul de victorie
        if (winSound != null)
            winSound.Play();

        // Dezactivează zarul
        Dice dice = FindObjectOfType<Dice>();
        if (dice != null)
        {
            dice.SetActive(false);
        }

        // Dezactivează tura
        TurnManager tm = FindObjectOfType<TurnManager>();
        if (tm != null)
        {
            tm.enabled = false;
        }

        UnityEngine.Debug.Log($"{playerName} a câștigat jocul!");
    }



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
        temporarySafePositions.Clear(); // resetăm safe zone-urile temporare

        // Grupăm pionii după poziție
        foreach (var pawn in pawns)
        {
            Vector3 pos = pawn.transform.position;
            pos.z = 0;

            if (!grouped.ContainsKey(pos))
                grouped[pos] = new List<PawnMovement>();

            grouped[pos].Add(pawn);
        }

        foreach (var group in grouped)
        {
            Vector3 center = group.Key;
            List<PawnMovement> pawnsAtSamePosition = group.Value;

            // Grupăm pionii după jucător
            var byOwner = pawnsAtSamePosition
                .GroupBy(p => p.Owner.CharacterIndex)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Safe zone temporar dacă un jucător are ≥ 2 pioni
            foreach (var kvp in byOwner)
            {
                if (kvp.Value.Count >= 2)
                {
                    temporarySafePositions.Add(center);
                    break;
                }
            }

            // 🔁 Dacă sunt mai mulți jucători → afișăm câte un pion pentru fiecare
            if (byOwner.Count > 1)
            {
                List<PawnMovement> compactView = new List<PawnMovement>();

                foreach (var kvp in byOwner)
                {
                    int ownerId = kvp.Key;
                    List<PawnMovement> playerPawns = kvp.Value;
                    PawnMovement representative = playerPawns[0];

                    CharacterVariants variants = null;
                    if (ownerId >= 0 && ownerId < normalVariantLists.Count)
                        variants = normalVariantLists[ownerId];
                    else if (ownerId < 0 && -ownerId - 1 < monsterVariantLists.Count)
                        variants = monsterVariantLists[-ownerId - 1];

                    representative.UpdateSprite(playerPawns.Count, variants);
                    compactView.Add(representative);

                    foreach (var pawn in playerPawns.Skip(1))
                    {
                        pawn.transform.localScale = Vector3.zero;
                    }
                }

                ArrangePawnsInGrid(center, compactView); // ← toți rămân mici
            }

            else
            {
                // Doar un jucător pe pătrățică → un pion vizibil cu sprite updatat
                var kvp = byOwner.First();
                int ownerId = kvp.Key;
                List<PawnMovement> playerPawns = kvp.Value;
                PawnMovement representative = playerPawns[0];

                CharacterVariants variants = null;
                if (ownerId >= 0 && ownerId < normalVariantLists.Count)
                    variants = normalVariantLists[ownerId];
                else if (ownerId < 0 && -ownerId - 1 < monsterVariantLists.Count)
                    variants = monsterVariantLists[-ownerId - 1];

                representative.UpdateSprite(playerPawns.Count, variants);
                representative.transform.position = center;
                representative.transform.localScale = Vector3.one;

                foreach (var pawn in playerPawns.Skip(1))
                {
                    pawn.transform.position = center;
                    pawn.transform.localScale = Vector3.zero;
                }
            }
        }
    }

}
