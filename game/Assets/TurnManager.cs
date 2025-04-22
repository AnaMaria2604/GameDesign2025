using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TurnManager : NetworkBehaviour
{
    [SyncVar] private int currentTurnIndex = 0;

    [SerializeField] private List<Dice> playerDice; // Setezi în Inspector

    public override void OnStartServer()
    {
        Invoke(nameof(InitializeTurnOrder), 1f); // Așteptăm un pic să apară jucătorii în scenă
    }

    [Server]
    void InitializeTurnOrder()
    {
        var players = FindObjectsOfType<NetworkGamePlayerLobby>();
        if (playerDice == null || playerDice.Count == 0 || players.Length == 0) return;

        for (int i = 0; i < players.Length && i < playerDice.Count; i++)
        {
            playerDice[i].SetOwner(players[i].netIdentity);
        }

        UpdateTurn();
    }

    [Server]
    public void NextTurn()
    {
        playerDice[currentTurnIndex].isMyTurn = false;

        currentTurnIndex = (currentTurnIndex + 1) % playerDice.Count;

        UpdateTurn();
    }

    [Server]
    void UpdateTurn()
    {
        UnityEngine.Debug.Log($"🎲 E rândul jucătorului {currentTurnIndex}");
        playerDice[currentTurnIndex].isMyTurn = true;
    }
}
