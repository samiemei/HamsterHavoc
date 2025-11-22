using UnityEngine;
using UnityEngine.InputSystem; 

public class TapToClean : MonoBehaviour
{
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
        if (cam == null)
            Debug.LogError("[TapToClean] No MainCamera found.");
    }

    void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            HandleTap(Mouse.current.position.ReadValue());
        else if (Touchscreen.current != null &&
                 Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            HandleTap(Touchscreen.current.primaryTouch.position.ReadValue());
    }

    void HandleTap(Vector2 screenPos)
    {
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent<Poop>(out var poop))
            {
                poop.Collect();  
                return;
            }
        }
    }
}