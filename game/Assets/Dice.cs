using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;
using System.Diagnostics;

public class Dice : NetworkBehaviour
{
    private UnityEngine.UI.Image image;
    private Button rollButton;
    private Sprite[] diceSides;
    private bool hasRolled = false;

    [SyncVar]
    public int lastResult;

    public override void OnStartAuthority()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        rollButton = GetComponent<Button>();
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");

        rollButton.onClick.AddListener(() =>
        {
            if (rollButton.interactable && !hasRolled)
            {
                hasRolled = true;
                CmdRollDice();
            }
        });
    }

    public override void OnStartClient()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        if (diceSides == null || diceSides.Length == 0)
        {
            diceSides = Resources.LoadAll<Sprite>("DiceSides/");
        }

        if (image != null)
        {
            image.enabled = true;
            image.color = Color.white;
        }
    }

    [Command]
    void CmdRollDice()
    {
        StartCoroutine(RollTheDice());
    }

    private void MovePawnOutOfHome()
    {
        UnityEngine.Debug.Log("Buna");
        var players = FindObjectsOfType<NetworkGamePlayerLobby>();
        var localPlayer = players.FirstOrDefault(p => p.isOwned);

        if (localPlayer == null) return;

        UnityEngine.Debug.Log("Buna2");

        // 🛠 Schimbăm aici
        var pawns = FindObjectsOfType<PawnMovement>(); // caută toți pionii din scenă!

        foreach (var pawn in pawns)
        {
            UnityEngine.Debug.Log("Buna3");

            // Verificăm dacă pionul aparține jucătorului curent
            if (!pawn.isOnBoard && pawn.Owner == localPlayer)
            {
                pawn.MoveToStart();
                UnityEngine.Debug.Log("Buna4");
                break; // mutăm doar unul
            }
        }
        UnityEngine.Debug.Log("Pa");
    }



    private IEnumerator RollTheDice()
    {
        int randomDiceSide = 0;

        for (int i = 0; i < 20; i++)
        {
            randomDiceSide = UnityEngine.Random.Range(0, diceSides.Length);
            RpcUpdateDiceSprite(randomDiceSide);
            yield return new WaitForSeconds(0.05f);
        }

        lastResult = randomDiceSide + 1;
        MovePawnOutOfHome();

        //if (lastResult == 6)
        //{
        //    MovePawnOutOfHome();
        //}

        if (isServer)
        {
            EndTurn();
        }


    }

    [ClientRpc]
    void RpcUpdateDiceSprite(int spriteIndex)
    {
        if (image != null && diceSides != null && spriteIndex < diceSides.Length)
        {
            image.sprite = diceSides[spriteIndex];
        }
    }

    [TargetRpc]
    public void TargetSetActive(NetworkConnection target, bool active)
    {
        if (rollButton != null)
        {
            rollButton.interactable = active;
            if (active)
            {
                hasRolled = false;
            }
        }
    }

    [Server]
    private void EndTurn()
    {
        TurnManager turnManager = FindObjectOfType<TurnManager>();
        if (turnManager != null)
        {
            turnManager.NextTurn();
        }
    }
}
