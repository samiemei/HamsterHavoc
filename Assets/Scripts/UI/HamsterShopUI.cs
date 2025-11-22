using UnityEngine;
using UnityEngine.UI;

public class HamsterShopUI : MonoBehaviour
{
    [System.Serializable]
    public class SkinItem
    {
        public string id;              
        public string displayName;     
        public int cost = 10;          
        public Sprite icon;            
        
        public Image iconImage;
        public Text nameText;
        public Text costText;
        public Button buyButton;
        public GameObject ownedTag;    

        [HideInInspector] public bool unlocked;
    }
    
    public Text currencyText;          
    
    public SkinItem[] skins;           

    GameXPManager xp;

    void Start()
    {
        xp = GameXPManager.Instance;
        if (!xp)
        {
            Debug.LogWarning("[HamsterShopUI] No GameXPManager found in scene.");
            return;
        }
        
        
        for (int i = 0; i < skins.Length; i++)
        {
            InitSkinUI(i);
        }
        
        xp.OnPoopCurrencyChanged += HandleCurrencyChanged;
        HandleCurrencyChanged(xp.poopCurrency); 
    }

    void OnDestroy()
    {
        if (xp != null)
            xp.OnPoopCurrencyChanged -= HandleCurrencyChanged;
    }

    void HandleCurrencyChanged(int newAmount)
    {
        if (currencyText)
            currencyText.text = $"Poops: {newAmount}";
    }

    void InitSkinUI(int index)
    {
        var item = skins[index];
        if (!item.buyButton) return;
        
        string key = $"skin_{item.id}_unlocked";
        item.unlocked = PlayerPrefs.GetInt(key, 0) == 1;
        
        if (item.iconImage && item.icon) item.iconImage.sprite = item.icon;
        if (item.nameText) item.nameText.text = item.displayName;
        if (item.costText) item.costText.text = item.cost.ToString();
        
        item.buyButton.onClick.RemoveAllListeners();
        int capturedIndex = index;
        item.buyButton.onClick.AddListener(() => OnBuyClicked(capturedIndex));

        UpdateSkinUIVisuals(item);
    }

    void UpdateSkinUIVisuals(SkinItem item)
    {
        if (item.ownedTag)
            item.ownedTag.SetActive(item.unlocked);

        if (item.buyButton)
        {
            item.buyButton.interactable = !item.unlocked;
            var btnText = item.buyButton.GetComponentInChildren<Text>();
            if (btnText)
                btnText.text = item.unlocked ? "Owned" : $"Buy ({item.cost})";
        }
    }

    public void OnBuyClicked(int index)
    {
        if (xp == null) return;
        var item = skins[index];
        
        if (!item.unlocked)
        {
            if (!xp.TrySpendPoopCurrency(item.cost))
            {
                Debug.Log("[Shop] Not enough poops!");
                return;
            }

            item.unlocked = true;
            PlayerPrefs.SetInt($"skin_{item.id}_unlocked", 1);
            PlayerPrefs.Save();
            UpdateSkinUIVisuals(item);
            Debug.Log($"[Shop] Purchased skin '{item.id}' for {item.cost} poops.");
        }
        else
        {
            Debug.Log($"[Shop] Skin '{item.id}' already owned, equipping instead.");
        }

        //equips on current hamster
        var skinManager = HamsterSkinManager.Instance;
        var hud = NeedsHUDController.Instance;

        if (skinManager != null && hud != null)
        {
            var hamsterAppearance = hud.CurrentTargetAppearance;
            if (hamsterAppearance != null)
            {
                bool equipped = skinManager.TryEquipSkin(hamsterAppearance, item.id);
                if (equipped)
                    Debug.Log($"[Shop] Equipped skin '{item.id}' on hamster '{hamsterAppearance.name}'.");
                else
                    Debug.Log($"[Shop] Could not equip skin '{item.id}' (maybe not unlocked?).");
            }
            else
            {
                Debug.Log("[Shop] No current hamster selected to equip skin on.");
            }
        }
    }

  
    public bool IsSkinUnlocked(string id)
    {
        for (int i = 0; i < skins.Length; i++)
        {
            if (skins[i].id == id)
                return skins[i].unlocked;
        }
        return false;
    }
}