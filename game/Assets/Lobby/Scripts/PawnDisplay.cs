using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PawnDisplay : MonoBehaviour
{
    [SerializeField] private Image pawnImage;
    [SerializeField] private TMP_Text playerNameText;

    public void Setup(Sprite pawnSprite, string playerName)
    {
        pawnImage.sprite = pawnSprite;
        playerNameText.text = playerName;
    }
}
