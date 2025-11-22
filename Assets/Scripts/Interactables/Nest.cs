using UnityEngine;

public class Nest : Interactable
{
    [SerializeField] private float energyGainPerSecond = 25f;
    [SerializeField] private float comfortGainPerSecond = 15f;

    private void Reset()
    {
        primaryNeed = NeedType.Energy;
        interactionDuration = 5f;
    }

    public override void OnUseTick(GameObject user, float dt)
    {
        if (user.TryGetComponent<HamsterNeeds>(out var needs))
        {
            needs.ModifyNeed(NeedType.Energy, +energyGainPerSecond * dt);
            needs.ModifyNeed(NeedType.Comfort, +comfortGainPerSecond * dt);
        }
    }
}
