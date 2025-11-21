using UnityEngine;

public class NeedsHUDController : MonoBehaviour
{
    [Header("Target Hamster")]
    public HamsterNeeds target;
    public bool autoFindFirst = true;

    [Header("Bars")]
    public NeedBarUI hungerBar;
    public NeedBarUI thirstBar;
    public NeedBarUI energyBar;
    public NeedBarUI funBar;
    public NeedBarUI comfortBar;

    void Start()
    {
        // label once
        if (hungerBar)  hungerBar.SetLabel("Hunger");
        if (thirstBar)  thirstBar.SetLabel("Thirst");
        if (energyBar)  energyBar.SetLabel("Energy");
        if (funBar)     funBar.SetLabel("Fun");
        if (comfortBar) comfortBar.SetLabel("Comfort");

        if (!target && autoFindFirst)
        {
            var first = Object.FindFirstObjectByType<HamsterNeeds>(FindObjectsInactive.Exclude);
            if (first) target = first;

        }
    }

    void Update()
    {
        if (!target)
            return;

        // map 0..100 to 0..1
        if (hungerBar)  hungerBar.SetValue01(Mathf.Clamp01(target.hunger  / 100f));
        if (thirstBar)  thirstBar.SetValue01(Mathf.Clamp01(target.thirst  / 100f));
        if (energyBar)  energyBar.SetValue01(Mathf.Clamp01(target.energy  / 100f));
        if (funBar)     funBar.SetValue01(Mathf.Clamp01(target.fun      / 100f));
        if (comfortBar) comfortBar.SetValue01(Mathf.Clamp01(target.comfort / 100f));
    }

    public void SetTarget(HamsterNeeds newTarget) => target = newTarget;
}