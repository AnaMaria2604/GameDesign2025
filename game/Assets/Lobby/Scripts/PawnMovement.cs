using Mirror;
using UnityEngine;

public class PawnMovement : NetworkBehaviour
{
    public Transform homeZone;
    public Transform startSquare;
    [SyncVar] public bool isOnBoard = false;
    [SyncVar] public NetworkGamePlayerLobby Owner; // 🛠 nou câmp!

    public void MoveToStart()
    {
        if (!isOnBoard)
        {
            transform.position = startSquare.position;
            isOnBoard = true;
        }
    }
}
