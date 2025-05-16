using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PawnDisplay : MonoBehaviour
{
    [SerializeField] private Image pawnImage;
    [SerializeField] public Button clickableButton;


    public void Setup(Sprite pawnSprite)
    {
        pawnImage.sprite = pawnSprite;
   }

    private Coroutine blinkCoroutine;

    public void SetBlinking(bool shouldBlink)
    {
        if (shouldBlink)
        {
            if (blinkCoroutine == null)
                blinkCoroutine = StartCoroutine(Blink());
            clickableButton.interactable = true;
        }
        else
        {
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }

            pawnImage.color = Color.white;
            clickableButton.interactable = false;
        }
    }

    private IEnumerator Blink()
    {
        while (true)
        {
            pawnImage.color = new Color(1f, 1f, 1f, 0.3f);
            yield return new WaitForSeconds(0.3f);
            pawnImage.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(0.3f);
        }
    }

}
