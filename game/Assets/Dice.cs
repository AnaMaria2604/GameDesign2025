using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections;

public class Dice : NetworkBehaviour
{
    private UnityEngine.UI.Image image;
    private Button rollButton;
    private Sprite[] diceSides;

    private NetworkIdentity owner;

    [SyncVar]
    public int lastResult;

    [SyncVar(hook = nameof(OnTurnChanged))]
    public bool isMyTurn = false;

    public void SetOwner(NetworkIdentity player)
    {
        owner = player;
    }

    void Start()
    {
        UnityEngine.Debug.Log("BUNAAA dice");
        image = GetComponent<UnityEngine.UI.Image>();
        rollButton = GetComponent<Button>();
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");

        rollButton.onClick.AddListener(() =>
        {

            if (isMyTurn)
            {
                CmdRollDice();
            }
        });

        rollButton.interactable = isMyTurn;
    }

    void OnTurnChanged(bool oldVal, bool newVal)
    {
        if (rollButton != null)
            rollButton.interactable = newVal;
    }

    [Command(requiresAuthority = false)]
    void CmdRollDice()
    {
        StartCoroutine(RollTheDice());
    }

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

        // Trecem la următorul jucător
        if (isServer)
        {
            FindObjectOfType<TurnManager>().NextTurn();
        }
    }
}
