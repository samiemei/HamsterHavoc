using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class TapToClean : MonoBehaviour
{
    [Tooltip("Optional: assign a dedicated 'Poop' layer so clicks only hit poops.")]
    public LayerMask poopLayer;

    void Update()
    {
        Vector2? screenPos = GetPointerDownScreenPos();
        if (!screenPos.HasValue) return;

        // ignore clicks/taps over UI
        if (IsPointerOverUI(screenPos.Value)) return;

        TryCleanAt(screenPos.Value);
    }

    // --- Input helpers (supports both systems) ---
    Vector2? GetPointerDownScreenPos()
    {
#if ENABLE_INPUT_SYSTEM
        // Mouse click
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return Mouse.current.position.ReadValue();

        // Any touch began
        if (Touchscreen.current != null)
        {
            foreach (var t in Touchscreen.current.touches)
                if (t.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                    return t.position.ReadValue();
        }
        return null;
#else
        if (Input.GetMouseButtonDown(0))
            return Input.mousePosition;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            return Input.GetTouch(0).position;

        return null;
#endif
    }

    bool IsPointerOverUI(Vector2 screenPos)
    {
        if (EventSystem.current == null) return false;

        var data = new PointerEventData(EventSystem.current) { position = screenPos };
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        return results.Count > 0;
    }

    void TryCleanAt(Vector2 screenPos)
    {
        var cam = Camera.main;
        if (!cam) return;

        Vector2 world = cam.ScreenToWorldPoint(screenPos);
        int mask = poopLayer.value == 0 ? Physics2D.DefaultRaycastLayers : poopLayer.value;

        var hit = Physics2D.OverlapPoint(world, mask);
        if (hit && hit.TryGetComponent<Poop>(out var poop))
            poop.Clean();
    }
}
