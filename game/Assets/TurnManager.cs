using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Diagnostics;
using TMPro;


public class TurnManager : NetworkBehaviour
{
    [SyncVar] private int currentTurnIndex = -1;

    private List<NetworkGamePlayerLobby> players = new List<NetworkGamePlayerLobby>();
    [SerializeField] private Dice globalDice;

    [SerializeField] private TMP_Text turnLabel;


    public override void OnStartServer()
    {
        Invoke(nameof(InitializeTurnOrder), 1f);
    }

    [Server]
    void InitializeTurnOrder()
    {
        players = new List<NetworkGamePlayerLobby>(FindObjectsOfType<NetworkGamePlayerLobby>());

        if (globalDice == null)
        {
            globalDice = FindObjectOfType<Dice>();
        }

        if (players.Count == 0 || globalDice == null)
        {
            //Debug.LogError("❌ Jucători sau zarul lipsesc!");
            return;
        }

        currentTurnIndex = -1;
        NextTurn();
    }

    [Server]
    public void NextTurn()
    {
        if (players.Count == 0 || globalDice == null) return;

        var identity = globalDice.GetComponent<NetworkIdentity>();
       
            identity.RemoveClientAuthority();
        

        currentTurnIndex = (currentTurnIndex + 1) % players.Count;
        var nextPlayer = players[currentTurnIndex];

        if (nextPlayer != null && nextPlayer.connectionToClient != null)
        {
            identity.AssignClientAuthority(nextPlayer.connectionToClient);
            globalDice.TargetSetActive(nextPlayer.connectionToClient, true);
            RpcUpdateTurnLabel(nextPlayer.DisplayName);
            //Debug.Log("🎲 Tura este acum la: " + nextPlayer.DisplayName);
        }

       
    }

    [ClientRpc]
    void RpcUpdateTurnLabel(string displayName)
    {
        if (turnLabel != null)
        {
            turnLabel.text = $"It's <b>{displayName}</b>'s turn!";
        }
    }

}
