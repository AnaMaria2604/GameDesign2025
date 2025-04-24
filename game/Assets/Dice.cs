using Mirror;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Dice : NetworkBehaviour
{
    private Sprite[] diceSides;
    private UnityEngine.UI.Image image;

    private void Start()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");
    }

    [Server]
    public void Roll()
    {
        StartCoroutine(RollTheDice());
    }

    private IEnumerator RollTheDice()
    {
        int randomDiceSide = 0;

        for (int i = 0; i < 20; i++)
        {
            randomDiceSide = UnityEngine.Random.Range(0, diceSides.Length);
            RpcSetSprite(randomDiceSide);
            yield return new WaitForSeconds(0.05f);
        }

        int finalSide = randomDiceSide + 1;
        UnityEngine.Debug.Log("Rezultat final zar: " + finalSide);
    }

    [ClientRpc]
    private void RpcSetSprite(int index)
    {
        if (diceSides.Length > index)
            image.sprite = diceSides[index];
    }
}

