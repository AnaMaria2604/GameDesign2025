using UnityEngine;

public class PawnMovement : MonoBehaviour
{
    public Transform homeZone;
    public Transform startSquare;
    public bool isOnBoard = false;
    public LocalPlayer Owner; // Nou: jucator local asociat pionului

    public void MoveToStart()
    {
        if (!isOnBoard)
        {
            transform.position = startSquare.position;
            isOnBoard = true;
        }
    }
}