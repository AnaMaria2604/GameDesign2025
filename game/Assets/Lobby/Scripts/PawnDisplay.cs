using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;

public class PawnDisplay : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image pawnImage;
    [SerializeField] public Button clickableButton;

    private Coroutine blinkCoroutine;

    [SerializeField] private UnityEngine.UI.Image highlightOverlay;

    public void Setup(Sprite pawnSprite)
    {
        pawnImage.sprite = pawnSprite;
    }

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

            SetOverlayAlpha(0f);
            clickableButton.interactable = false;
        }
    }

    private IEnumerator Blink()
    {
        while (true)
        {
            yield return FadeOverlay(0f, 0.3f); // fade in to 80%
            yield return FadeOverlay(0.8f, 0.3f); // fade out to 0%
        }
    }

    private IEnumerator FadeOverlay(float targetAlpha, float duration)
    {
        float startAlpha = highlightOverlay.color.a;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            SetOverlayAlpha(alpha);
            time += Time.deltaTime;
            yield return null;
        }

        SetOverlayAlpha(targetAlpha);
    }

    private void SetOverlayAlpha(float alpha)
    {
        Color c = highlightOverlay.color;
        c.a = alpha;
        highlightOverlay.color = c;
    }

    private void Awake()
    {
        if (highlightOverlay == null)
            highlightOverlay = transform.GetComponentInChildren<UnityEngine.UI.Image>(includeInactive: true);
    }



}
