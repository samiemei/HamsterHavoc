using UnityEngine;

public class Wheel : Interactable
{
    [SerializeField] private float funGainPerSecond = 20f;
    [SerializeField] private float energyCostPerSecond = 8f;
    [SerializeField] private Transform usePoint;

    private void Reset()
    {
        primaryNeed = NeedType.Fun;
        interactionDuration = 4f;
        capacity = 1;
    }

    public override Vector2 GetUsePosition()
    {
        return usePoint ? usePoint.position : base.GetUsePosition();
    }

    public override void OnUseTick(GameObject user, float dt)
    {
        if (user.TryGetComponent<HamsterNeeds>(out var needs))
        {
            needs.ModifyNeed(NeedType.Fun, +funGainPerSecond * dt);
            needs.ModifyNeed(NeedType.Energy, -energyCostPerSecond * dt);
        }
    }
}
