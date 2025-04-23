using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections;

public class Dice : NetworkBehaviour
{
    private UnityEngine.UI.Image image;
    private Button rollButton;
    private Sprite[] diceSides;
    private bool hasRolled = false;

    private NetworkIdentity owner;

    [SyncVar]
    public int lastResult;

    //[SyncVar(hook = nameof(OnTurnChanged))]
    //public bool isMyTurn = false;

    public void SetOwner(NetworkIdentity player)
    {
        owner = player;
    }

    //void Start()
    //{
    //    UnityEngine.Debug.Log("BUNAAA dice");
    //    image = GetComponent<UnityEngine.UI.Image>();
    //    rollButton = GetComponent<Button>();
    //    diceSides = Resources.LoadAll<Sprite>("DiceSides/");
    //    Debug.Log($"---> Dice netId: {GetComponent<NetworkIdentity>().netId}, isServer: {isServer}");

    //    rollButton.onClick.AddListener(() =>
    //    {

    //        if (isMyTurn)
    //        {
    //            CmdRollDice();
    //        }
    //    });

    //    rollButton.interactable = isMyTurn;
    //}


    //void Start()
    //{
    //    image = GetComponent<UnityEngine.UI.Image>();
    //    rollButton = GetComponent<Button>();
    //    diceSides = Resources.LoadAll<Sprite>("DiceSides/");

    //    Debug.Log($"🎲 Dice Start() | netId={netId} | isLocalPlayer={isLocalPlayer}");

    //    rollButton.onClick.AddListener(() =>
    //    {
    //        Debug.Log("Click pe zar");
    //        if (((NetworkBehaviour)this).hasAuthority && rollButton.interactable)

    //        {
    //            CmdRollDice();
    //        }
    //    });

    //    //rollButton.interactable = isMyTurn;
    //}
    public override void OnStartAuthority()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        rollButton = GetComponent<Button>();
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");

        Debug.Log($"Dice StartAuthority | netId={netId}");
        Debug.Log("Zar a primit autoritate - client local");

        rollButton.onClick.AddListener(() =>
        {
            if (rollButton.interactable && !hasRolled)
            {
                hasRolled = true;
                CmdRollDice();
            }
        });

    }

    [TargetRpc]
    public void TargetSetActive(NetworkConnection target, bool active)
    {
        if (rollButton != null)
        {
            rollButton.interactable = active;

            if (active)
            {
                hasRolled = false; // reseteaza pentru noul jucator
            }

            Debug.Log("Buton zar activat: " + active);
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


    //void OnTurnChanged(bool oldVal, bool newVal)
    //{
    //    Debug.Log($"🔁 OnTurnChanged triggered on {netId} | newVal = {newVal}");

    //    if (rollButton != null)
    //        rollButton.interactable = newVal;
    //}

    [Command]
    void CmdRollDice()
    {
        StartCoroutine(RollTheDice());
    }

    //private IEnumerator RollTheDice()
    //{
    //    int randomDiceSide = 0;

    //    for (int i = 0; i < 20; i++)
    //    {
    //        randomDiceSide = UnityEngine.Random.Range(0, diceSides.Length);
    //        image.sprite = diceSides[randomDiceSide];
    //        yield return new WaitForSeconds(0.05f);
    //    }

    //    lastResult = randomDiceSide + 1;
    //    Debug.Log("Rezultat zar: " + lastResult);

    //    // Trecem la următorul jucător
    //    if (isServer)
    //    {
    //        FindObjectOfType<TurnManager>().NextTurn();
    //    }
    //}

    private IEnumerator RollTheDice()
    {
        int randomDiceSide = 0;

        for (int i = 0; i < 20; i++)
        {
            randomDiceSide = UnityEngine.Random.Range(0, diceSides.Length);
            image.sprite = diceSides[randomDiceSide];
            yield return new WaitForSeconds(0.05f);
        }

        lastResult = randomDiceSide + 1;
        Debug.Log("Rezultat zar: " + lastResult);

        // Apelam finalul de tura pe server
        if (isServer)
        {
            EndTurn();
        }
    }

}
