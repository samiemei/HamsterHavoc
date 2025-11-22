using UnityEngine;

public class NeedsHUDController : MonoBehaviour
{
    public static NeedsHUDController Instance { get; private set; }
    
    public HamsterNeeds target;
    public bool autoFindFirst = true;
    
    public NeedBarUI hungerBar;
    public NeedBarUI thirstBar;
    public NeedBarUI energyBar;
    public NeedBarUI funBar;
    public HamsterNeeds CurrentTarget => target;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (hungerBar) hungerBar.SetLabel("Hunger");
        if (thirstBar) thirstBar.SetLabel("Thirst");
        if (energyBar) energyBar.SetLabel("Energy");
        if (funBar) funBar.SetLabel("Fun");

        if (!target && autoFindFirst)
        {
            var first = Object.FindFirstObjectByType<HamsterNeeds>(FindObjectsInactive.Exclude);
            if (first != null)
                target = first;
        }
    }

    void Update()
    {
        if (!target) return;

        if (hungerBar)  hungerBar.SetValue01(Mathf.Clamp01(target.hunger  / 100f));
        if (thirstBar)  thirstBar.SetValue01(Mathf.Clamp01(target.thirst  / 100f));
        if (energyBar)  energyBar.SetValue01(Mathf.Clamp01(target.energy  / 100f));
        if (funBar)     funBar.SetValue01(Mathf.Clamp01(target.fun      / 100f));
    }

    public HamsterAppearance CurrentTargetAppearance
    {
        get
        {
            if (!target) return null;
            return target.GetComponent<HamsterAppearance>();
        }
    }
    
    public void SetTarget(HamsterNeeds newTarget)
    {
        target = newTarget;
    }
}