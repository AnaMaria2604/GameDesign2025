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
    public bool IsMoving { get; private set; } = false;

    // public void MoveForward(int steps)
    // {
    //     if (!isOnBoard) return;

    //     // Găsim pătrățica pe care se află pionul
    //     var allSquares = FindObjectsOfType<BoardSquare>();
    //     BoardSquare currentSquare = null;
    //     float closestDist = float.MaxValue;

    //     foreach (var square in allSquares)
    //     {
    //         float dist = Vector3.Distance(transform.position, square.transform.position);
    //         if (dist < closestDist)
    //         {
    //             closestDist = dist;
    //             currentSquare = square;
    //         }
    //     }

    //     if (currentSquare == null)
    //     {
    //         Debug.LogWarning("❌ Nu am găsit pătrățica curentă.");
    //         return;
    //     }

    //     int currentIndex = currentSquare.index;
    //     int newIndex = (currentIndex + steps) % PathManager.Instance.GetPathLength();

    //     Transform nextSquare = PathManager.Instance.GetSquareAt(newIndex);
    //     if (nextSquare != null)
    //     {
    //         transform.position = nextSquare.position;
    //         pathIndex = newIndex;

    //         StartCoroutine(UpdateSpritesNextFrame());
    //     }
    // }

    public void MoveForward(int steps)
    {
        if (!isOnBoard) return;
        StartCoroutine(MoveStepByStep(steps));
    }

    private IEnumerator MoveStepByStep(int steps)
    {
        IsMoving = true;

        var path = PathManager.Instance;
        if (path == null) yield break;

        int current = pathIndex;
        int pathLength = path.GetPathLength();

        for (int i = 1; i <= steps; i++)
        {
            int nextIndex = (current + i) % pathLength;
            Transform nextSquare = path.GetSquareAt(nextIndex);
            if (nextSquare != null)
            {
                // animația: te duci cu o mică viteză către poziția pătrățelei
                yield return StartCoroutine(MoveToPosition(nextSquare.position));
            }
        }

        pathIndex = (current + steps) % pathLength;

        // actualizează sprite după mutare
        StartCoroutine(UpdateSpritesNextFrame());
        IsMoving = false;

    }

    // private IEnumerator MoveToPosition(Vector3 target)
    // {
    //     float speed = 5f;
    //     while (Vector3.Distance(transform.position, target) > 0.01f)
    //     {
    //         transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    //         yield return null;
    //     }

    //     transform.position = target;
    //     yield return new WaitForSeconds(0.1f); // mică pauză între pași
    // }
    private IEnumerator MoveToPosition(Vector3 target)
    {
        float duration = 0.4f; // timp total per pătrățică (super rapid)
        float elapsed = 0f;
        Vector3 start = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }

    public void MoveToStart()
    {
        if (!isOnBoard)
        {
            transform.position = startSquare.position;
            isOnBoard = true;

            var square = startSquare.GetComponent<BoardSquare>();
            if (square != null)
            {
                pathIndex = square.index; // ← esențial!
            }

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