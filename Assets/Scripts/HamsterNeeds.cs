using UnityEngine;

public class HamsterNeeds : MonoBehaviour
{
    [Header("Starting Values")]
    [Range(0,100)] public float hunger = 70f;
    [Range(0,100)] public float thirst = 70f;
    [Range(0,100)] public float energy = 70f;
    [Range(0,100)] public float fun = 50f;
    [Range(0,100)] public float comfort = 60f;

    [Header("Decay per minute")]
    public float hungerDecay = 10f;
    public float thirstDecay = 12f;
    public float energyDecay = 6f;
    public float funDecay = 8f;
    public float comfortDecay = 4f;

    public float GetValue(NeedType type)
    {
        return type switch
        {
            NeedType.Hunger => hunger,
            NeedType.Thirst => thirst,
            NeedType.Energy => energy,
            NeedType.Fun => fun,
            NeedType.Comfort => comfort,
            _ => 0f
        };
    }

    public void ModifyNeed(NeedType type, float delta)
    {
        switch (type)
        {
            case NeedType.Hunger:   hunger   = Mathf.Clamp(hunger + delta,   0f, 100f); break;
            case NeedType.Thirst:   thirst   = Mathf.Clamp(thirst + delta,   0f, 100f); break;
            case NeedType.Energy:   energy   = Mathf.Clamp(energy + delta,   0f, 100f); break;
            case NeedType.Fun:      fun      = Mathf.Clamp(fun + delta,      0f, 100f); break;
            case NeedType.Comfort:  comfort  = Mathf.Clamp(comfort + delta,  0f, 100f); break;
        }
    }

    public NeedType GetLowestNeed()
    {
        float minVal = Mathf.Infinity;
        NeedType minNeed = NeedType.None;

        void Check(NeedType t, float v) { if (v < minVal) { minVal = v; minNeed = t; } }

        Check(NeedType.Hunger, hunger);
        Check(NeedType.Thirst, thirst);
        Check(NeedType.Energy, energy);
        Check(NeedType.Fun, fun);
        Check(NeedType.Comfort, comfort);

        return minNeed;
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        float m = dt / 60f; // per-minute decay

        ModifyNeed(NeedType.Hunger,   -hungerDecay * m);
        ModifyNeed(NeedType.Thirst,   -thirstDecay * m);
        ModifyNeed(NeedType.Energy,   -energyDecay * m);
        ModifyNeed(NeedType.Fun,      -funDecay * m);
        ModifyNeed(NeedType.Comfort,  -comfortDecay * m);
    }
}
