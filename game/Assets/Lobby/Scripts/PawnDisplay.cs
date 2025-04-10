using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PawnDisplay : MonoBehaviour
{
    [SerializeField] private Image pawnImage;

    public void Setup(Sprite pawnSprite)
    {
        pawnImage.sprite = pawnSprite;
   }
}
