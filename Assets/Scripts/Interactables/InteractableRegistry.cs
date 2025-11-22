using System.Collections.Generic;
using UnityEngine;

public class InteractableRegistry : MonoBehaviour
{
    public static InteractableRegistry Instance { get; private set; }

    private readonly List<Interactable> all = new();

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
        Interactable[] existing;

#if UNITY_2023_1_OR_NEWER
        existing = Object.FindObjectsByType<Interactable>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );
#else
#pragma warning disable 618
        existing = Object.FindObjectsOfType<Interactable>();
#pragma warning restore 618
#endif

        foreach (var i in existing)
        {
            Register(i);
        }

        Debug.Log($"[Registry] Initialized with {all.Count} interactables.");
    }

    public void Register(Interactable i)
    {
        if (!i) return;
        if (!all.Contains(i))
            all.Add(i);
    }

    public void Unregister(Interactable i)
    {
        if (!i) return;
        all.Remove(i);
    }

    public Interactable FindNearest(Vector2 from, NeedType need, float maxRange, int cageId)
    {
        Interactable best = null;
        float bestDist = maxRange;

        foreach (var i in all)
        {
            if (!i) continue;
            if (i.PrimaryNeed != need) continue;
            if (i.cageId != cageId) continue;

            float d = Vector2.Distance(from, i.GetUsePosition());
            if (d < bestDist)
            {
                bestDist = d;
                best = i;
            }
        }
        return best;
    }
}