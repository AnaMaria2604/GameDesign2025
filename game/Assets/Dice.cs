using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

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
            UnityEngine.Debug.LogWarning("⚠ MovePawnOutOfHome: TurnManager not found.");
            return;
        }

        LocalPlayer currentPlayer = turnManager.GetCurrentPlayer();
        if (currentPlayer == null)
        {
            UnityEngine.Debug.LogWarning("⚠ MovePawnOutOfHome: Current player is null.");
            return;
        }

        UnityEngine.Debug.Log($"🔄 It's {currentPlayer.DisplayName}'s turn (CharacterIndex: {currentPlayer.CharacterIndex})");
        GameLogger.Instance?.Log($"It's {currentPlayer.DisplayName}'s turn (CharacterIndex: {currentPlayer.CharacterIndex})");


        var pawns = FindObjectsOfType<PawnMovement>();

        foreach (var pawn in pawns)
        {
            if (pawn.Owner == null)
            {
                UnityEngine.Debug.Log("⚠ Found a pawn with null owner, skipping.");
                continue;
            }

            // Comparam dupa CharacterIndex, nu dupa referinta
            if (!pawn.isOnBoard && pawn.Owner != null &&
                pawn.Owner.CharacterIndex == currentPlayer.CharacterIndex)
            {
                UnityEngine.Debug.Log($"✅ Moving pawn out for player {currentPlayer.DisplayName} with index {pawn.Owner.CharacterIndex}");
                GameLogger.Instance?.Log($"Moving pawn out for player {currentPlayer.DisplayName}");
                pawn.MoveToStart();
                return;
            }
        }

        UnityEngine.Debug.Log($"ℹ {currentPlayer.DisplayName} has no pawns to move out.");
    }


    private IEnumerator RollTheDice()
    {
        GameLogicManager logic = FindObjectOfType<GameLogicManager>();
        if (logic != null && logic.gameEnded)
        {
            UnityEngine.Debug.Log("🎲 Jocul s-a terminat. Zarul nu se mai aruncă.");
            yield break;
        }

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
            if (consecutiveSixes >= 4)
            {
                UnityEngine.Debug.Log(" Ai dat 6 de 3 ori în aceeași tură. Pierzi rândul.");
                GameLogger.Instance?.Log("Ai dat 6 de 3 ori în aceeași tură. Pierzi rândul.");

                consecutiveSixes = 0;
                turnManager.NextTurn();
                yield break;
            }

            var playerPawns = pawns
                .Where(p => p.Owner != null && p.Owner.CharacterIndex == currentPlayer.CharacterIndex)
                .ToList();

            var inHome = playerPawns.Where(p => !p.isOnBoard).ToList();
            var onBoard = playerPawns.Where(p => p.isOnBoard).ToList();

            if (inHome.Count == 0 && onBoard.Count == 0)
            {
                UnityEngine.Debug.Log("ℹ Niciun pion disponibil pentru mutare.");
                GameLogger.Instance?.Log("Niciun pion disponibil pentru mutare.");

                turnManager.NextTurn();
                yield break;
            }

            yield return StartCoroutine(WaitForPlayerChoice(currentPlayer, inHome, onBoard));

            SetActive(true); // Permite o nouă aruncare
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

            bool hasValidMove = false;

            foreach (var pawn in candidatePawns)
            {
                if (pawn.CanMove(lastResult))
                {
                    hasValidMove = true;
                    pawn.EnableManualMove(lastResult);
                }
            }

            if (!hasValidMove)
            {
                yield return new WaitForSeconds(0.5f);
                turnManager.NextTurn();
                yield break;
            }



        }

        consecutiveSixes = 0;
    }

    private IEnumerator WaitForPlayerChoice(LocalPlayer currentPlayer, List<PawnMovement> inHome, List<PawnMovement> onBoard)
    {
        bool choiceMade = false;

        foreach (var pawn in inHome)
        {
            pawn.display.SetBlinking(true);
            pawn.display.clickableButton.onClick.AddListener(() =>
            {
                pawn.MoveToStart();
                choiceMade = true;
                ClearAllPawnChoices();
            });
        }

        foreach (var pawn in onBoard)
        {
            if (pawn.CanMove(6))
            {
                pawn.display.SetBlinking(true);
                pawn.display.clickableButton.onClick.AddListener(() =>
                {
                    pawn.MoveForward(6);
                    StartCoroutine(WaitUntilNotMoving(pawn, () => choiceMade = true));
                    ClearAllPawnChoices();
                });
            }
        }

        yield return new WaitUntil(() => choiceMade);
    }



    private void ClearAllPawnChoices()
    {
        var pawns = FindObjectsOfType<PawnMovement>();
        foreach (var pawn in pawns)
        {
            pawn.display.SetBlinking(false);
            pawn.display.clickableButton.onClick.RemoveAllListeners();
        }
    }

    private IEnumerator WaitUntilNotMoving(PawnMovement pawn, System.Action callback)
    {
        yield return new WaitUntil(() => !pawn.IsMoving);
        callback?.Invoke();
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
