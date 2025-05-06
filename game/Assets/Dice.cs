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

    private void MovePawnOutOfHome()
    {
        var pawns = FindObjectsOfType<PawnMovement>();

        foreach (var pawn in pawns)
        {
            if (!pawn.isOnBoard)
            {
                pawn.MoveToStart();
                break;
            }
        }
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
        MovePawnOutOfHome();

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
