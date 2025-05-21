using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions; // adaugă sus

public class UIFlowManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelGoToGame;
    public GameObject panelLandingPage;
    public GameObject panelNameInput;

    [Header("Name Inputs")]
    [SerializeField] private TMP_InputField[] nameInputs;
    public GameObject[] nameInputBlocks; // parent panels for styling (optional)
    [SerializeField] private Button confirmButton;
    private void Start()
    {
        for (int i = 0; i < nameInputs.Length; i++)
        {
            nameInputs[i].onValueChanged.AddListener(delegate { ValidateNames(); });
        }

        ValidateNames(); // apel inițial
    }

    public void StartNewGame()
    {
        panelGoToGame.SetActive(false);
        panelLandingPage.SetActive(true);
    }

    // public void SelectNumberOfPlayers(int count)
    // {
    //     GameSettings.NumberOfPlayers = count;
    //     nameInputs[i].characterLimit = 6;

    //     panelLandingPage.SetActive(false);
    //     panelNameInput.SetActive(true);

    //     for (int i = 0; i < nameInputBlocks.Length; i++)
    //     {
    //         bool isActive = i < count;

    //         // Activează sau dezactivează inputul
    //         nameInputs[i].interactable = isActive;
    //         if (!isActive)
    //         {
    //             nameInputs[i].text = ""; // Șterge textul din câmpurile inactive
    //         }

    //         // Aplică stil vizual pe blocul complet (label + input)
    //         var group = nameInputBlocks[i].GetComponent<CanvasGroup>();
    //         if (group == null)
    //         {
    //             group = nameInputBlocks[i].AddComponent<CanvasGroup>();
    //         }

    //         group.alpha = isActive ? 1f : 0.25f;
    //         group.interactable = isActive;
    //         group.blocksRaycasts = isActive;
    //     }
    // }

    public void SelectNumberOfPlayers(int count)
    {
        GameSettings.NumberOfPlayers = count;

        panelLandingPage.SetActive(false);
        panelNameInput.SetActive(true);

        for (int i = 0; i < nameInputBlocks.Length; i++)
        {
            bool isActive = i < count;

            // Activează sau dezactivează inputul
            nameInputs[i].interactable = isActive;
            nameInputs[i].characterLimit = 10; // ← limita de caractere aplicată tuturor

            if (!isActive)
            {
                nameInputs[i].text = ""; // Șterge textul din câmpurile inactive
            }

            // Aplică stil vizual pe blocul complet (label + input)
            var group = nameInputBlocks[i].GetComponent<CanvasGroup>();
            if (group == null)
            {
                group = nameInputBlocks[i].AddComponent<CanvasGroup>();
            }

            group.alpha = isActive ? 1f : 0.25f;
            group.interactable = isActive;
            group.blocksRaycasts = isActive;
        }
    }


    public void ValidateNames()
    {
        bool allActiveFieldsFilled = true;

        for (int i = 0; i < GameSettings.NumberOfPlayers; i++)
        {
            if (string.IsNullOrWhiteSpace(nameInputs[i].text))
            {
                allActiveFieldsFilled = false;
                break;
            }
        }

        confirmButton.interactable = allActiveFieldsFilled;
    }



    public void ConfirmNamesAndStart()
    {
        GameSettings.PlayerNames.Clear();
        for (int i = 0; i < GameSettings.NumberOfPlayers; i++)
        {
            string rawName = nameInputs[i].text;
            string cleanedName = rawName.Replace(" ", "").Replace("\t", "").Trim(); // elimină toate spațiile
            GameSettings.PlayerNames.Add(cleanedName);
        }

        SceneManager.LoadScene("Scene_Map_01");
    }
}
