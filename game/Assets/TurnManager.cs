using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // ← aceasta e esențială pentru Button


public class TurnManager : MonoBehaviour
{
    private int currentTurnIndex = -1;

    private List<LocalPlayer> players = new List<LocalPlayer>();
    [SerializeField] private Dice globalDice;
    [SerializeField] private TMP_Text turnLabel;

    [Header("Confirm Button")]
    [SerializeField] private Button confirmButton;

    private void Start()
    {
        InitializeTurnOrder();
    }
    public LocalPlayer GetCurrentPlayer()
    {
        return players[currentTurnIndex];
    }



    void InitializeTurnOrder()
    {
        players = new List<LocalPlayer>(GameSettings.LocalPlayers);

        if (players == null || players.Count == 0)
        {
            Debug.LogError("⚠ TurnManager: No players found in GameSettings.LocalPlayers");
            return;
        }

        if (globalDice == null)
        {
            globalDice = FindObjectOfType<Dice>();
        }

        currentTurnIndex = -1;
        NextTurn();
    }


    // public void NextTurn()
    // {
    //     if (players.Count == 0 || globalDice == null) return;
    //     UnityEngine.Debug.Log(players.Count);
    //     currentTurnIndex = (currentTurnIndex + 1) % players.Count;
    //     var nextPlayer = players[currentTurnIndex];

    //     globalDice.SetActive(true);
    //     UpdateTurnLabel(nextPlayer.DisplayName);

    //     if (globalDice != null)
    //     {
    //         globalDice.ResetSixCounter();
    //         globalDice.SetActive(true);
    //     }


    // }

    public void NextTurn()
    {
        GameLogicManager logic = FindObjectOfType<GameLogicManager>();
        if (logic != null && logic.gameEnded)
        {
            Debug.Log("🎯 Jocul s-a încheiat. Nu mai schimbăm tura.");
            return;
        }

        if (players.Count == 0 || globalDice == null) return;

        currentTurnIndex = (currentTurnIndex + 1) % players.Count;
        var nextPlayer = players[currentTurnIndex];

        globalDice.SetActive(true);
        UpdateTurnLabel(nextPlayer.DisplayName);
        globalDice.ResetSixCounter();
    }

    void UpdateTurnLabel(string displayName)
    {
        if (turnLabel != null)
        {
            string label = string.IsNullOrWhiteSpace(displayName) ? "Unnamed Player" : displayName;
            turnLabel.text = $"It's <b>{label}</b>'s turn!";
        }
    }

    public void RepeatTurn()
    {
        UnityEngine.Debug.Log("Jucătorul mai primește o tură bonus!");

        globalDice.SetActive(true);
        globalDice.ResetSixCounter();

        var currentPlayer = players[currentTurnIndex];
        UpdateTurnLabel(currentPlayer.DisplayName);
    }


}
