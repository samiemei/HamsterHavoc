using UnityEngine;

public class DebugResetSkins : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.DeleteKey("skin_purple_unlocked");
        
        PlayerPrefs.Save();
        Debug.Log("Reset skin unlocks for debug.");
    }
}