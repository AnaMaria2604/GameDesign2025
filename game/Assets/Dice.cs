using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;

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
        var players = FindObjectsOfType<NetworkGamePlayerLobby>();
        var localPlayer = players.FirstOrDefault(p => p.isOwned);

        if (localPlayer == null) return;

        var pawns = localPlayer.GetComponentsInChildren<PawnMovement>();

        foreach (var pawn in pawns)
        {
            if (!pawn.isOnBoard)
            {
                pawn.MoveToStart();
                break; // mutăm doar unul!
            }
        }
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

        if (lastResult == 6)
        {
            MovePawnOutOfHome();
        }

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
