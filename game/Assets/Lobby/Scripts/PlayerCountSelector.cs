using UnityEngine;

public class PlayerCountSelector : MonoBehaviour
{
    public GameObject landingPagePanel;
    public GameObject nameInputPanel;

    public void SelectPlayerCount(int count)
    {
        GameSettings.NumberOfPlayers = count;

        landingPagePanel.SetActive(false);
        nameInputPanel.SetActive(true);
    }
}
