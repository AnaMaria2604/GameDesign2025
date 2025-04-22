using UnityEngine;

public class DebugTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("✅ Consola funcționează corect!");
        Debug.LogWarning("⚠️ Acesta este un mesaj de tip Warning.");
        Debug.LogError("❌ Acesta este un mesaj de tip Error.");
    }
}
