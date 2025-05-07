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

    // public void ValidateNames()
    // {
    //     bool hasAtLeastOneName = false;

    //     for (int i = 0; i < GameSettings.NumberOfPlayers; i++)
    //     {
    //         if (!string.IsNullOrWhiteSpace(nameInputs[i].text))
    //         {
    //             hasAtLeastOneName = true;
    //             break;
    //         }
    //     }

    //     confirmButton.interactable = hasAtLeastOneName;
    // }

    //     void InitializeTurnOrder()
    // {
    //     players.Clear();

    //     for (int i = 0; i < GameSettings.NumberOfPlayers; i++)
    //     {
    //         players.Add(new LocalPlayer
    //         {
    //             DisplayName = GameSettings.PlayerNames[i],
    //             CharacterIndex = i
    //         });
    //     }

    //     if (globalDice == null)
    //     {
    //         globalDice = FindObjectOfType<Dice>();
    //     }

    //     if (players.Count == 0 || globalDice == null)
    //     {
    //         return;
    //     }

    //     currentTurnIndex = -1;
    //     NextTurn();
    // }
    // void InitializeTurnOrder()
    // {
    //     players.Clear();

    //     if (GameSettings.PlayerNames == null || GameSettings.PlayerNames.Count < GameSettings.NumberOfPlayers)
    //     {
    //         Debug.LogWarning("⚠ TurnManager: Player names list is missing or incomplete. Using fallback names.");
    //         for (int i = 0; i < GameSettings.NumberOfPlayers; i++)
    //         {
    //             players.Add(new LocalPlayer
    //             {
    //                 DisplayName = $"Player {i + 1}",
    //                 CharacterIndex = i
    //             });
    //         }
    //     }
    //     else
    //     {
    //         for (int i = 0; i < GameSettings.NumberOfPlayers; i++)
    //         {
    //             players.Add(new LocalPlayer
    //             {
    //                 DisplayName = GameSettings.PlayerNames[i],
    //                 CharacterIndex = i
    //             });
    //         }
    //     }

    //     if (globalDice == null)
    //     {
    //         globalDice = FindObjectOfType<Dice>();
    //     }

    //     if (players.Count == 0 || globalDice == null)
    //     {
    //         return;
    //     }

    //     currentTurnIndex = -1;
    //     NextTurn();
    // }

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

    //     currentTurnIndex = (currentTurnIndex + 1) % players.Count;
    //     var nextPlayer = players[currentTurnIndex];

    //     globalDice.SetActive(true); // ← activăm zarul DOAR pentru jucătorul curent
    //     UpdateTurnLabel(nextPlayer.DisplayName);
    // }

    public void NextTurn()
    {
        if (players.Count == 0 || globalDice == null) return;
        UnityEngine.Debug.Log(players.Count);
        currentTurnIndex = (currentTurnIndex + 1) % players.Count;
        var nextPlayer = players[currentTurnIndex];

        globalDice.SetActive(true);
        UpdateTurnLabel(nextPlayer.DisplayName);
    }

    void UpdateTurnLabel(string displayName)
    {
        if (turnLabel != null)
        {
            string label = string.IsNullOrWhiteSpace(displayName) ? "Unnamed Player" : displayName;
            turnLabel.text = $"It's <b>{label}</b>'s turn!";
        }
    }

}
