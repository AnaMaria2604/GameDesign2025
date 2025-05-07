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

    // private void MovePawnOutOfHome()
    // {
    //     var pawns = FindObjectsOfType<PawnMovement>();

    //     foreach (var pawn in pawns)
    //     {
    //         if (!pawn.isOnBoard)
    //         {
    //             pawn.MoveToStart();
    //             break;
    //         }
    //     }
    // }

//    private void MovePawnOutOfHome()
// {
//     var turnManager = FindObjectOfType<TurnManager>();
//     if (turnManager == null)
//     {
//         UnityEngine.Debug.LogWarning("⚠ MovePawnOutOfHome: TurnManager not found.");
//         return;
//     }

//     LocalPlayer currentPlayer = turnManager.GetCurrentPlayer();
//     if (currentPlayer == null)
//     {
//         UnityEngine.Debug.LogWarning("⚠ MovePawnOutOfHome: Current player is null.");
//         return;
//     }

//     UnityEngine.Debug.Log($"🔄 It's {currentPlayer.DisplayName}'s turn (CharacterIndex: {currentPlayer.CharacterIndex})");

//     var pawns = FindObjectsOfType<PawnMovement>();

//     foreach (var pawn in pawns)
//     {
//         if (pawn.Owner == null)
//         {
//             UnityEngine.Debug.Log("⚠ Found a pawn with null owner, skipping.");
//             continue;
//         }

//         if (pawn.Owner.CharacterIndex == currentPlayer.CharacterIndex && !pawn.isOnBoard)
//         {
//             UnityEngine.Debug.Log($"✅ Moving pawn out for player {currentPlayer.DisplayName}");
//             pawn.MoveToStart();
//             return; // oprim după ce am mutat un pion
//         }
//     }

//     UnityEngine.Debug.Log($"ℹ {currentPlayer.DisplayName} has no pawns to move out.");
// }
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
        // if (lastResult == 6)
        // { //TODO
            MovePawnOutOfHome();
        // }
        

        TurnManager turnManager = FindObjectOfType<TurnManager>();
        if (turnManager != null)
        {
            turnManager.NextTurn();
        }
    }
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

//     // Move pawn logic here (if needed)

//     // Blochează zarul imediat
//     SetActive(false);

//     // Treci la următorul jucător după o mică pauză
//     yield return new WaitForSeconds(0.5f);
    
//     TurnManager turnManager = FindObjectOfType<TurnManager>();
//     if (turnManager != null)
//     {
//         turnManager.NextTurn();
//     }
// }

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

//     // Blochează zarul imediat după rulare
//     SetActive(false);

//     // Mută un pion al jucătorului curent dacă există vreunul neactiv
//     TurnManager turnManager = FindObjectOfType<TurnManager>();
//     if (turnManager != null)
//     {
//         LocalPlayer currentPlayer = turnManager.GetCurrentPlayer();
//         PawnMovement[] pawns = FindObjectsOfType<PawnMovement>();

//         foreach (var pawn in pawns)
//         {
//             if (pawn.Owner == currentPlayer && !pawn.isOnBoard)
//             {
//                 pawn.MoveToStart();
//                 break; // mutăm doar primul găsit
//             }
//         }

//         yield return new WaitForSeconds(0.5f);
//         turnManager.NextTurn();
//     }
// }

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
}
