//using System.Collections.Generic;
//using UnityEngine;
//using Mirror;

//public class TurnManager : NetworkBehaviour
//{
//    [SyncVar] private int currentTurnIndex = 0;

//    [SerializeField] private List<Dice> playerDice; // Setezi în Inspector

//    public override void OnStartServer()
//    {
//        Invoke(nameof(InitializeTurnOrder), 1f); // Așteptăm un pic să apară jucătorii în scenă
//    }

//    [Server]
//    void InitializeTurnOrder()
//    {
//        var players = FindObjectsOfType<NetworkGamePlayerLobby>();
//        playerDice = new List<Dice>(FindObjectsOfType<Dice>());

//        if (playerDice.Count == 0 || players.Length == 0)
//        {
//            Debug.LogWarning("Nu s-au gasit zaruri sau jucatori.");
//            return;
//        }

//        for (int i = 0; i < players.Length && i < playerDice.Count; i++)
//        {
//            playerDice[i].SetOwner(players[i].netIdentity);
//        }

//        currentTurnIndex = -1; // pornim de la -1 pentru ca prima tura vine dupa
//        NextTurn(); // porneste primul jucator
//    }

//    [Server]
//    public void NextTurn()
//    {
//        if (playerDice.Count == 0) return;

//        // Dezactivam butonul tuturor
//        foreach (var dice in playerDice)
//        {
//            var identity = dice.GetComponent<NetworkIdentity>();
//            if (identity != null && identity.connectionToClient != null)
//            {
//                dice.TargetSetActive(identity.connectionToClient, false);
//            }
//        }

//        currentTurnIndex = (currentTurnIndex + 1) % playerDice.Count;

//        var currentDice = playerDice[currentTurnIndex];
//        var currentIdentity = currentDice.GetComponent<NetworkIdentity>();
//        if (currentIdentity != null && currentIdentity.connectionToClient != null)
//        {
//            currentDice.TargetSetActive(currentIdentity.connectionToClient, true);
//            Debug.Log("Randul este acum la jucatorul " + currentTurnIndex);
//        }
//    }


//    [Server]
//    void UpdateTurn()
//    {
//        if (playerDice.Count == 0) return;

//        UnityEngine.Debug.Log($"🎲 E randul jucatorului {currentTurnIndex}");
//        //playerDice[currentTurnIndex].isMyTurn = true;
//        NetworkIdentity identity = playerDice[currentTurnIndex].GetComponent<NetworkIdentity>();
//        if (identity != null && identity.connectionToClient != null)
//        {
//            playerDice[currentTurnIndex].TargetSetActive(identity.connectionToClient, true);
//        }

//    }
//}
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TurnManager : NetworkBehaviour
{
    [SyncVar] private int currentTurnIndex = -1;

    private List<Dice> playerDice = new List<Dice>();

    public override void OnStartServer()
    {
        Invoke(nameof(InitializeTurnOrder), 1f); // Asteptam putin ca sa se incarce tot
    }

    [Server]
    void InitializeTurnOrder()
    {
        var players = FindObjectsOfType<NetworkGamePlayerLobby>();
        playerDice = new List<Dice>(FindObjectsOfType<Dice>());

        if (playerDice.Count == 0 || players.Length == 0)
        {
            Debug.LogWarning("Nu s-au gasit zaruri sau jucatori.");
            return;
        }

        for (int i = 0; i < players.Length && i < playerDice.Count; i++)
        {
            playerDice[i].SetOwner(players[i].netIdentity);
        }

        currentTurnIndex = -1;
        NextTurn(); // Pornim tura automat
    }

    //[Server]
    //public void NextTurn()
    //{
    //    if (playerDice.Count == 0) return;

    //    // Dezactivam butonul la toti jucatorii
    //    foreach (var dice in playerDice)
    //    {
    //        var identity = dice.GetComponent<NetworkIdentity>();
    //        if (identity != null && identity.connectionToClient != null)
    //        {
    //            dice.TargetSetActive(identity.connectionToClient, false);
    //        }
    //    }

    //    currentTurnIndex = (currentTurnIndex + 1) % playerDice.Count;

    //    var currentDice = playerDice[currentTurnIndex];
    //    var currentIdentity = currentDice.GetComponent<NetworkIdentity>();
    //    if (currentIdentity != null && currentIdentity.connectionToClient != null)
    //    {
    //        currentDice.TargetSetActive(currentIdentity.connectionToClient, true);
    //        Debug.Log("Randul este acum la jucatorul " + currentTurnIndex);
    //    }
    //}
    private void TransferDiceAuthority(Dice dice, NetworkConnectionToClient newOwner)
    {
        NetworkIdentity identity = dice.GetComponent<NetworkIdentity>();

        // Scoatem autoritatea veche inainte sa atribuim alta
        identity.RemoveClientAuthority();

        // Dam autoritate noului client
        identity.AssignClientAuthority(newOwner);
    }


    [Server]
    public void NextTurn()
    {
        if (playerDice.Count == 0) return;

        // Dezactiveaza butoanele tuturor
        foreach (var dice in playerDice)
        {
            var identity = dice.GetComponent<NetworkIdentity>();
            if (identity != null && identity.connectionToClient != null)
            {
                dice.TargetSetActive(identity.connectionToClient, false);
            }
        }

        // Incrementeaza indexul turei
        currentTurnIndex = (currentTurnIndex + 1) % playerDice.Count;

        var nextDice = playerDice[currentTurnIndex];
        var nextIdentity = nextDice.GetComponent<NetworkIdentity>();
        var nextConnection = nextIdentity.connectionToClient;

        if (nextConnection != null)
        {
            TransferDiceAuthority(nextDice, nextConnection);
            nextDice.TargetSetActive(nextConnection, true);
            Debug.Log("Autoritatea si tura au fost mutate la jucatorul " + currentTurnIndex);
        }
    }





}
