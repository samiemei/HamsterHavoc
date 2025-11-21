using System.Collections.Generic;
using UnityEngine;

public class InteractableRegistry : MonoBehaviour
{
    public static InteractableRegistry Instance { get; private set; }

    private readonly List<Interactable> all = new();

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Register(Interactable i) { if (i && !all.Contains(i)) all.Add(i); }
    public void Unregister(Interactable i) { if (i) all.Remove(i); }

    void Start()
    {
        // optional sweep so you don't need a separate AutoRegister component
        var all = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
        foreach (var i in all) Register(i);
    }

    public Interactable FindNearest(Vector2 from, NeedType need, float maxRange)
    {
        Interactable best = null;
        float bestDist = Mathf.Infinity;

        foreach (var i in all)
        {
            if (!i || i.PrimaryNeed != need) continue;
            float d = Vector2.Distance(from, i.GetUsePosition());
            if (d < bestDist && d <= maxRange) { bestDist = d; best = i; }
        }
        return best;
    }
}