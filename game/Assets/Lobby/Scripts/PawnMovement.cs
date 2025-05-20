using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Diagnostics;


public class PawnMovement : MonoBehaviour
{
    public Transform homeZone;
    public Transform startSquare;
    public bool isOnBoard = false;
    public LocalPlayer Owner; // Nou: jucator local asociat pionului
    public PawnDisplay display;
    public int pathIndex = -1;
    public bool IsMoving { get; private set; } = false;
    private int pendingMoveSteps = 0;
    private bool onFinalPath = false;
    private int finalPathIndex = 0;
    public bool hasFinished = false;


    //private void OnPawnClicked()
    //{
    //    display.SetBlinking(false);
    //    display.GetComponent<Button>().onClick.RemoveListener(OnPawnClicked);
    //    MoveForward(pendingMoveSteps);

    //    TurnManager tm = FindObjectOfType<TurnManager>();
    //    if (tm != null)
    //        tm.NextTurn();
    //}

    public bool CanMove(int steps)
    {
        if (!isOnBoard || IsMoving || hasFinished)
            return false;

        if (onFinalPath)
        {
            var finalPath = PathManager.Instance.GetFinalPathForPlayer(GetPlayerIndex());
            int remaining = finalPath.Count - 1 - finalPathIndex;
            return steps <= remaining;
        }

        return true; // pe traseul comun, orice mutare e permisă
    }

    public void EnableManualMove(int steps)
    {
        if (!CanMove(steps)) return; 

        pendingMoveSteps = steps;
        display.SetBlinking(true);
        display.GetComponent<Button>().onClick.AddListener(OnPawnClicked);
    }

    private void OnPawnClicked()
    {
        // Dezactivează interactivitatea tuturor pionilor jucătorului
        var allPawns = FindObjectsOfType<PawnMovement>();
        foreach (var p in allPawns)
        {
            if (p != this)
            {
                p.display.SetBlinking(false);
                p.display.clickableButton.onClick.RemoveAllListeners();
            }
        }

        // Acest pion se mută
        display.SetBlinking(false);
        display.clickableButton.onClick.RemoveListener(OnPawnClicked);
        MoveForward(pendingMoveSteps);

        TurnManager tm = FindObjectOfType<TurnManager>();
        if (tm != null)
            tm.NextTurn();
    }



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

        for (int i = 0; i < steps; i++)
        {
            if (onFinalPath)
            {
                var finalPath = path.GetFinalPathForPlayer(GetPlayerIndex());
                if (finalPath == null) break;

                if (finalPathIndex + 1 < finalPath.Count)
                {
                    finalPathIndex++;
                    Transform next = finalPath[finalPathIndex];
                    yield return StartCoroutine(MoveToPosition(next.position));

                    if (finalPathIndex == finalPath.Count - 1)
                    {
                        hasFinished = true;
                       
                        break; // oprim mutarea
                    }
                }
                else
                {
                   
                    break;
                }
            }
            else
            {
                int nextIndex = (pathIndex + 1) % path.GetPathLength();
                Transform nextSquare = path.GetSquareAt(nextIndex);
                yield return StartCoroutine(MoveToPosition(nextSquare.position));
                pathIndex = nextIndex;

                // Intrare pe drumul final dacă e pe pătratul de start propriu
                if (nextSquare.name == startSquare.name)
                {
                    onFinalPath = true;
                    finalPathIndex = -1; // va deveni 0 la următorul pas
                    
                }
            }
        }

        StartCoroutine(UpdateSpritesNextFrame());
        IsMoving = false;
    }


    private int GetPlayerIndex()
    {
        return GameSettings.LocalPlayers.IndexOf(Owner);
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