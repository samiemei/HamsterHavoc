using UnityEngine;

public class ShopToggleUI : MonoBehaviour
{
    public GameObject shopPanel;

    void Start()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    public void OpenShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    public void ToggleShop()
    {
        if (!shopPanel) return;
        shopPanel.SetActive(!shopPanel.activeSelf);
    }
}