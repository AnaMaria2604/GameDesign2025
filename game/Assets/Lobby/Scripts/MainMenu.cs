using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private GameObject selectPlayersPanel = null;

    public void StartGame()
    {
        landingPagePanel.SetActive(false);
        selectPlayersPanel.SetActive(true); // arată panoul cu 2/3/4 players
    }
}
