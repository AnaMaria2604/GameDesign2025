using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class Dice : MonoBehaviour
{
    private Image image;
    private Button rollButton;
    private Sprite[] diceSides;
    private bool hasRolled = false;

    public int lastResult;
    private int consecutiveSixes = 0;

    private void Start()
    {
        image = GetComponent<Image>();
        rollButton = GetComponent<Button>();
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");

        rollButton.onClick.AddListener(() =>
        {
            if (rollButton.interactable && !hasRolled)
            {
                hasRolled = true;
                StartCoroutine(RollTheDice());
            }
        });
    }


    private void MovePawnOutOfHome()
    {
        var turnManager = FindObjectOfType<TurnManager>();
        if (turnManager == null)
        {
            Debug.LogWarning("⚠ MovePawnOutOfHome: TurnManager not found.");
            return;
        }

        LocalPlayer currentPlayer = turnManager.GetCurrentPlayer();
        if (currentPlayer == null)
        {
            Debug.LogWarning("⚠ MovePawnOutOfHome: Current player is null.");
            return;
        }

        Debug.Log($"🔄 It's {currentPlayer.DisplayName}'s turn (CharacterIndex: {currentPlayer.CharacterIndex})");

        var pawns = FindObjectsOfType<PawnMovement>();

        foreach (var pawn in pawns)
        {
            if (pawn.Owner == null)
            {
                Debug.Log("⚠ Found a pawn with null owner, skipping.");
                continue;
            }

            // Comparam dupa CharacterIndex, nu dupa referinta
            if (!pawn.isOnBoard && pawn.Owner != null &&
                pawn.Owner.CharacterIndex == currentPlayer.CharacterIndex)
            {
                Debug.Log($"✅ Moving pawn out for player {currentPlayer.DisplayName} with index {pawn.Owner.CharacterIndex}");
                pawn.MoveToStart();
                return;
            }
        }

        Debug.Log($"ℹ {currentPlayer.DisplayName} has no pawns to move out.");
    }


    //DO NOT DELETE IT!!!!!!!!!!!!!! este codul de inainte de a adauga optiunea de a da 6 de 3 ori
    // private IEnumerator RollTheDice()
    // {
    //     int randomDiceSide = 0;

    //     for (int i = 0; i < 20; i++)
    //     {
    //         randomDiceSide = Random.Range(0, diceSides.Length);
    //         UpdateDiceSprite(randomDiceSide);
    //         yield return new WaitForSeconds(0.05f);
    //     }

    //     lastResult = randomDiceSide + 1;

    //     if (lastResult == 6)
    //         consecutiveSixes++;
    //     else
    //         consecutiveSixes = 0; // reset dacă nu e 6

    //     TurnManager turnManager = FindObjectOfType<TurnManager>();
    //     if (turnManager == null) yield break;

    //     LocalPlayer currentPlayer = turnManager.GetCurrentPlayer();
    //     PawnMovement[] pawns = FindObjectsOfType<PawnMovement>();


    //     if (lastResult == 6)
    //     {
    //         var playerPawns = pawns
    //             .Where(p => p.Owner != null && p.Owner.CharacterIndex == currentPlayer.CharacterIndex)
    //             .ToList();

    //         // Verificăm dacă există cel puțin un pion în casă
    //         var inHome = playerPawns.Where(p => !p.isOnBoard).ToList();

    //         if (inHome.Count > 0)
    //         {
    //             // Scoatem unul random
    //             var pawnToRelease = inHome[Random.Range(0, inHome.Count)];
    //             pawnToRelease.MoveToStart();
    //             yield return new WaitForSeconds(0.5f);
    //         }
    //         else
    //         {
    //             // Toți sunt scoși → alegem unul random de pe tablă
    //             var onBoard = playerPawns.Where(p => p.isOnBoard).ToList();
    //             if (onBoard.Count > 0)
    //             {
    //                 var pawnToMove = onBoard[Random.Range(0, onBoard.Count)];
    //                 pawnToMove.MoveForward(6);
    //                 yield return new WaitUntil(() => !pawnToMove.IsMoving);
    //             }
    //         }

    //         if (consecutiveRolls < 3)
    //         {
    //             SetActive(true); // încă ai voie să mai arunci
    //         }
    //         else
    //         {
    //             turnManager.NextTurn();
    //         }
    //         yield break;
    //     }

    //     else
    //     {
    //         var candidatePawns = pawns
    //             .Where(p => p.Owner != null &&
    //                         p.Owner.CharacterIndex == currentPlayer.CharacterIndex &&
    //                         p.isOnBoard)
    //             .ToList();

    //         // Dacă nu are niciun pion pe tablă, nu facem nimic
    //         if (candidatePawns.Count == 0)
    //         {
    //             yield return new WaitForSeconds(0.5f);
    //             turnManager.NextTurn();
    //             yield break;
    //         }

    //         // Grupăm pionii după poziția exactă
    //         var grouped = candidatePawns
    //             .GroupBy(p => p.transform.position)
    //             .OrderByDescending(g => g.Count()) // cele mai multe suprapuneri
    //             .First(); // luăm primul grup cu cei mai mulți pioni

    //         // Alegem unul random din acel grup
    //         PawnMovement selectedPawn = grouped
    //             .OrderBy(p => Random.value)
    //             .First();

    //         // Îl mutăm
    //         selectedPawn.MoveForward(lastResult);
    //         yield return new WaitUntil(() => !selectedPawn.IsMoving);

    //     }

    //     turnManager.NextTurn();
    // }


    //JOS E CODUL PT A ARUNCA 6 DE MAXIM DE 3 ORI PE TURA
    private IEnumerator RollTheDice()
    {
        int randomDiceSide = 0;

        for (int i = 0; i < 20; i++)
        {
            randomDiceSide = Random.Range(0, diceSides.Length);
            UpdateDiceSprite(randomDiceSide);
            yield return new WaitForSeconds(0.05f);
        }

        lastResult = randomDiceSide + 1;

        if (lastResult == 6)
            consecutiveSixes++;
        else
            consecutiveSixes = 0;

        TurnManager turnManager = FindObjectOfType<TurnManager>();
        if (turnManager == null) yield break;

        LocalPlayer currentPlayer = turnManager.GetCurrentPlayer();
        PawnMovement[] pawns = FindObjectsOfType<PawnMovement>();

        if (lastResult == 6)
        {
            if (consecutiveSixes >= 3)
            {
                Debug.Log("🚫 Ai dat 6 de 3 ori în aceeași tură. Pierzi rândul.");
                consecutiveSixes = 0;
                turnManager.NextTurn();
                yield break;
            }

            var playerPawns = pawns
                .Where(p => p.Owner != null && p.Owner.CharacterIndex == currentPlayer.CharacterIndex)
                .ToList();

            var inHome = playerPawns.Where(p => !p.isOnBoard).ToList();

            if (inHome.Count > 0)
            {
                var pawnToRelease = inHome[Random.Range(0, inHome.Count)];
                pawnToRelease.MoveToStart();
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                var onBoard = playerPawns.Where(p => p.isOnBoard).ToList();
                if (onBoard.Count > 0)
                {
                    var pawnToMove = onBoard[Random.Range(0, onBoard.Count)];
                    pawnToMove.MoveForward(6);
                    yield return new WaitUntil(() => !pawnToMove.IsMoving);
                }
            }

            SetActive(true); // permite altă aruncare dacă nu s-a ajuns la 3×6
            yield break;
        }
        else
        {
            var candidatePawns = pawns
                .Where(p => p.Owner != null &&
                            p.Owner.CharacterIndex == currentPlayer.CharacterIndex &&
                            p.isOnBoard)
                .ToList();

            if (candidatePawns.Count == 0)
            {
                yield return new WaitForSeconds(0.5f);
                consecutiveSixes = 0;
                turnManager.NextTurn();
                yield break;
            }

            var grouped = candidatePawns
                .GroupBy(p => p.transform.position)
                .OrderByDescending(g => g.Count())
                .First();

            //PawnMovement selectedPawn = grouped
            //    .OrderBy(p => Random.value)
            //    .First();

            ////selectedPawn.MoveForward(lastResult);
            ////yield return new WaitUntil(() => !selectedPawn.IsMoving);
            //selectedPawn.EnableManualMove(lastResult);

            foreach (var pawn in candidatePawns)
            {
                pawn.EnableManualMove(lastResult);
            }


        }

        consecutiveSixes = 0;
        //turnManager.NextTurn();
    }
    private void UpdateDiceSprite(int spriteIndex)
    {
        if (image != null && diceSides != null && spriteIndex < diceSides.Length)
        {
            image.sprite = diceSides[spriteIndex];
        }
    }

    public void SetActive(bool active)
    {
        if (rollButton != null)
        {
            rollButton.interactable = active;
            if (active)
            {
                hasRolled = false;
            }
        }
    }


    public void ResetSixCounter()
    {
        consecutiveSixes = 0;
    }


}
