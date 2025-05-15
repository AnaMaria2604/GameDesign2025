using UnityEngine;
using System.Collections;


public class PawnMovement : MonoBehaviour
{
    public Transform homeZone;
    public Transform startSquare;
    public bool isOnBoard = false;
    public LocalPlayer Owner; // Nou: jucator local asociat pionului
    public PawnDisplay display;
    public int pathIndex = -1;

    // public void MoveForward(int steps)
    // {
    //     if (!isOnBoard) return;

    //     pathIndex = (pathIndex + steps) % PathManager.Instance.GetPathLength();
    //     Transform targetSquare = PathManager.Instance.GetSquareAt(pathIndex);
    //     if (targetSquare != null)
    //     {
    //         transform.position = targetSquare.position;
    //         StartCoroutine(UpdateSpritesNextFrame());
    //     }
    // }

    public void MoveForward(int steps)
    {
        if (!isOnBoard) return;

        // Găsim pătrățica pe care se află pionul
        var allSquares = FindObjectsOfType<BoardSquare>();
        BoardSquare currentSquare = null;
        float closestDist = float.MaxValue;

        foreach (var square in allSquares)
        {
            float dist = Vector3.Distance(transform.position, square.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                currentSquare = square;
            }
        }

        if (currentSquare == null)
        {
            Debug.LogWarning("❌ Nu am găsit pătrățica curentă.");
            return;
        }

        int currentIndex = currentSquare.index;
        int newIndex = (currentIndex + steps) % PathManager.Instance.GetPathLength();

        Transform nextSquare = PathManager.Instance.GetSquareAt(newIndex);
        if (nextSquare != null)
        {
            transform.position = nextSquare.position;
            pathIndex = newIndex;

            StartCoroutine(UpdateSpritesNextFrame());
        }
    }


    public void MoveToStart()
    {
        if (!isOnBoard)
        {
            transform.position = startSquare.position;
            isOnBoard = true;

            // 🔧 Delay actualizarea sprite-urilor cu un frame
            StartCoroutine(UpdateSpritesNextFrame());
        }
    }

    private IEnumerator UpdateSpritesNextFrame()
    {
        yield return null; // așteaptă un frame
        GameLogicManager logic = FindObjectOfType<GameLogicManager>();
        logic?.UpdateAllPawnSprites();
    }

    public void UpdateSprite(int count, CharacterVariants variants)
    {
        if (count <= 1)
            display.Setup(variants.baseSprite);
        else if (count == 2)
            display.Setup(variants.x2Sprite);
        else if (count == 3)
            display.Setup(variants.x3Sprite);
        else
            display.Setup(variants.x4Sprite);
    }

}