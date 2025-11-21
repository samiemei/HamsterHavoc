using UnityEngine;

public class ClickToSelectHamster : MonoBehaviour
{
    public NeedsHUDController hud;
    public LayerMask hamsterLayer; // assign a layer used by hamster colliders

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var cam = Camera.main;
            if (!cam) return;
            Vector2 world = cam.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.OverlapPoint(world, hamsterLayer);
            if (hit && hit.TryGetComponent<HamsterNeeds>(out var needs))
                hud.SetTarget(needs);
        }
    }
}