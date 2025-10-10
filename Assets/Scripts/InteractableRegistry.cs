using System.Collections.Generic;
using UnityEngine;

public class InteractableRegistry : MonoBehaviour
{
    public static InteractableRegistry Instance { get; private set; }
    private List<Interactable> all = new();

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void Register(Interactable i)
    {
        if (!all.Contains(i)) all.Add(i);
    }

    public void Unregister(Interactable i) => all.Remove(i);

    public Interactable FindNearest(Vector2 from, NeedType need, float maxRange)
    {
        Interactable best = null;
        float bestDist = Mathf.Infinity;
        foreach (var i in all)
        {
            if (i == null || i.PrimaryNeed != need) continue;
            float d = Vector2.Distance(from, i.transform.position);
            if (d < bestDist && d <= maxRange)
            {
                best = i;
                bestDist = d;
            }
        }
        return best;
    }
}