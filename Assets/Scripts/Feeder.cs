using UnityEngine;

public class Feeder : Interactable
{
    [SerializeField] private float hungerGainPerSecond = 30f;

    private void Reset()
    {
        primaryNeed = NeedType.Hunger;
        interactionDuration = 3f;
    }

    public override void OnStartUse(GameObject user) { }
    public override void OnUseTick(GameObject user, float dt)
    {
        if (user.TryGetComponent<HamsterNeeds>(out var needs))
            needs.ModifyNeed(NeedType.Hunger, +hungerGainPerSecond * dt);
        if (user.TryGetComponent<HamsterPooper>(out var pooper))
            pooper.NotifyAteFood();
    }
    public override void OnCompleteUse(GameObject user) { }
}