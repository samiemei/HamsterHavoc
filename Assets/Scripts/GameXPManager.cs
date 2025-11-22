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


    public int startingCagesUnlocked = 1;

    private int cagesUnlocked = 0;
    

    // Events you can hook UI into if you want later
    public event Action<int> OnLevelChanged;           // new level
    public event Action<int, int> OnXPChanged;         // currentXP, xpToNext
    public event Action<int> OnCageUnlocked;           // index of new cage

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);  // optional; remove if you don't want persistence

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
    
    public void RegisterPoopCollected(bool bonus)
    {
        totalPoopsCollected++;
        if (bonus) totalBonusPoopsCollected++;

        OnPoopCollected?.Invoke(totalPoopsCollected, totalBonusPoopsCollected);

        // Debug so you can see it working:
        Debug.Log($"[XP] Poops collected: {totalPoopsCollected} (bonus: {totalBonusPoopsCollected})");
    }

    // --- Public API ---------------------------------------------------------

    public void AddXP(int amount)
    {
        if (amount <= 0) return;

        CurrentXP += amount;
        NotifyXPChanged();

        // Handle multiple level-ups in case of big XP bursts
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


        // Optional: sort them so IDs are in a predictable order
        System.Array.Sort(existing, (a, b) =>
            a.transform.position.x.CompareTo(b.transform.position.x));

        for (int i = 0; i < existing.Length; i++)
        {
            var area = existing[i];
            area.cageId = i;                     // 0, 1, 2, ...
            CageAreaAssignIds(area);             // stamp hamsters + interactables
            if (CameraCageController.Instance != null)
                CameraCageController.Instance.RegisterCage(area);
        }

        // Now this reflects how many cages already exist.
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
            cageArea.cageId = cagesUnlocked;   // will now be 1, 2, 3, ...
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
