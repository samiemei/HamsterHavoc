using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HamsterNeeds))]
public class HamsterBrain2D : MonoBehaviour
{
    private enum State { Idle, Moving, Interacting }

    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public float decisionInterval = 1f;
    public float interactionTime = 2f;

    private Rigidbody2D rb;
    private HamsterNeeds needs;
    private Interactable target;
    private Vector2 destination;
    private State state = State.Idle;
    private float interactTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        needs = GetComponent<HamsterNeeds>();
    }

    void OnEnable() => StartCoroutine(BrainLoop());

    IEnumerator BrainLoop()
    {
        var wait = new WaitForSeconds(decisionInterval);
        while (enabled)
        {
            switch (state)
            {
                case State.Idle:
                    DecideNext();
                    break;

                case State.Moving:
                    MoveTowardDestination();
                    break;

                case State.Interacting:
                    interactTimer -= Time.deltaTime;
                    if (target)
                        target.OnUseTick(gameObject, Time.deltaTime);
                    if (interactTimer <= 0)
                    {
                        if (target) target.OnCompleteUse(gameObject);
                        target = null;
                        state = State.Idle;
                    }
                    break;
            }
            yield return wait;
        }
    }

    void DecideNext()
    {
        var need = needs.GetLowestNeed();
        target = InteractableRegistry.Instance?.FindNearest(transform.position, need, 10f);

        if (target)
        {
            destination = target.GetUsePosition();
            state = State.Moving;
        }
        else
        {
            // wander randomly
            destination = (Vector2)transform.position + Random.insideUnitCircle * 2f;
            state = State.Moving;
        }
    }

    void MoveTowardDestination()
    {
        Vector2 dir = (destination - (Vector2)transform.position);
        if (dir.magnitude < 0.1f)
        {
            rb.velocity = Vector2.zero;
            if (target)
            {
                interactTimer = interactionTime;
                target.OnStartUse(gameObject);
                state = State.Interacting;
            }
            else state = State.Idle;
        }
        else
        {
            rb.velocity = dir.normalized * moveSpeed;
        }
    }
}