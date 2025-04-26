using UnityEngine;
using Mirror;

public class PawnMovement : NetworkBehaviour
{
    public Transform homeZone; // Zona unde sunt ținuți pioni inainte sa iasă
    public Transform startSquare; // Square-ul de Start
    [SyncVar] public bool isOnBoard = false;

    public void MoveToStart()
    {
        if (!isOnBoard)
        {
            transform.position = startSquare.position;
            isOnBoard = true;
        }
    }
}
