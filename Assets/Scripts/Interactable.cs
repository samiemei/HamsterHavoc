using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected NeedType primaryNeed = NeedType.None;
    [SerializeField] protected float interactionDuration = 3f;

    public NeedType PrimaryNeed => primaryNeed;

    public int cageId = 0;
    public float InteractionDuration => interactionDuration;

    public virtual Vector2 GetUsePosition() => (Vector2)transform.position;

    public virtual void OnStartUse(GameObject user) { }
    public virtual void OnUseTick(GameObject user, float dt) { }
    public virtual void OnCompleteUse(GameObject user) { }
    
    
    protected virtual void OnEnable()
    {
        if (InteractableRegistry.Instance != null)
            InteractableRegistry.Instance.Register(this);
    }

    protected virtual void OnDisable()
    {
        if (InteractableRegistry.Instance != null)
            InteractableRegistry.Instance.Unregister(this);
    }
}