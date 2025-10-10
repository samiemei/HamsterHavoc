using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected NeedType primaryNeed = NeedType.None;
    [SerializeField] protected float interactionDuration = 3f;
    [SerializeField] protected int capacity = 1;

    public NeedType PrimaryNeed => primaryNeed;
    public virtual Vector2 GetUsePosition() => transform.position;
    public virtual void OnStartUse(GameObject user) { }
    public virtual void OnUseTick(GameObject user, float dt) { }
    public virtual void OnCompleteUse(GameObject user) { }
}