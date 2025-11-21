using UnityEngine;

public class WaterBottle : Interactable
{
    [SerializeField] private float thirstGainPerSecond = 30f;

    private void Reset()
    {
        primaryNeed = NeedType.Thirst;
        interactionDuration = 3f;
    }

    public override void OnUseTick(GameObject user, float dt)
    {
        if (user.TryGetComponent<HamsterNeeds>(out var needs))
            needs.ModifyNeed(NeedType.Thirst, +thirstGainPerSecond * dt);
    }
}
