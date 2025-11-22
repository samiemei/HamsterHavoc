using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Poop : MonoBehaviour
{
    [Header("General")]
    public bool isBonus = false;          // set by spawner
    public float stinkRadius = 1.2f;
    public float comfortPenaltyPerSecond = 4f;

    [Header("Lifetime")]
    [Tooltip("How long a normal poop stays before despawning.")]
    public float normalLifeSeconds = 8f;  // faster than before

    [Tooltip("How long a bonus poop stays before despawning.")]
    public float bonusLifeSeconds = 3f;   // VERY quick

    [Header("XP Rewards")]
    public int normalXpReward = 5;
    public int bonusXpReward = 20;

    [Header("FX (optional)")]
    public ParticleSystem cleanFX;
    public AudioSource cleanSfx;
    public Sprite normalSprite;
    public Sprite bonusSprite;           // optional shiny sprite

    float lifeSeconds;
    int xpReward;
    float t;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        // default to normal config if not set by spawner
        ApplyBonusState(isBonus);
    }

    void Update()
    {
        // stink effect
        var hits = Physics2D.OverlapCircleAll(transform.position, stinkRadius);
        foreach (var h in hits)
        {
            if (h.TryGetComponent<HamsterNeeds>(out var needs))
            {
                needs.ModifyNeed(NeedType.Comfort, -comfortPenaltyPerSecond * Time.deltaTime);
            }
        }

        // lifetime countdown
        t += Time.deltaTime;
        if (t >= lifeSeconds)
            Clean();   // auto-despawn (no XP if player missed it)
    }

    public void Clean()
    {
        // notify XP + poop counter
        if (GameXPManager.Instance != null)
        {
            GameXPManager.Instance.AddXP(xpReward);
            GameXPManager.Instance.RegisterPoopCollected(isBonus);
        }

        if (cleanFX) Instantiate(cleanFX, transform.position, Quaternion.identity);
        if (cleanSfx) cleanSfx.Play();

        Destroy(gameObject);
    }

    /// <summary> Called by the spawner to turn this into bonus or normal poop. </summary>
    public void ApplyBonusState(bool bonus)
    {
        isBonus = bonus;

        lifeSeconds = isBonus ? bonusLifeSeconds : normalLifeSeconds;
        xpReward   = isBonus ? bonusXpReward   : normalXpReward;

        var sr = GetComponent<SpriteRenderer>();
        if (sr)
        {
            if (isBonus && bonusSprite != null) sr.sprite = bonusSprite;
            else if (!isBonus && normalSprite != null) sr.sprite = normalSprite;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 0.3f, 0.25f);
        Gizmos.DrawSphere(transform.position, stinkRadius);
    }
#endif
}
