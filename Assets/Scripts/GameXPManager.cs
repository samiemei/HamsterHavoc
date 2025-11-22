using UnityEngine;
using System;
using Object = UnityEngine.Object;

public class GameXPManager : MonoBehaviour
{
    public static GameXPManager Instance { get; private set; }

    
    [SerializeField] private int startingLevel = 1;
    
    [SerializeField] private int baseXPToLevel = 100;
    
    [SerializeField] private int xpPerLevelIncrease = 25;

    public int CurrentLevel { get; private set; }
    public int CurrentXP { get; private set; }
    public int XPToNextLevel => baseXPToLevel + xpPerLevelIncrease * (CurrentLevel - 1);

  
    public GameObject cagePrefab;

 
    public Transform[] cageSpawnPoints;
    
    public int totalPoopsCollected { get; private set; }
    public int totalBonusPoopsCollected { get; private set; }
    public System.Action<int,int> OnPoopCollected;
    
    public int poopCurrency { get; private set; }

    public System.Action<int> OnPoopCurrencyChanged;


    public int startingCagesUnlocked = 1;

    private int cagesUnlocked = 0;
    
    
    public event Action<int> OnLevelChanged;   
    public event Action<int, int> OnXPChanged;      
    public event Action<int> OnCageUnlocked;   

    void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); 

        CurrentLevel = startingLevel;
        CurrentXP = 0;
        
        NotifyXPChanged();
        OnLevelChanged?.Invoke(CurrentLevel);
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitExistingCages(); 
    }
    
    public void AddPoopCurrency(int amount)
    {
        if (amount <= 0) return;
        poopCurrency += amount;
        OnPoopCurrencyChanged?.Invoke(poopCurrency);
    }
    
    
    public bool TrySpendPoopCurrency(int cost)
    {
        if (cost <= 0) return true;
        if (poopCurrency < cost) return false;

        poopCurrency -= cost;
        OnPoopCurrencyChanged?.Invoke(poopCurrency);
        return true;
    }
    
    public void RegisterPoopCollected(bool bonus)
    {
        totalPoopsCollected++;
        if (bonus) totalBonusPoopsCollected++;

        // 1 = 1 rn
        poopCurrency++;
        
        OnPoopCurrencyChanged?.Invoke(poopCurrency);
        OnPoopCollected?.Invoke(totalPoopsCollected, totalBonusPoopsCollected);

        Debug.Log($"[XP] Poops collected: {totalPoopsCollected} (bonus: {totalBonusPoopsCollected}), currency: {poopCurrency}");
    }

    public void AddXP(int amount)
    {
        if (amount <= 0) return;

        CurrentXP += amount;
        NotifyXPChanged();
        
        while (CurrentXP >= XPToNextLevel)
        {
            CurrentXP -= XPToNextLevel;
            LevelUp();
        }
        NotifyXPChanged();
    }
    
    
    public void SetLevel(int level, int xp = 0)
    {
        CurrentLevel = Mathf.Max(1, level);
        CurrentXP = Mathf.Max(0, xp);
        NotifyXPChanged();
        OnLevelChanged?.Invoke(CurrentLevel);
    }

    void LevelUp()
    {
        CurrentLevel++;
        Debug.Log($"[XP] Level up! Now level {CurrentLevel}");

        OnLevelChanged?.Invoke(CurrentLevel);
        
        if (CurrentLevel % 5 == 0)
        {
            UnlockNextCage();
        }
    }
    
    void InitExistingCages()
    {
        CageArea[] existing;
        
        existing = Object.FindObjectsByType<CageArea>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );
        
        System.Array.Sort(existing, (a, b) =>
            a.transform.position.x.CompareTo(b.transform.position.x));

        for (int i = 0; i < existing.Length; i++)
        {
            var area = existing[i];
            area.cageId = i;
            CageAreaAssignIds(area);
            if (CameraCageController.Instance != null)
                CameraCageController.Instance.RegisterCage(area);
        }
        
        cagesUnlocked = existing.Length;
    }



    void UnlockNextCage()
    {
        if (!cagePrefab || cageSpawnPoints == null || cageSpawnPoints.Length == 0)
        {
            Debug.LogWarning("[XP] Tried to unlock cage, but prefab or spawn points are not set.");
            return;
        }

        if (cagesUnlocked >= cageSpawnPoints.Length)
        {
            Debug.Log("[XP] All cage spawn points already used.");
            return;
        }

        var spawnPoint = cageSpawnPoints[cagesUnlocked];
        var cageGO = Instantiate(cagePrefab, spawnPoint.position, Quaternion.identity);

        var cageArea = cageGO.GetComponent<CageArea>();
        if (cageArea)
        {
            cageArea.cageId = cagesUnlocked;
            CageAreaAssignIds(cageArea);

            if (CameraCageController.Instance != null)
                CameraCageController.Instance.RegisterCage(cageArea);
        }

        Debug.Log($"[XP] Unlocked cage #{cagesUnlocked + 1} at {spawnPoint.position}");
        cagesUnlocked++;
        OnCageUnlocked?.Invoke(cagesUnlocked);
    }
    
    void NotifyXPChanged()
    {
        OnXPChanged?.Invoke(CurrentXP, XPToNextLevel);
    }
    
    public static void CageAreaAssignIds(CageArea area)
    {
        if (!area) return;

        int id = area.cageId;
        
        foreach (var interactable in area.GetComponentsInChildren<Interactable>(true))
            interactable.cageId = id;
        
        foreach (var brain in area.GetComponentsInChildren<HamsterBrain>(true))
            brain.cageId = id;
    }

}
