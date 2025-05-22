using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLogger : MonoBehaviour
{

        [SerializeField] private TMP_Text logText;
        [SerializeField] private ScrollRect scrollRect;


        public static GameLogger Instance;

        private void Awake()
        {
                if (Instance == null)
                        Instance = this;
                else
                        Destroy(gameObject);
        }

        public void Log(string message)
        {
                logText.text += $"• {message}\n";

                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f; // auto-scroll to bottom
        }
}
