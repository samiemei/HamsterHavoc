using UnityEngine;

[RequireComponent(typeof(HamsterNeeds))]
public class HamsterPooper : MonoBehaviour
{
    [Header("Spawning")]
    public GameObject poopPrefab;
    public float minInterval = 25f;
    public float maxInterval = 55f;
    public int maxActivePoopsPerHamster = 3;

    [Header("After eating bonus")]
    public float eatBonusWindow = 20f;
    public float eatPoopChance = 0.6f;

    [Header("Special Poop")]
    [Tooltip("Chance this spawn will be a bonus poop (0–1).")]
    public float bonusPoopChance = 0.15f;

    float nextTime;
    int activeCount;
    float eatBonusUntil;

    void OnEnable() => ScheduleNext();

    void ScheduleNext()
    {
        nextTime = Time.time + Random.Range(minInterval, maxInterval);
    }

    void Update()
    {
        if (!poopPrefab) return;
        if (activeCount >= maxActivePoopsPerHamster) return;

        if (Time.time >= nextTime)
        {
            SpawnPoop();
            ScheduleNext();
        }
    }

    public void NotifyAteFood()
    {
        eatBonusUntil = Time.time + eatBonusWindow;

        if (Random.value < eatPoopChance)
        {
            if (activeCount < maxActivePoopsPerHamster)
                SpawnPoop();
        }
    }

    void SpawnPoop()
    {
        var p = Instantiate(
            poopPrefab,
            (Vector2)transform.position + Random.insideUnitCircle * 0.1f,
            Quaternion.identity
        );
        activeCount++;
        
        bool makeBonus =
            Random.value < bonusPoopChance ||
            Time.time <= eatBonusUntil;               // often bonus right after eating

        var poop = p.GetComponent<Poop>();
        if (poop != null)
            poop.ApplyBonusState(makeBonus);

        var destroyWatcher = p.AddComponent<OnDestroyedCallback>();
        destroyWatcher.onDestroyed = () => activeCount = Mathf.Max(0, activeCount - 1);
    }

    class OnDestroyedCallback : MonoBehaviour
    {
        public System.Action onDestroyed;
        void OnDestroy() { onDestroyed?.Invoke(); }
    }
}
