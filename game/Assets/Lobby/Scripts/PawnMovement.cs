using UnityEngine;

public class PawnMovement : MonoBehaviour
{
    public Transform homeZone;
    public Transform startSquare;
    public bool isOnBoard = false;
    public LocalPlayer Owner; // Nou: jucator local asociat pionului
    public PawnDisplay display;


    public void MoveToStart()
{
    if (!isOnBoard)
    {
        transform.position = startSquare.position;
        isOnBoard = true;

        GameLogicManager logic = FindObjectOfType<GameLogicManager>();
        logic?.UpdateAllPawnSprites();
    }
}


    

    public void UpdateSprite(int count, CharacterVariants variants)
    {
        if (count <= 1)
            display.Setup(variants.baseSprite);
        else if (count == 2)
            display.Setup(variants.x2Sprite);
        else if (count == 3)
            display.Setup(variants.x3Sprite);
        else
            display.Setup(variants.x4Sprite);
    }

}