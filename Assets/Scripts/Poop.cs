using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Poop : MonoBehaviour
{
    [Header("General")]
    public bool isBonus = false;
    public float stinkRadius = 1.2f;
    public float comfortPenaltyPerSecond = 4f;

    [Header("Lifetime")]
    public float normalLifeSeconds = 8f;
    public float bonusLifeSeconds = 3f;

    [Header("XP / Currency Rewards")]
    public int normalXpReward = 0; 
    public int bonusXpReward = 0;

    [Header("FX (optional)")]
    public ParticleSystem cleanFX;
    public AudioSource cleanSfx;
    public Sprite normalSprite;
    public Sprite bonusSprite;

    float lifeSeconds;
    float t;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        ApplyBonusState(isBonus);
    }

    void Update()
    {
        t += Time.deltaTime;
        if (t >= lifeSeconds)
        {
            Expire();
        }
    }
    
    public void Collect()
    {
        if (GameXPManager.Instance != null)
        {
            GameXPManager.Instance.RegisterPoopCollected(isBonus);
            
            int xpReward = isBonus ? bonusXpReward : normalXpReward;
            if (xpReward > 0)
                GameXPManager.Instance.AddXP(xpReward);
        }

        if (cleanFX) Instantiate(cleanFX, transform.position, Quaternion.identity);
        if (cleanSfx) cleanSfx.Play();

        Destroy(gameObject);
    }
    
    void Expire()
    {
        Destroy(gameObject);
    }

    public void ApplyBonusState(bool bonus)
    {
        isBonus = bonus;
        lifeSeconds = isBonus ? bonusLifeSeconds : normalLifeSeconds;

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
