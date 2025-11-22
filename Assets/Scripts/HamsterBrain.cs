using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HamsterNeeds))]
public class HamsterBrain : MonoBehaviour
{
    private enum State { Idle, Moving, Interacting }

    [Header("Movement")]
    public float moveSpeed = 1.6f;
    public float decisionInterval = 0.4f;
    public float interactionTime = 2.5f;
    public float arrivalRadius = 0.25f;

    
    public float triggerThreshold = 35f;
    
    public float desperateThreshold = 15f;

    //stop interacting after this
    public float satisfactionTarget = 80f;
    
    public float perNeedCooldown = 4f;

    public int cageId = 0;

    private Rigidbody2D rb;
    private HamsterNeeds needs;
    private Interactable target;
    private NeedType currentNeed = NeedType.None; 
    private Vector2 destination;
    private State state = State.Idle;
    private float decideTimer;
    private float interactTimer;

    
    private readonly Dictionary<NeedType, float> cooldownUntil = new();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        needs = GetComponent<HamsterNeeds>();
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
        
        cooldownUntil[NeedType.Hunger] = 0f;
        cooldownUntil[NeedType.Thirst] = 0f;
        cooldownUntil[NeedType.Energy] = 0f;
        cooldownUntil[NeedType.Fun] = 0f;
        cooldownUntil[NeedType.Comfort] = 0f;
    }

    void Update()
    {
        decideTimer -= Time.deltaTime;
        if (decideTimer <= 0f && state != State.Interacting)
        {
            DecideNext();
            decideTimer = decisionInterval;
        }

        switch (state)
        {
            case State.Moving: MoveTowardDestination(); break;

            case State.Interacting:
                rb.linearVelocity = Vector2.zero;
                interactTimer -= Time.deltaTime;

                if (target) target.OnUseTick(gameObject, Time.deltaTime);

                // stop early if hits satisfaction target
                if (currentNeed != NeedType.None &&
                    needs.GetValue(currentNeed) >= satisfactionTarget)
                {
                    FinishInteraction();
                    break;
                }

                if (interactTimer <= 0f)
                {
                    FinishInteraction();
                }
                break;

            case State.Idle:
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    void FinishInteraction()
    {
        if (target) target.OnCompleteUse(gameObject);
        
        if (currentNeed != NeedType.None)
            cooldownUntil[currentNeed] = Time.time + perNeedCooldown;

        target = null;
        currentNeed = NeedType.None;
        state = State.Idle;
    }

    void DecideNext()
    {
        // 1) Figure out which need we care about right now
        NeedType lowest = needs.GetLowestNeed();
        float v = needs.GetValue(lowest);

        bool isDesperate = v <= desperateThreshold;
        bool isTriggered = v <= triggerThreshold;
        bool cooledDown = Time.time >= (cooldownUntil.TryGetValue(lowest, out var until) ? until : 0f);

        // 2) If nothing is urgent enough, just wander in this cage
        if (!(isDesperate || (isTriggered && cooledDown)))
        {
            destination = (Vector2)transform.position + Random.insideUnitCircle * 2.0f;
            state = State.Moving;
            currentNeed = NeedType.None;
            target = null;
            return;
        }

        // 3) Ask the registry for the best target IN THIS CAGE
        Interactable best = null;
        if (InteractableRegistry.Instance != null)
        {
            best = InteractableRegistry.Instance.FindNearest(
                (Vector2)transform.position,
                lowest,
                50f,
                cageId              // 🔑 only look at interactables in my cage
            );
        }

        // 4) If we found something, go there. Otherwise, wander.
        if (best != null)
        {
            target = best;
            currentNeed = lowest;
            destination = best.GetUsePosition();
            state = State.Moving;
        }
        else
        {
            destination = (Vector2)transform.position + Random.insideUnitCircle * 2.0f;
            state = State.Moving;
            currentNeed = NeedType.None;
            target = null;
        }

        // 5) Debug logging
        if (target == null)
            Debug.Log($"[Brain] No target for need {lowest} in cage {cageId} " +
                      $"(registry {(InteractableRegistry.Instance ? "OK" : "NULL")})");
        else
            Debug.Log($"[Brain] Going to {target.name} for {target.PrimaryNeed} in cage {cageId}");
    }

    void MoveTowardDestination()
    {
        Vector2 pos = transform.position;
        Vector2 to = destination - pos;
        float dist = to.magnitude;

        if (dist <= arrivalRadius)
        {
            rb.linearVelocity = Vector2.zero;

            if (target != null)
            {
                interactTimer = interactionTime;
                target.OnStartUse(gameObject);
                state = State.Interacting;
                GameXPManager.Instance.AddXP(0);
            }
            else
            {
                state = State.Idle;
            }
            return;
        }

        // movement
        float slowRadius = arrivalRadius * 3f;
        float speed = dist < slowRadius ? Mathf.Lerp(0f, moveSpeed, dist / slowRadius) : moveSpeed;
        Vector2 next = Vector2.MoveTowards(pos, destination, speed * Time.deltaTime);
        rb.MovePosition(next);
    }
}
