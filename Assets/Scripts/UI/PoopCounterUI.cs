using UnityEngine;
using UnityEngine.UI;

public class PoopCounterUI : MonoBehaviour
{
    public Text counterText;

    void Start()
    {
        if (!counterText)
            counterText = GetComponent<Text>();

        if (GameXPManager.Instance != null)
        {
            GameXPManager.Instance.OnPoopCollected += HandlePoopCollected;
            // init
            HandlePoopCollected(
                GameXPManager.Instance.totalPoopsCollected,
                GameXPManager.Instance.totalBonusPoopsCollected
            );
        }
    }

    void OnDestroy()
    {
        if (GameXPManager.Instance != null)
            GameXPManager.Instance.OnPoopCollected -= HandlePoopCollected;
    }

    void HandlePoopCollected(int total, int bonusTotal)
    {
        if (!counterText) return;
        counterText.text = $"Poops: {total}  (⭐ {bonusTotal})";
    }
}