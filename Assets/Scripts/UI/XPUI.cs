using UnityEngine;
using UnityEngine.UI;

public class XPUI : MonoBehaviour
{
    public Text levelText;
    public Text xpText;
    public Image xpFill;

    private GameXPManager xpManager;

    void Start()
    {
        xpManager = GameXPManager.Instance;

        if (xpManager == null)
        {
            Debug.LogWarning("[XPUI] No GameXPManager.Instance found in scene.");
            return;
        }
        
        xpManager.OnXPChanged += HandleXPChanged;
        xpManager.OnLevelChanged += HandleLevelChanged;
        
        HandleLevelChanged(xpManager.CurrentLevel);
        HandleXPChanged(xpManager.CurrentXP, xpManager.XPToNextLevel);
    }

    void OnDestroy()
    {
        if (xpManager != null)
        {
            xpManager.OnXPChanged -= HandleXPChanged;
            xpManager.OnLevelChanged -= HandleLevelChanged;
        }
    }

    void HandleLevelChanged(int newLevel)
    {
        if (levelText)
            levelText.text = $"Level {newLevel}";
    }

    void HandleXPChanged(int currentXP, int xpToNext)
    {
        if (xpText)
            xpText.text = $"{currentXP} / {xpToNext}";

        if (xpFill)
        {
            float t = xpToNext > 0 ? (float)currentXP / xpToNext : 0f;
            xpFill.fillAmount = Mathf.Clamp01(t);
        }
    }
}